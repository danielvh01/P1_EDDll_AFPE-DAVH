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
        public ObjectId Id { get;  }
        public DateTime dateTime { get;  }
        public bool visible  { get; private set; }
        public byte[] content { get; }
        public int type { get; }

        //Keys for cipher 
        public int k1  { get; }
        public int k2 { get; }
    

    public Message(ObjectId _Id, byte[] _content, int _type ,int _k1, int _k2)
        {
            Id = _Id;
            dateTime = DateTime.Now;
            visible = true;
            content = _content;
            type = _type;
            k1 = _k1;
            k2 = _k2;

        }

        public void DeleteForMe()
        {
            visible = false;
        }
    }
}
