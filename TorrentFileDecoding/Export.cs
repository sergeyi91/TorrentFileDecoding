using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TorrentFileDecoding
{
   class Export
   {
      private static readonly string outputFile = @"C:\Users\Sergey\Desktop\resume\difference.log";

      public static void ToFile(Dictionary<string, TorrentStatistics> stats)
      {
         List<TorrentStatistics> list = new List<TorrentStatistics>();
         foreach(TorrentStatistics stat in stats.Values)
         {
            list.Add(stat);
         }

         list = list.OrderBy(key => key.Uploaded).ToList();

         FileStream fs = new FileStream(outputFile, FileMode.Create);
         StreamWriter writer = new StreamWriter(fs);
         foreach(TorrentStatistics stat in list)
         {
            double uploadMB = stat.Uploaded / (1024.0 * 1024);
            writer.WriteLine(String.Format("{0}\t{1:000.000}", stat.Name, uploadMB));
         }

         writer.Close();
      }

   }
}
