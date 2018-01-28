using System.Collections.Generic;
using BEncoding;

namespace TorrentFileDecoding
{
   class ResumeFileStatistics
   {
      public Dictionary<string, TorrentStatistics> torrents = null;

      public ResumeFileStatistics(BDictionary elements, TableTorrentFiles torrentFiles)
      {
         torrents = new Dictionary<string, TorrentStatistics>();
         foreach(KeyValuePair<BString, BElement> element in elements)
         {
            if (element.Key.Value == ".fileguard" || element.Key.Value == "rec")
               continue;

            var stat = new TorrentStatistics(element.Key.Value, (BDictionary)element.Value, torrentFiles);
            torrents.Add(element.Key.Value, stat);
         }
      }
   }
}
