using System.Collections.Generic;
using BEncoding;

namespace TorrentFileDecoding
{
   class ResumeFileStatistics
   {
      public Dictionary<string, TorrentStatistics> torrents = null;

      public ResumeFileStatistics(BDictionary elements)
      {
         torrents = new Dictionary<string, TorrentStatistics>();
         foreach(KeyValuePair<BString, BElement> element in elements)
         {
            if (element.Key.Value == ".fileguard" || element.Key.Value == "rec")
               continue;

            torrents.Add(element.Key.Value, new TorrentStatistics(element.Key.Value,(BDictionary)element.Value));
         }
      }
   }
}
