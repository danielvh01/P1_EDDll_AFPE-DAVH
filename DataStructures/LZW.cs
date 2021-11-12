using System;
using System.Collections.Generic;
using System.Text;

namespace DataStructures
{
    public class LZW : ILZWCompressor
    {
        private Dictionary<int, string> dictionary;
        private Dictionary<string, int> dictionaryC;
        public LZW() 
        {
            dictionary = new Dictionary<int, string>();
            dictionaryC = new Dictionary<string, int>();
        }

        private byte binaryToByte(string binaryByte)
        {
            int number = 0;
            int cantBits = binaryByte.Length;
            for (int i = cantBits; i >= 0; i++)
            {
                number += int.Parse(Math.Pow(2, i).ToString()) * int.Parse(binaryByte.Substring(0, 1));
                binaryByte = binaryByte.Remove(0, 1);
            }
            return byte.Parse(number.ToString());
        }
        private int binaryToInt(string Btext)
        {
            int result = 0;
            for (int i = Btext.Length - 1; i >= 0; i--)
            {
                result += Convert.ToInt32(Btext.Substring((Btext.Length - 1) - i, 1)) * Convert.ToInt32(Math.Pow(2, i));
            }
            return result;
        }

        private string byteToBinaryString(byte x)
        {
            int numericValue = Convert.ToInt32(x);
            string result = "";
            result += numericValue / 128;
            numericValue = numericValue % 128;
            result += numericValue / 64;
            numericValue = numericValue % 64;
            result += numericValue / 32;
            numericValue = numericValue % 32;
            result += numericValue / 16;
            numericValue = numericValue % 16;
            result += numericValue / 8;
            numericValue = numericValue % 8;
            result += numericValue / 4;
            numericValue = numericValue % 4;
            result += numericValue / 2;
            numericValue = numericValue % 2;
            result += numericValue / 1;
            return result;
        }

        private string intToBinaryString(int numericValue, int cantBytes)
        {
            string result = "";
            for (int i = cantBytes-1; i >= 0; i--)
            {
                int divisor = Convert.ToInt32(Math.Pow(2, i));
                result += numericValue / divisor;
                numericValue = numericValue % divisor;
            }
            return result;
        }

        public byte[] Compress(string text)
        {
            string binaryCode = "";
            int cantByte = 0;
            int dictionaryCharCant = 0;
            string compressed = string.Empty;
            byte[] result = new byte[0];
            
            if (text != "" || text != null || text.Length != 0)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (!dictionaryC.ContainsKey(text[i].ToString()))
                    {
                        dictionaryC.Add(text[i].ToString(), dictionaryC.Count + 1);
                    }
                }
                dictionaryCharCant = dictionaryC.Count;
                string w = "";

                foreach (char k in text)
                {
                    string wk = w + k;
                    if (dictionaryC.ContainsKey(wk))
                    {
                        w = wk;
                    }
                    else
                    {
                        compressed += dictionaryC[w] + "-";
                        dictionaryC.Add(wk, dictionaryC.Count + 1);
                        w = k.ToString();
                    }
                }
                if (!string.IsNullOrEmpty(w))
                {
                    compressed += dictionaryC[w];
                }
                cantByte = Convert.ToInt32(Math.Round(Math.Log2(dictionaryC.Count), MidpointRounding.ToPositiveInfinity));
                string[] code = compressed.Split("-");
                for (int i = 0; i < code.Length; i++)
                {
                    binaryCode += intToBinaryString(int.Parse(code[i]), cantByte);
                }
                int cant = 8 - binaryCode.Length % 8;
                while (cant > 0)
                {
                    binaryCode += 0;
                    cant--;
                }
                result = new byte[2 + dictionaryCharCant + (binaryCode.Length / 8)];
                result[0] = byte.Parse(cantByte.ToString());
                result[1] = byte.Parse(dictionaryCharCant.ToString());
                int count = 0;
                foreach (var x in dictionaryC.Keys)
                {
                    if (count < dictionaryCharCant)
                    {
                        result[2 + count] = Convert.ToByte(char.Parse(x));
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                for (int i = 0; i < binaryCode.Length / 8; i++)
                {
                    result[i + 2 + dictionaryCharCant] = Convert.ToByte(binaryToInt(binaryCode.Substring(i * 8, 8)));
                }
                return result;

            }
            else
            {
                return result;
            };

        }
        public string Decompression(byte[] compressedText)
        {
            string result = "";
            int bytesPerCharacter = Convert.ToInt32(compressedText[0]);
            int alphabethLength = Convert.ToInt32(compressedText[1]);
            for(int i = 0; i < alphabethLength; i++)
            {
                dictionary.Add(i + 1, Convert.ToString(Convert.ToChar(compressedText[2 + i])));
            }
            string binaryText = "";
            for(int i = 2 + alphabethLength; i < compressedText.Length; i++)
            {
                binaryText += byteToBinaryString(compressedText[i]);
            }
            int previous;
            int current;
            string chain = "";
            string character = "";

            previous = binaryToInt(binaryText.Substring(0, bytesPerCharacter));
            character = dictionary[Convert.ToInt32(previous)];
            result += character;
            for(int i = 1; i < binaryText.Length/bytesPerCharacter && binaryText.Length > bytesPerCharacter; i++)
            {
                current = binaryToInt(binaryText.Substring(i*bytesPerCharacter, bytesPerCharacter));
                if (current!= 0)
                {
                    if (!dictionary.ContainsKey(current))
                    {
                        chain = dictionary[Convert.ToInt32(previous)];
                        chain = chain + character;
                    }
                    else
                    {
                        chain = dictionary[Convert.ToInt32(current)];
                    }
                    result += chain;
                    character = chain.Substring(0, 1);
                    dictionary.Add(dictionary.Count + 1, dictionary[Convert.ToInt32(previous)] + character);
                    previous = current; 
                }
                else
                {
                    break;
                }
            }
            return result;

        }

    }
}
