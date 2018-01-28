using System.Data;

using BEncoding;
using DatabaseUtils;

namespace TorrentFileDecoding
{
   class Program
   {
      static void Main(string[] args)
      {
         var database = new DatabaseConnection();
         var tableTorrentFiles = new TableTorrentFiles(database);

         string oldFilePath = @"C:\Users\Sergey\Desktop\resume\resume.dat.2018.01.28H0143";
         string newFilePath = @"C:\Users\Sergey\Desktop\resume\resume.dat.2018.01.28H1757";

         BDictionary oldFileDictionary = ResumeFileEncoder.Decode(oldFilePath);
         BDictionary newFileDictionary = ResumeFileEncoder.Decode(newFilePath);

         ResumeFileStatistics oldStatistics = new ResumeFileStatistics(oldFileDictionary, tableTorrentFiles);
         ResumeFileStatistics newStatistics = new ResumeFileStatistics(newFileDictionary, tableTorrentFiles);

         var diff = Calculator.Difference(oldStatistics, newStatistics);

         Export.ToFile(diff);

         var tableStatsForLastRun = new TableStatsForLastRun(database);
         tableStatsForLastRun.Save(diff.Values);

         tableTorrentFiles.Save();
      }
   }
}
