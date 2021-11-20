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
        public List<Contact> Contacts { get; set; }
        public List<Contact> ConnectionRequests { get; set; }
        public List<string> Chats { get; set; }
        public int a { get; set; }
        public int n { get; set; }
        public int d { get; set;}        
        public int e { get; set; }
        public User()
        {
        }

    }
}
