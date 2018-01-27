using System.IO;
using BEncoding;

namespace TorrentFileDecoding
{
   class TorrentFileEncoder
   {
      private static readonly string sc_torrentsFolder = @"B:\Program Files\µTorrent\App\uTorrent";

      public static TorrentFileInfo Decode(string fileName)
      {
         string filePath = Path.Combine(sc_torrentsFolder, fileName);
         FileStream fileStream = new FileStream(filePath, FileMode.Open);
         BEncoder encoder = new BEncoder(fileStream);
         BElement[] elements = encoder.Decode();
         fileStream.Close();
         BDictionary dictionary = (BDictionary)elements[0];

         TorrentFileInfo info = new TorrentFileInfo();
         info.TrackerId = ((BString)dictionary["comment"]).Value;

         BElement creationDateInt;
         if (dictionary.TryGetValue("creation date", out creationDateInt))
            info.CreatedOn = Utils.ConvertToDate(((BInteger)creationDateInt).Value);

         return info;
      }
   }
}
