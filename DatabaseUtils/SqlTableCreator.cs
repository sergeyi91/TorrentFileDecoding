/**
@author: Nick Tompson 
(http://social.msdn.microsoft.com/Forums/en-US/adodotnetdataproviders/thread/4929a0a8-0137-45f6-86e8-d11e220048c3)
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DatabaseUtils
{
    /// <summary>
    /// helper class that creates tables by tables schema
    /// </summary>
    class SqlTableCreator
    {
        private SqlConnection m_connection;
        public SqlConnection Connection
        {
            get { return m_connection; }
            set { m_connection = value; }
        }

        private SqlTransaction m_transaction;
        public SqlTransaction Transaction
        {
            get { return m_transaction; }
            set { m_transaction = value; }
        }

        private string m_tableName;
        public string DestinationTableName
        {
            get { return m_tableName; }
            set { m_tableName = value; }
        }

        public SqlTableCreator() { }
        public SqlTableCreator(SqlConnection connection) : this(connection, null) { }
        public SqlTableCreator(SqlConnection connection, SqlTransaction transaction)
        {
            m_connection = connection;
            m_transaction = transaction;
        }

        public object Create(DataTable schema)
        {
            return Create(schema, null);
        }

        public object Create(DataTable schema, int numKeys)
        {
            int[] primaryKeys = new int[numKeys];
            for (int i = 0; i < numKeys; i++)
            {
                primaryKeys = new[] { i };
            }
            return Create(schema, primaryKeys);
        }

        public object Create(DataTable schema, int[] primaryKeys)
        {
            string sql = GetCreateSQL(m_tableName, schema, primaryKeys);

            SqlCommand cmd;
            if (m_transaction != null && m_transaction.Connection != null)
                cmd = new SqlCommand(sql, m_connection, m_transaction);
            else
                cmd = new SqlCommand(sql, m_connection);

            return cmd.ExecuteNonQuery();
        }

        public object CreateFromDataTable(DataTable table)
        {
            string sql = GetCreateFromDataTableSQL(m_tableName, table);

            SqlCommand cmd;
            if (m_transaction != null && m_transaction.Connection != null)
                cmd = new SqlCommand(sql, m_connection, m_transaction);
            else
                cmd = new SqlCommand(sql, m_connection);

            return cmd.ExecuteNonQuery();
        }


        public static string GetCreateSQL(string tableName, DataTable schema, int[] primaryKeys)
        {
            string sql = "CREATE TABLE " + tableName + " (\n";

            // columns
            foreach (DataRow column in schema.Rows)
            {
                if (!(schema.Columns.Contains("IsHidden") && (bool)column["IsHidden"]))
                    sql += column["ColumnName"].ToString() + " " + SQLGetType(column) + ",\n";
            }
            sql = sql.TrimEnd(new char[] { ',', '\n' }) + "\n";

            // primary keys
            string pk = "CONSTRAINT PK_" + tableName + " PRIMARY KEY CLUSTERED (";
            bool hasKeys = (primaryKeys != null && primaryKeys.Length > 0);
            if (hasKeys)
            {
                // user defined keys
                foreach (int key in primaryKeys)
                {
                    pk += schema.Rows[key]["ColumnName"].ToString() + ", ";
                }
            }
            else
            {
                // check schema for keys
                string keys = string.Join(", ", GetPrimaryKeys(schema));
                pk += keys;
                hasKeys = keys.Length > 0;
            }
            pk = pk.TrimEnd(new char[] { ',', ' ', '\n' }) + ")\n";
            if (hasKeys) sql += pk;
            sql += ")";

            return sql;
        }

        public static string GetCreateFromDataTableSQL(string tableName, DataTable table)
        {
            string sql = "CREATE TABLE [" + tableName + "] (\n";
            // columns
            foreach (DataColumn column in table.Columns)
            {
                sql += "[" + column.ColumnName + "] " + SQLGetType(column) + ",\n";
            }
            sql = sql.TrimEnd(new char[] { ',', '\n' }) + "\n";
            // primary keys
            if (table.PrimaryKey.Length > 0)
            {
                sql += "CONSTRAINT [PK_" + tableName + "] PRIMARY KEY CLUSTERED (";
                foreach (DataColumn column in table.PrimaryKey)
                {
                    sql += "[" + column.ColumnName + "],";
                }
                sql = sql.TrimEnd(new char[] { ',' }) + ")\n";
            }

            sql += ")";

            return sql;
        }

        public static string[] GetPrimaryKeys(DataTable schema)
        {
            List<string> keys = new List<string>();

            foreach (DataRow column in schema.Rows)
            {
                if (schema.Columns.Contains("IsKey") && (bool)column["IsKey"])
                    keys.Add(column["ColumnName"].ToString());
            }

            return keys.ToArray();
        }

        // Return T-SQL data type definition, based on schema definition for a column
        public static string SQLGetType(object type, int columnSize, int numericPrecision, int numericScale)
        {
            switch (type.ToString())
            {
                case "System.String":
                    return "NVARCHAR(" +
                        ((columnSize == -1) ? "max" : columnSize.ToString() ) + ")";

                case "System.Decimal":
                    if (numericScale > 0)
                    {
                       return "REAL";
                    }
                    if (numericPrecision > 10)
                    {
                       return "BIGINT";
                    }
                    return "INT";

                case "System.Double":
                case "System.Single":
                    return "REAL";

                case "System.Int64":
                case "System.UInt64":
                case "System.UInt32":
                    return "BIGINT";

                case "System.Int8":
                case "System.UInt8":
                    return "NCHAR(1)";

                case "System.Int16":
                case "System.UInt16":
                case "System.Int32":
                    return "INT";

                case "System.DateTime":
                    return "DATETIME2";

                case "System.Boolean":
                    return "BIT";

                default:
                    throw new Exception(type.ToString() + " not implemented.");
            }
        }

        // Overload based on row from schema table
        public static string SQLGetType(DataRow schemaRow)
        {
            return SQLGetType(schemaRow["DataType"],
                                int.Parse(schemaRow["ColumnSize"].ToString()),
                                int.Parse(schemaRow["NumericPrecision"].ToString()),
                                int.Parse(schemaRow["NumericScale"].ToString()));
        }
        // Overload based on DataColumn from DataTable type
        public static string SQLGetType(DataColumn column)
        {
            return SQLGetType(column.DataType, column.MaxLength, 10, 2);
        }
    }
}
