using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace P1_EDDll_AFPE_DAVH.Models
{
    public class Login
    {

        public string Username { get; set; }
        public string Password { get; set; }

        public Login()
        {
            
        }
    }
}
