using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using DataStructures;

namespace P1_EDDll_AFPE_DAVH.Models
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public List<string> Contacts { get; set; }
        public List<string> ConnectionRequests { get; set; }
        public List<Chat> Chats { get; set; }
        public int a { get; set; }
        public int n { get; set; }
        public int d { get; set;}        
        public int e { get; set; }
        public User()
        {
            
            Random rnd = new Random();
            Contacts = new List<string>();
            ConnectionRequests = new List<string>();
            Chats = new List<Chat>();
            a = rnd.Next(2);

            int p = rnd.Next(16);
            while (!IsPrime(p))
            {
                p = rnd.Next(16);
            }

            int q = rnd.Next(16);
            while (!IsPrime(q))
            {
                q = rnd.Next(16);
            }
            RSA RSACipher = new RSA();

            var result = RSACipher.GenKeys(p,q);
            e = (int)result.Item1;
            n = (int)result.Item2;
            d = (int)result.Item3;
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
