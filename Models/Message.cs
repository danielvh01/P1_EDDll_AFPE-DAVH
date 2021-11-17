using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_DataTransfer.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace P1_EDDll_AFPE_DAVH.Models
{
    public class Message
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public DateTime dateTime { get; set; }
        public bool visible  { get; set; }
        public byte[] content { get; set; }
        public int type { get; set; }

        public string IdSender { get; set; }

        //Keys for cipher 
        public int k1  { get; set; }
        public int k2 { get; set; }
    

    public Message(ObjectId _Id, byte[] _content, int _type, string _idSender ,int _k1, int _k2)
        {
            Id = _Id;
            dateTime = DateTime.Now;
            visible = true;
            content = _content;
            type = _type;
            IdSender = _idSender;
            k1 = _k1;
            k2 = _k2;

        }

        public void DeleteForMe()
        {
            visible = false;
        }
    }
}
