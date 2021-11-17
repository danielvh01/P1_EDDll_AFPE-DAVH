using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P1_EDDll_AFPE_DAVH.Models
{
    public class ChatRoom
    {
        List<Message> Messages  { get; }
        List<byte[]> Users { get; }
        public int type { get; }

        public ChatRoom(List<Message> _Messages, List<byte[]> _Users, int _type)
        {
            Messages = _Messages;
            Users = _Users;
            type = _type;
        }
    }
}
