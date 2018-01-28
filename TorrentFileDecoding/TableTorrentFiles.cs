using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DatabaseUtils;

namespace TorrentFileDecoding
{
   class RowTableTorrentFiles: TorrentFileInfo
   {
      public string FileName;
   }

   public class TableTorrentFiles
   {
      private static readonly string sc_tableName = "TorrentFiles";
      private DatabaseConnection m_connection;
      private Dictionary<string, TorrentFileInfo> m_torrents;
      private ValueList<RowTableTorrentFiles> m_newTorrents;

      public TableTorrentFiles(DatabaseConnection connection)
      {
         m_connection = connection;
         m_torrents = new Dictionary<string, TorrentFileInfo>();
         m_newTorrents = new ValueList<RowTableTorrentFiles>();
         fetch();
      }

      private void fetch()
      {
         if (!m_connection.IsTableExists(sc_tableName))
         {
            ValueList<RowTableTorrentFiles> valueList = new ValueList<RowTableTorrentFiles>();
            DataSet resultData = new DataSet();
            resultData.Tables.Add(valueList.GetCollectedData(sc_tableName));
            m_connection.SaveResultData(resultData);
            return;
         }

         string query = $"SELECT * FROM dbo.{sc_tableName}";
         SqlDataReader reader = m_connection.ExecuteQuery(query);
         while (reader.Read())
         {
            var fileName = (string)reader["FileName"];
            var trackerId = (string)reader["TrackerId"];
            var createdOnObject = reader["CreatedOn"];
            DateTime? createdOn = createdOnObject.Equals(DBNull.Value) ? null : (DateTime?)createdOnObject;
            m_torrents.Add(fileName, new TorrentFileInfo(trackerId, createdOn));
         }
      }

      public bool TryGetValue(string fileName, out TorrentFileInfo value)
      {
         return m_torrents.TryGetValue(fileName, out value);
      }

      public void AddTorrentFileInfo(string fileName, TorrentFileInfo info)
      {
         m_torrents.Add(fileName, info);
         RowTableTorrentFiles row = new RowTableTorrentFiles();
         row.TrackerId = info.TrackerId;
         row.CreatedOn = info.CreatedOn;
         row.FileName = fileName;
         m_newTorrents.OnValue(row);
      }

      public void Save()
      {
         Console.WriteLine($"TableTorrentFiles: Save {m_newTorrents.Count} new torrents:");
         foreach (RowTableTorrentFiles row in m_newTorrents)
         {
            Console.WriteLine(row.FileName);
         }
         Console.WriteLine();

         if (m_newTorrents.Count > 0)
         {
            DataSet resultData = new DataSet();
            resultData.Tables.Add(m_newTorrents.GetCollectedData(sc_tableName));
            m_connection.SaveResultData(resultData);
         }
      }
   }
}
