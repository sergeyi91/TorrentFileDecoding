using System;
using BEncoding;

namespace TorrentFileDecoding
{
   public class TorrentStatistics
   {
      public string Name;
      public long Uploaded;
      public DateTime AddedOn;
      public string TorrentName;
      public string TrackerId;
      public DateTime? CreatedOn;

      public TorrentStatistics(string torrentName, BDictionary dictionary, TableTorrentFiles torrentFiles)
      {
         TorrentName = (string)torrentName.Clone();
         Name = ((BString) dictionary["caption"]).Value;
         Uploaded = ((BInteger)dictionary["uploaded"]).Value;
         AddedOn = Utils.ConvertToDate(((BInteger)dictionary["added_on"]).Value);

         TorrentFileInfo torrentFileInfo;
         if (!torrentFiles.TryGetValue(TorrentName, out torrentFileInfo))
         {
            torrentFileInfo = TorrentFileEncoder.Decode(TorrentName);
            torrentFiles.AddTorrentFileInfo(TorrentName, torrentFileInfo);
         }

         TrackerId = torrentFileInfo.TrackerId;
         CreatedOn = torrentFileInfo.CreatedOn;
      }

      public TorrentStatistics()
      {
      }

      public TorrentStatistics Clone()
      {
         TorrentStatistics deepCopy = new TorrentStatistics();
         deepCopy.Name = (string)Name.Clone();
         deepCopy.Uploaded = Uploaded;
         deepCopy.AddedOn = AddedOn;
         deepCopy.TorrentName = (string)TorrentName.Clone();
         if (TrackerId != null)
            deepCopy.TrackerId = (string)TrackerId.Clone();
         deepCopy.CreatedOn = CreatedOn;

         return deepCopy;
      }

   }
}
