using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_DataTransfer.Models
{
    public class ChatRoom
    {
        List<Message> Messages { get; set; }
        List<string> Users { get; set; }
        public int type { get; set; }

        public string name { get; set; }

        public ChatRoom(List<Message> _Messages, List<string> _Users, int _type, string _name)
        {
            Messages = _Messages;
            Users = _Users;
            type = _type;
            name = _name;
        }
    }
}
