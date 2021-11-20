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
                var nums = new List<int>();

                foreach (int b in content)
                {

                    int ciphereds = (int)BigInteger.ModPow(b, k2, k1);
                    nums.Add(ciphereds);

                }
                byte[] compressedBytes = nums.SelectMany(BitConverter.GetBytes).ToArray();
                byte[] spacer = { default };
                return Combine(spacer, compressedBytes);

            }
            else
            {
                content = content.Skip(1).ToArray();

                var messageNumbers = new int[content.Length / 4];
                Buffer.BlockCopy(content, 0, messageNumbers, 0, content.Length);

                List<byte> descipherContent = new List<byte>();

                foreach (int b in messageNumbers)
                {

                    int ciphered = (int)BigInteger.ModPow(b, k2, k1);

                    descipherContent.Add((byte)ciphered);
                }

                return descipherContent.ToArray();
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

        #endregion
    }
}
