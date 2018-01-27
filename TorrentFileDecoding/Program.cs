using System.Data;

using BEncoding;
using DatabaseUtils;

namespace TorrentFileDecoding
{
   class Program
   {
      static void Main(string[] args)
      {
         string oldFilePath = @"C:\Users\Sergey\Desktop\resume\resume.dat.2018.01.27H0258";
         //string newFilePath = @"C:\Users\Sergey\Desktop\resume\resume.dat.2018.01.27H1339";
         string newFilePath = @"C:\Users\Sergey\Desktop\resume\resume.dat.2018.01.27H1903";

         BDictionary oldFileDictionary = ResumeFileEncoder.Decode(oldFilePath);
         BDictionary newFileDictionary = ResumeFileEncoder.Decode(newFilePath);

         ResumeFileStatistics oldStatistics = new ResumeFileStatistics(oldFileDictionary);
         ResumeFileStatistics newStatistics = new ResumeFileStatistics(newFileDictionary);

         var diff = Calculator.Difference(oldStatistics, newStatistics);

         Export.ToFile(diff);

         ValueList<TorrentStatistics> valueList = new ValueList<TorrentStatistics>();
         foreach (TorrentStatistics stat in diff.Values)
         {
            valueList.OnValue(stat);
         }
         DataSet resultData = new DataSet();
         resultData.Tables.Add(valueList.GetCollectedData("StatsForLastRun"));

         var database = new DatabaseConnection();
         database.SaveResultData(resultData);
      }
   }
}
