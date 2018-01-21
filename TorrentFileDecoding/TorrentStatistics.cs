namespace TorrentFileDecoding
{
   class TorrentStatistics
   {
      public string Caption;
      public long Uploaded;

      public TorrentStatistics(BDictionary dictionary)
      {
         Caption = ((BString) dictionary["caption"]).Value;
         Uploaded = ((BInteger)dictionary["uploaded"]).Value;
      }

      public TorrentStatistics()
      {
      }

   }
}
