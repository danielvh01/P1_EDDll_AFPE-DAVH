﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API_DataTransfer.Models
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
        public int k1 { get; set; }
        public int k2 { get; set;}
        public User()
        {
            Contacts = new List<string>();
            ConnectionRequests = new List<string>();
            Chats = new List<Chat>();
        }

    }
}
