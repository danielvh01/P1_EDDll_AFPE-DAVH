using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using API_DataTransfer.Models;

namespace API_DataTransfer.Data
{
    public class Login_Collection
    {
        MongoDBR repo = new MongoDBR();
        IMongoCollection<Login> collection;
        public Login_Collection()
        {
            collection = repo.database.GetCollection<Login>("Login");
        }
    }
}
