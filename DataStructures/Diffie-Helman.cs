using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace DataStructures
{
    public class Diffie_Helman
    {


        Diffie_Helman()
        {
            
        }

        

        public BigInteger D_H(int g, int p ,int a)
        {
            return BigInteger.ModPow(g, a, p);
        }

        public BigInteger SecretCKey(int a,int B, int p)
        {
            return BigInteger.ModPow(B, a, p);
        }

    }
}
