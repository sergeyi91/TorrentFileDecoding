using System.Collections.Generic;

namespace TorrentFileDecoding
{
   class Calculator
   {
      public static TorrentStatistics Difference(TorrentStatistics oldStat, TorrentStatistics newStat)
      {
         TorrentStatistics result = newStat.Clone();
         result.Uploaded = newStat.Uploaded - oldStat.Uploaded;
         return result;
      }

      public static Dictionary<string, TorrentStatistics> Difference(ResumeFileStatistics oldStatistics, ResumeFileStatistics newStatistics)
      {
         Dictionary<string, TorrentStatistics> diff = new Dictionary<string, TorrentStatistics>();
         foreach(KeyValuePair<string, TorrentStatistics> torrent in newStatistics.torrents)
         {
            TorrentStatistics oldTorrentStatistics = null;
            if (oldStatistics.torrents.TryGetValue(torrent.Key, out oldTorrentStatistics))
            {
               TorrentStatistics diffTorrentStat = Difference(oldTorrentStatistics, torrent.Value);
               if (diffTorrentStat.Uploaded != 0)
                  diff.Add(torrent.Key, diffTorrentStat);
            }
            else
            {
               if (torrent.Value.Uploaded != 0)
                  diff.Add(torrent.Key, torrent.Value);
            }
         }

         return diff;
      }

   }
}
