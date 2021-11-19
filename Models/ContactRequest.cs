using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P1_EDDll_AFPE_DAVH.Models
{
    public class ContactRequest
    {
        public string IDSender { get; set; }
        public string UsernameSender { get; set; }

        public string IDReceiver { get; set; }
        public string UsernameReceiver { get; set; }
    }
}
