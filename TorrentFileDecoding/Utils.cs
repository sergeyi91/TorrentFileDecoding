using System;

namespace TorrentFileDecoding
{
   class Utils
   {
      public static DateTime ConvertToDate(long seconds)
      {
         DateTime time = new DateTime(1970, 1, 1);
         time = time.AddSeconds(seconds);
         return time;
      }
   }
}
