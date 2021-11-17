using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_DataTransfer.Models
{
    public class Chat
    {
        public string ID { get; set; }
        public string name { get; set; }

        public Chat(string _ID, string _name)
        {
            ID = _ID;
            name = _name;
        }
    }
}
