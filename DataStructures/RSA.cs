using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace DataStructures
{
    public class RSA : ICipher<int[]>
    {
        public RSA()
        {

        }

        #region Generate private and public Keys
        // int #1 and #2 are the public key (d,N) and int #3 and #4 the private key (e,N)
        public (BigInteger, BigInteger, BigInteger) GenKeys(int p,int q)
        {
            BigInteger N = p * q;
            BigInteger phiN = (p - 1)*(q - 1);
            BigInteger e = 2;
            while (e < phiN && (MCD(e, N) != 1 || MCD(e, phiN) != 1))
            {
                e++;
            }
            BigInteger d = GetD(phiN,e);
            return (e,N,d);
        }



        BigInteger GetD(BigInteger phiN, BigInteger e)
        {
            BigInteger d = 0;
            BigInteger k = 2;
            //Choose d
            while ((d*e)%phiN != 1){
                d = (1 + k * phiN) / e;
                k++;
            }            
            return d;            
        }

        BigInteger MCD(BigInteger a, BigInteger b)
        {
            BigInteger result = 0;
            do
            {
                result = b;
                b = a % b;
                a = result;
            }
            while (b != 0);
            return result;
        }

        

        #endregion

        #region Cipher / Decipher method

        public byte[] Cipher(byte[] content, int[] keys)
        {
            return RSApher(content, keys[0], keys[1]);
        }
        public byte[] Decipher(byte[] content, int[] keys)
        {
            return RSApher(content, keys[0], keys[1]);
        }

        byte[] RSApher(byte[] content, int k1, int k2)
        {
            
            if (content[0] != 0)
            {
                byte[] cipheredContent;
                byte[] enter = { default }; //defaut same as null
                List<int> cipherText = new List<int>();
                List<byte[]> arraySorter = new List<byte[]>();

                foreach (int character in content){                    
                    cipherText.Add((int)ModularPower(character, k1, k2));
                }

                cipheredContent = cipherText.SelectMany(BitConverter.GetBytes).ToArray();
                arraySorter.Add(enter);
                arraySorter.Add(cipheredContent);
                byte[] result = new byte[
                                     arraySorter.ElementAt(0).Length +
                                     arraySorter.ElementAt(1).Length];
                int dstoffset = 0;
                for (int i = 0; i < 2; i++)
                {
                    var element = arraySorter.ElementAt(i);
                    Buffer.BlockCopy(element, 0, result, dstoffset, element.Length);
                    dstoffset += element.Length;
                }

                return result;
            }
            else {
                content = content.Skip(1).ToArray();
                var cipheredBytes = new int[(content.Length / 4)];
                Buffer.BlockCopy(content,0, cipheredBytes,0, content.Length -1);
                List<byte> result = new List<byte>();
                foreach (int c in cipheredBytes)
                {
                    result.Add((byte)(ModularPower(c, k1, k2)));
                }
                return result.ToArray();
            }
            
        }

        static byte[] Combine(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        BigInteger ModularPower(BigInteger x, BigInteger y, BigInteger p)
        {
            BigInteger res = 1;
            x = x % p;                       

            if (x == 0){
                return 0;            
            }
            
            while (y > 0)
            {
                if ((y & 1) != 0){
                    res = (res * x) % p;
                }                
                y = y >> 1;
                x = (x * x) % p;
            }
            return res;
        }

        #endregion
    }
}
