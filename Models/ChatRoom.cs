using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P1_EDDll_AFPE_DAVH.Models
{
    public class ChatRoom
    {
        List<Message> Messages { get; set; }
        List<string> Users { get; set; }
        public int type { get; set; }

        public string name { get; set; }
        public int g { get; set; }
        public int p { get; set; }
        public int A { get; set; }
        public int B { get; set; }

        public ChatRoom(List<Message> _Messages, List<string> _Users, int _type, string _name, int _A, int _B)
        {
            Messages = _Messages;
            Users = _Users;
            type = _type;
            name = _name;
            Random rnd = new Random();

            int valP = rnd.Next(2, 100);
            while (!IsPrime(valP))
            {
                valP = rnd.Next(2, 100);
            }
            int valG = rnd.Next(2, 100);
            while (valG == valP)
            {
                valG = rnd.Next(2, 100);
            }
            g = valG;
            p = valP;
            A = _A;
            B = _B;
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
