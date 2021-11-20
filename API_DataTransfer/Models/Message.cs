using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_DataTransfer.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API_DataTransfer.Models
{
    public class Message
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public DateTime dateTime { get; set; }
        public bool visible  { get; set; }
        public List<byte> content { get; set; }
        public int type { get; set; }

        public string title { get; set; }
        public string UserSender { get; set; }

        //Keys for cipher 
        public int k1  { get; set; }
        public int k2 { get; set; } 


        public Message()
        {
            Id = ObjectId.GenerateNewId(); ;
            dateTime = DateTime.Now;
            visible = true;
            content = new List<byte>();
            type = 1;
            UserSender = "";
            k1 = 1;
            k2 = 1;
            title = "";
        }

        public void DeleteForMe()
        {
            visible = false;
        }
    }
}
