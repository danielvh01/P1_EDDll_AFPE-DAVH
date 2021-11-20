using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Linq;

namespace DataStructures
{
    public class LZW : ILZWCompressor
    {

        public LZW()
        {

        }

        public byte[] Compress(byte[] uncompressed)
        {
            #region Creating Dictinary
            //Creates the dictionary that will be used along the compress process
            Dictionary<string, ushort> dictionary = new Dictionary<string, ushort>();

            //Dictionary in string that will contain the first non-repeatable bytes in string found in the content.
            string firstDict = string.Empty;

            //Ushort in order to reach the best compression rate, unsigned 16-bit integer. 
            List<ushort> saveCompressedData = new List<ushort>();

            //If the dictionary returns false , the character will be added.
            foreach (char k in uncompressed)
            {
                if (!dictionary.ContainsKey(k.ToString()))
                {
                    dictionary.Add(k.ToString(), (ushort)(dictionary.Count + 1));
                    firstDict += k.ToString();
                }
            }
            //Because we're working with bytes, if the original dictionary length is greater than 256, we will need two bytes to represent it.
            int lengthinbytes = firstDict.Length;
            List<Byte> bytesUsed = new List<Byte>();
            while (lengthinbytes > 255)
            {
                bytesUsed.Add(Convert.ToByte(255));
                lengthinbytes -= 255;
            }
            bytesUsed.Add(Convert.ToByte(lengthinbytes));

            //w is the actual character
            string w = string.Empty;
            foreach (char k in uncompressed)
            {
                //Creating a chain
                string wk = w + k;
                if (dictionary.ContainsKey(wk))
                {
                    w = wk;
                }
                //If the dictionary doesnt contain the chain, will be added.
                else
                {
                    saveCompressedData.Add(dictionary[w]);
                    // wc is a new sequence; add it to the dictionary
                    dictionary.Add(wk, (ushort)(dictionary.Count + 1));
                    w = k.ToString();
                }
            }
            // write remaining output if necessary
            if (!string.IsNullOrEmpty(w))
                saveCompressedData.Add(dictionary[w]);
            #endregion

            #region Building final compressed bytes
            //In order to separate original dictionary from the output, will be used a [nul] byte.
            byte[] enter = { default }; //defaut same as null

            //***** The BitConverter class helps manipulate value types in their fundamental form, 
            //      as a series of bytes. A byte is defined as an 8-bit unsigned integer. The BitConverter 
            //      class includes static methods to convert each of the primitive types to and from an array of bytes

            //      Documentation: https://docs.microsoft.com/en-us/dotnet/api/system.bitconverter?view=net-5.0         *****

            byte[] toCompress = saveCompressedData.SelectMany(BitConverter.GetBytes).ToArray();

            //List of type byte array in order to sort the final compression byte array.
            List<byte[]> arraySorter = new List<byte[]>();
            arraySorter.Add(bytesUsed.ToArray());
            arraySorter.Add(enter);
            arraySorter.Add(firstDict.ToString().Select(Convert.ToByte).ToArray());
            arraySorter.Add(toCompress);

            //Byte array for the return. Length is assigned by the sum of the 4 arrays 
            //(Dictionary original length, a null byte array, the characters of the first dictionary and the entire compressed output.
            byte[] result = new byte[
                                     arraySorter.ElementAt(0).Length +
                                     arraySorter.ElementAt(1).Length +
                                     arraySorter.ElementAt(2).Length +
                                     arraySorter.ElementAt(3).Length];

            //Buffer.BlockCopy copies a specified number of bytes from a source array starting at a
            //particular offset to a destination array starting at a particular offset. In other words, fussionates n quantity of arrays.
            //Documentation: https://docs.microsoft.com/en-us/dotnet/api/system.buffer.blockcopy?view=net-5.0
            int dstoffset = 0;
            for (int i = 0; i < 4; i++)
            {
                var element = arraySorter.ElementAt(i);
                Buffer.BlockCopy(element, 0, result, dstoffset, element.Length);
                dstoffset += element.Length;
            }
            return result;

            #endregion
        }


        public byte[] Decompression(byte[] filebytes)
        {
            #region Generating Original Dictionary
            //Lists in order to convert to array for further processes

            List<Byte> realCOntent = new List<Byte>(); //Bytes without including the dictionary chars and the respective length of it.
            List<Byte> diccionaryChar = new List<Byte>();
            List<Byte> diccionaryLength = new List<Byte>();

            //Extract dictionary from compressed data bytes

            int offset = 0; //Save the last position where the filebyte array was left from the whiles.
            int longg = 0;  //Length of the original dictionary
            int zentinel = 0;

            //While in order to get the original dictionary length
            while (filebytes[offset] != 0)
            {
                diccionaryLength.Add(filebytes[offset]); offset++;
            }
            offset++;

            //Takes the bit(s) assigned in compression of the original dictionary length and then makes the sum to the longg variable.
            for (int i = 0; i < diccionaryLength.Count; i++)
            {
                longg += Convert.ToInt32(diccionaryLength.ElementAt(i));
            }

            //While the zentinel value have not reached longg, the bytes of the original bytes will be added starting at an specific index (offset)
            while (zentinel != longg)
            {
                diccionaryChar.Add(filebytes[offset]); zentinel++; offset++;
            }
            byte[] originDict = diccionaryChar.ToArray();

            //Now, with the original dictionary length and its respective characters already set, we extract the rest bytes that are the compressed output.
            //The while ends until the total compressed bytes length had been reached.
            while (offset != filebytes.Length)
            {
                realCOntent.Add(filebytes[offset]); offset++;
            }


            Dictionary<ushort, string> dictionary = new Dictionary<ushort, string>();
            foreach (char c in originDict)
            {
                dictionary.Add((ushort)(dictionary.Count + 1), c.ToString());
            }

            #endregion

            #region Bulding final decompressed bytes

            //Compressed Data saved in order to fulfill the dictionary with the new combinations
            byte[] realC = realCOntent.ToArray();

            //Generating ushort array, the length is divided by two because realC.length is in INT32 type value, USHORT is an int16 value, the half of it.
            ushort[] compressedBytes = new ushort[realC.Length / 2];

            //Copies all the content to the ushort byte array.
            //Documentation: https://docs.microsoft.com/en-us/dotnet/api/system.buffer.blockcopy?view=net-5.0
            Buffer.BlockCopy(realC, 0, compressedBytes, 0, realC.Length);

            //Takes the first byte of the compressedContent
            string w = dictionary[compressedBytes[0]];

            //StringBuilder Represents a mutable string of characters.
            StringBuilder result = new StringBuilder(w);

            //Skips one because the first one was already used in string w, so we want the next of it.
            compressedBytes = compressedBytes.Skip(1).ToArray();

            //Build Dictionary
            foreach (ushort k in compressedBytes)
            {
                //Takes the first character from previous chain
                string kk = w[0].ToString();
                string wkk = "";
                if (dictionary.ContainsKey(k))
                {
                    wkk = dictionary[k];
                }
                else
                {
                    //Joins previous chain and its first position
                    wkk = w + kk;
                }
                //The StringBuilder Append method appends a string, a substring, a character array, a portion of a character array, a single character repeated multiple times, 
                //or the string representation of a primitive data type to a StringBuilder object.
                //Documentation: https://docs.microsoft.com/en-us/dotnet/api/system.text.stringbuilder?view=net-5.0
                result.Append(wkk);
                dictionary.Add((ushort)(dictionary.Count + 1), w + wkk[0]);
                w = wkk;
            }
            //Converts the StringBuilder object to string, and then is converted to bytes , so the ToArray method can be used.
            byte[] decompressedFile = result.ToString().Select(Convert.ToByte).ToArray();

            return decompressedFile;

            #endregion
        }
    }
}