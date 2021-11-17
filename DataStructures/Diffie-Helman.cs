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

        

        public BigInteger D_H(int a)
        {
            Random rnd = new Random();

            int valP = rnd.Next(2,100);
            while (!IsPrime(valP))
            {
                valP = rnd.Next(2, 100);
            }
            int valG = rnd.Next(2, 100);
            while (valG == valP)
            {
                valG = rnd.Next(2, 100);
            }
            
            BigInteger A = BigInteger.ModPow(valG,a,valP);

           // BigInteger secretKey1 = BigInteger.ModPow(B,a,valP);
           // BigInteger secretKey2 = BigInteger.ModPow(B, a, valP);
            return 0;

        }
        bool IsPrime(int number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
                if (number % i == 0)
                    return false;

            return true;
        }

    }
}
