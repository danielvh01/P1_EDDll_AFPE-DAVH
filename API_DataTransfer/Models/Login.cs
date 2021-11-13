using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API_DataTransfer.Models
{
    public class Login
    {
        [BsonId]
        public ObjectId Id { get;}

        public string Username { get;}
        public string Password { get; }

        public Login()
        {
            
        }
    }
}
