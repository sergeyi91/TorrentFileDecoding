using System;

namespace TorrentFileDecoding
{
   public class TorrentFileInfo
   {
      public string TrackerId;
      public DateTime? CreatedOn;

      public TorrentFileInfo()
      {
      }

      public TorrentFileInfo(string trackerId, DateTime? createdOn)
      {
         TrackerId = trackerId;
         CreatedOn = createdOn;
      }
   }
}
