using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace DataStructures
{
    public class SDES : ICipher<int>
    {
        string path;
        public SDES(string p)
        {
            path = p;
            PermutationConfigurator();
        }
        string[] P10out;
        string[] P8out;
        string[] P4out;
        string[] EPout;
        string[] IPout;
        string[] IP_1out;

        string[,] SBOXo = new string [4,4]

            { { "01","00","11","10"},
              { "11","10","01","00"},
              { "00","10","01","11"},
              { "11","01","11","10"} };

        string[,] SBOX1 = new string[4, 4]

            { { "00","01","10","11"},
              { "10","00","01","11"},
              { "11","00","01","00"},
              { "10","01","00","11"} };


        #region methods used by cipher and decipher methods

        void PermutationConfigurator()
        {
            string filename = "Permutations.txt";
            string[] files = File.ReadAllLines(path + "\\" + filename);
            P10out = files[0].Split(",");
            P8out = files[1].Split(",");
            P4out = files[2].Split(",");
            EPout = files[3].Split(",");
            IPout = files[4].Split(",");
            IP_1out = files[5].Split(",");
        }
        string P10(string key)
        {            
            string P10array = "";
            for (int i = 0; i < 10; i++)
            {
                P10array+= key[int.Parse(P10out[i])-1];
            }
            return P10array;
        }

        string LeftShift(string p10array)
        {
            string LeftShiftarray = "";
            for (int i = 0; i < 9; i++)
            {                
                LeftShiftarray += p10array[i + 1];
            }
            LeftShiftarray += p10array[0];
            return LeftShiftarray;
        }

        string P8(string LeftShiftarray)
        {            
            string P8array = "";
            for (int i = 0; i < 8; i++)
            {
                P8array += LeftShiftarray[int.Parse(P8out[i])-1];
            }
            return P8array;
        }

        string LeftShift2(string LeftShiftarray)
        {
            string LeftShift2array = "";
            for (int i = 0; i < 8; i++)
            {
                LeftShift2array += LeftShiftarray[i + 2];
            }
            LeftShift2array += LeftShiftarray[0];
            LeftShift2array += LeftShiftarray[1];
            return LeftShift2array;
        }


        string IP(string array)
        {            
            string IP = "";
            for (int i = 0; i < 8; i++)
            {
                IP += array[int.Parse(IPout[i])-1];
            }
            return IP;
        }

        string IP_1(string array)
        {            
            string IP_1 = "";
            for (int i = 0; i < 8; i++)
            {
                IP_1 += array[int.Parse(IP_1out[i])-1];
            }
            return IP_1;
        }

        string EP(string array)
        {            
            string EP = "";
            for (int i = 0; i < 8; i++)
            {
                EP += array[int.Parse(EPout[i])-1];
            }
            return EP;
        }

        string P4(string array)
        {            
            string P4 = "";
            for (int i = 0; i < 4; i++)
            {
                P4 += array[int.Parse(P4out[i])-1];
            }
            return P4;
        }

        string xor(string key1, string key2)
        {
            string result = "";
            for (int i = 0; i < key1.Length; i++)
            {
                if (key1[i] != key2[i])
                {
                    result += "1";
                }
                else {
                    result += "0";
                }
            }
            return result;
        }

        string byteToBinaryString(byte x)
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

        string[] genK1K2(int key)
        {
            string binary = Convert.ToString(key, 2);
            int cont = binary.Length;
            while (cont < 10)
            {
                binary = "0" + binary;
                cont++;
            }
            string LS1 = LeftShift(P10(binary));
            string k1 = P8(LS1);
            string k2 = P8(LeftShift2(LS1));
            string[] generatedKeys = {k1,k2};
            return generatedKeys;
        }

        string Switch(string xor2, string restIP)
        {
            string result = restIP + xor2;
            return result;
        }

        string fk1(string IPgen, string k1)
        {
            //Primeros 4 generados por IP que iran a compararse con XOR
            string IP_4 = IPgen.Substring(0, 4);
            //EP->Xor con K1 ->Sboxes->P4
            string firstXor = xor(EP(IPgen.Substring(4)), k1);

            string fo = firstXor[0].ToString() + firstXor[3].ToString();
            string co = firstXor[1].ToString() + firstXor[2].ToString();
            string S0 = SBOXo[Convert.ToInt32(fo, 2), Convert.ToInt32(co, 2)];

            string fl = firstXor[4].ToString() + firstXor[7].ToString();
            string cl = firstXor[5].ToString() + firstXor[6].ToString();
            string Sl = SBOXo[Convert.ToInt32(fl, 2), Convert.ToInt32(cl, 2)];

            string sw1tch = Switch(xor(P4(S0 + Sl), IP_4), IPgen.Substring(4));
            return sw1tch;
        }

        int fk2(string switched, string k2)
        {
            //Primeros 4 generados por Switch que iran a compararse con XOR
            string SW_4 = switched.Substring(0, 4);
            //EP->Xor con K2 ->Sboxes->P4
            string firstXor = xor(EP(switched.Substring(4)), k2);

            string fo = firstXor[0].ToString() + firstXor[3].ToString();
            string co = firstXor[1].ToString() + firstXor[2].ToString();
            string S0 = SBOXo[Convert.ToInt32(fo, 2), Convert.ToInt32(co, 2)];

            string fl = firstXor[4].ToString() + firstXor[7].ToString();
            string cl = firstXor[5].ToString() + firstXor[6].ToString();
            string Sl = SBOXo[Convert.ToInt32(fl, 2), Convert.ToInt32(cl, 2)];

            string iIP = IP_1(xor(P4(S0 + Sl), SW_4) + switched.Substring(4));
            return Convert.ToInt32(iIP, 2);
        }
        #endregion

        public byte[] Cipher(byte [] message, int key)
        {            
            byte[] result = new byte[message.Length];
            string[] k1k2 = genK1K2(key);
            for (int index = 0; index < message.Length; index++)
            {                
                string Ipermutation = IP(Convert.ToString(message[index],2).PadLeft(8,'0'));
                result[index] = (byte)fk2(fk1(Ipermutation, k1k2[0]), k1k2[1]);
            }                       
            return result;
        }

        public byte[] Decipher(byte[] message, int key)
        {
            byte[] result = new byte[message.Length];
            string[] k1k2 = genK1K2(key);
            for (int index = 0; index < message.Length; index++)
            {
                string Ipermutation = IP(Convert.ToString(message[index], 2).PadLeft(8, '0'));
                result[index] = (byte)fk2(fk1(Ipermutation, k1k2[1]), k1k2[0]);
            }
            return result;
        }

    }
}
