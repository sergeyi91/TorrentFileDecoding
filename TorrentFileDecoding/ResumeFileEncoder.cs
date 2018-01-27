using System;
using System.IO;
using BEncoding;

namespace TorrentFileDecoding
{
   class ResumeFileEncoder
   {
      public static BDictionary Decode(string filePath)
      {
         FileStream fileStream = new FileStream(filePath, FileMode.Open);
         BEncoder encoder = new BEncoder(fileStream);
         BElement[] elements = encoder.Decode();
         fileStream.Close();
         if (elements.Length != 1)
            throw new Exception("Unexpected number of elements in resume.dat");

         return (BDictionary)elements[0];
      }
   }
}
