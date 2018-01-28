using System.Collections.Generic;
using System.Data;
using DatabaseUtils;

namespace TorrentFileDecoding
{
   public class TableStatsForLastRun
   {
      private static readonly string sc_tableName = "StatsForLastRun";
      private DatabaseConnection m_connection;
      public TableStatsForLastRun(DatabaseConnection connection)
      {
         m_connection = connection;
      }

      public void Save(IEnumerable<TorrentStatistics> stats)
      {
         ValueList<TorrentStatistics> valueList = new ValueList<TorrentStatistics>();
         foreach (TorrentStatistics stat in stats)
         {
            valueList.OnValue(stat);
         }
         DataSet resultData = new DataSet();
         resultData.Tables.Add(valueList.GetCollectedData(sc_tableName));
         m_connection.SaveResultData(resultData);
      }
   }
}
