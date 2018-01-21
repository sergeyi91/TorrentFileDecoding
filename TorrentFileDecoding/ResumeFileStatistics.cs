using System.Collections.Generic;

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

            torrents.Add(element.Key.Value, new TorrentStatistics((BDictionary)element.Value));
         }
      }
   }
}
