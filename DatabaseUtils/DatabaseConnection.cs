using System;
using System.Data;
using System.Data.SqlClient;

namespace DatabaseUtils
{
   public class DatabaseConnection
   {
      private SqlConnection m_connection;
      private bool m_newDatabase;
      private bool m_needToDropTables;
      private string m_serverName;
      private string m_databaseName;
      private string m_dbLogin;
      private string m_dbPassword;
      private int m_sqlCommandTimeoutSec;
      private int m_sqlBulkCopyTimeoutSec;

      public DatabaseConnection()
      {
         m_dbLogin = "sa";
         m_dbPassword = "pass";
         m_serverName = @"SERGEYPC\SQLEXPRESS";
         m_sqlCommandTimeoutSec = 60;
         m_sqlBulkCopyTimeoutSec = 60;
         m_databaseName = "TorrentStats";
         m_newDatabase = false;
         m_needToDropTables = true;

         ensureConnection();

         // TODO [sergeyi] consider introducing schemaName as parameter

         if (m_newDatabase)
         {
            createDatabase();
         }
         else
         {
            checkDatabase();
         }
      }

      private void switchDatabase()
      {
         m_connection.ChangeDatabase(m_databaseName);
      }

      private void configureConnection()
      {
         m_connection = new SqlConnection();
         SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
         {
            DataSource = (m_serverName == String.Empty) ? "(local)" : m_serverName,
            IntegratedSecurity = m_dbLogin == string.Empty ? true : false,
         };
         if (m_dbLogin != string.Empty)
         {
            builder.UserID = m_dbLogin;
            builder.Password = m_dbPassword;
         }

         m_connection.ConnectionString = builder.ConnectionString;
      }

      private void ensureConnection()
      {
         if (m_connection != null)
         {
            return; //connection already initialized
         }

         configureConnection();

         try
         {
            m_connection.Open();
            return;
         }
         catch (SqlException ex)
         {
            Console.WriteLine($"Connection to SQL Server failed: '{ex.Message}'.");
         }
      }

      private void dropConnection()
      {
         if (m_connection != null)
         {
            m_connection.Close();
            m_connection.Dispose();
            m_connection = null;
         }
      }

      private void createDatabase()
      {
         try
         {
            executeSql($"CREATE DATABASE [{m_databaseName}];");
         }
         catch (SqlException ex)
         {
            Console.WriteLine($"Failed to create '{m_databaseName}' database: {ex.Message}");
         }
      }

      private void checkDatabase()
      {
         try
         {
            // Check DB exists and we can access it
            switchDatabase();
         }
         catch (SqlException ex)
         {
            Console.WriteLine($"Failed to access '{m_databaseName}' database: {ex.Message}");
         }
      }

      private void executeSql(string sql)
      {
         try
         {
            SqlCommand cmd = new SqlCommand(sql, m_connection)
            {
               CommandTimeout = m_sqlCommandTimeoutSec
            };

            cmd.ExecuteNonQuery();
            return;
         }
         catch (SqlException ex)
         {
            Console.WriteLine($"SQL command failed: '{ex.Message}'");
            dropConnection();
            ensureConnection();
            switchDatabase();
         }
      }

      public bool IsTableExists(string tableName)
      {
         string sql =
             "declare @exists bit; " +
             "select @exists = 0; " +
             "if exists (select * from dbo.sysobjects " +
             "where id = object_id(N'[dbo].[" + tableName + "]') " +
             "and OBJECTPROPERTY(id, N'IsUserTable') = 1)" +
             "select @exists = 1; " +
             "select @exists;";
         SqlCommand cmd = new SqlCommand(sql, m_connection);
         return (bool)cmd.ExecuteScalar();
      }

      private void dropTable(string tableName)
      {
         executeSql("drop table dbo." + tableName);
      }

      // Creates tables. Drops old tables on first call if NewTables=1 is in params
      private void createTables(DataSet collectedData)
      {
         SqlTableCreator creator = new SqlTableCreator(m_connection);
         foreach (DataTable table in collectedData.Tables)
         {
            bool exists = IsTableExists(table.TableName);
            if (exists && !m_needToDropTables)
            {
               continue;
            }
            if (exists && m_needToDropTables)
            {
               dropTable(table.TableName);
            }
            creator.DestinationTableName = table.TableName;
            creator.CreateFromDataTable(table);
         }
         m_needToDropTables = false;
      }

      private void copyData(DataSet collectedData)
      {
         // Note: what bulk copy fails in the middle due to SQL server anavailablity?
         // Transaction is muc slower. What else can be done?
         foreach (DataTable table in collectedData.Tables)
         {
            SqlBulkCopy bulkCopy = new SqlBulkCopy(m_connection)
            {
               BulkCopyTimeout = m_sqlBulkCopyTimeoutSec,
               DestinationTableName = table.TableName
            };
            bulkCopy.WriteToServer(table);
            bulkCopy.Close();
         }
      }

      public void SaveResultData(DataSet collectedData)
      {
         switchDatabase();
         createTables(collectedData);
         copyData(collectedData);
      }

      public SqlDataReader ExecuteQuery(string query)
      {
         SqlCommand command = m_connection.CreateCommand();
         command.CommandText = query;
         return command.ExecuteReader();
      }
   }
}
