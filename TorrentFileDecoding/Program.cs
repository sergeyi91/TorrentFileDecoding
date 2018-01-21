using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorrentFileDecoding
{
   class Program
   {
      static void Main(string[] args)
      {
         //string filename1 = @"C:\Users\Sergey\Desktop\resume\resume.dat";
         //string filename2 = @"C:\Users\Sergey\Desktop\resume\resume.dat.old";

         string filename1 = @"C:\Users\Sergey\Desktop\resume\resume.dat.new2";
         string filename2 = @"C:\Users\Sergey\Desktop\resume\resume.dat";

         FileStream fs1 = new FileStream(filename1, FileMode.Open);
         Encoder encoder1 = new Encoder(fs1);
         BElement[] elements1 = encoder1.Decode();
         fs1.Close();

         FileStream fs2 = new FileStream(filename2, FileMode.Open);
         Encoder encoder2 = new Encoder(fs2);
         BElement[] elements2 = encoder2.Decode();
         fs2.Close();

         ResumeFileStatistics oldStatistics = new ResumeFileStatistics((BDictionary)elements2[0]);
         ResumeFileStatistics newStatistics = new ResumeFileStatistics((BDictionary)elements1[0]);

         var diff = Calculator.Difference(oldStatistics, newStatistics);

         Export.ToFile(diff);

      }
   }
}
