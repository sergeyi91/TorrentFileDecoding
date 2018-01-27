using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BEncoding
{
   public class BEncoder
   {
      private FileStream m_stream;
      private byte[] m_byteArray;
      private readonly long m_byteArraySize = 4000000;

      public BEncoder(FileStream stream)
      {
         m_stream = stream;
         m_byteArray = new byte[m_byteArraySize];
      }
      public BElement[] Decode()
      {
         List<BElement> rootElements = new List<BElement>();
         while (m_stream.Position < m_stream.Length)
         {
            rootElements.Add(readElement());
         }

         return rootElements.ToArray();
      }

      private BElement readElement()
      {
         int b = m_stream.ReadByte();
         switch (b)
         {
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9': m_stream.Position--; return readString();
            case 'i': return readInteger();
            case 'l': return readList();
            case 'd': return readDictionary();
            default: throw new Exception("unknown type of byte");
         }
      }

      private BString readString()
      {
         int length = 0;

         int currentByte = '0';
         do
         {
            length = length * 10 + (currentByte - '0');
            currentByte = m_stream.ReadByte();
         } while (currentByte != ':');

         if (length >= m_byteArraySize)
            throw new Exception("temporary byte array is too short");

         m_stream.Read(m_byteArray, 0, length);

         string result = Encoding.UTF8.GetString(m_byteArray, 0, length);
         return result;
      }

      private BInteger readInteger()
      {
         long value = 0;
         int currentByte = '0';
         do
         {
            value = value * 10 + (currentByte - '0');
            currentByte = m_stream.ReadByte();
         } while (currentByte != 'e');

         return new BInteger(value);
      }

      private BList readList()
      {
         BList list = new BList();

         int b = m_stream.ReadByte();
         bool endOfList = (b == 'e');
         while (!endOfList)
         {
            m_stream.Position--;
            BElement element = readElement();
            list.Add(element);
            b = m_stream.ReadByte();
            endOfList = (b == 'e');
         }

         return list;
      }

      private BDictionary readDictionary()
      {
         BDictionary dict = new BDictionary();

         int b = m_stream.ReadByte();
         bool endOfDictionary = (b == 'e');
         while (!endOfDictionary)
         {
            m_stream.Position--;
            BString key = readString();
            BElement value = readElement();
            dict.Add(key, value);
            b = m_stream.ReadByte();
            endOfDictionary = (b == 'e');
         }

         return dict;
      }

   }
}
