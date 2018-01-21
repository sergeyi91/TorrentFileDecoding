using System.IO;

namespace TorrentFileDecoding
{
   class Program
   {
      static void Main(string[] args)
      {
         string oldFilePath = @"C:\Users\Sergey\Desktop\resume\resume.dat.2018.01.21H0053";
         string newFilePath = @"C:\Users\Sergey\Desktop\resume\resume.dat.2018.01.21H1445";

         BDictionary oldFileDictionary = ResumeFileEncoder.Decode(oldFilePath);
         BDictionary newFileDictionary = ResumeFileEncoder.Decode(newFilePath);

         ResumeFileStatistics oldStatistics = new ResumeFileStatistics(oldFileDictionary);
         ResumeFileStatistics newStatistics = new ResumeFileStatistics(newFileDictionary);

         var diff = Calculator.Difference(oldStatistics, newStatistics);

         Export.ToFile(diff);
      }
   }
}
