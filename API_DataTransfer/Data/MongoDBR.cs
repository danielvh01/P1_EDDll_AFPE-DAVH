using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace API_DataTransfer.Data
{
    public class MongoDBR
    {
        public MongoClient client;
        public IMongoDatabase database;

        public MongoDBR(){
            client = new MongoClient("mongodb://localhost:27017"); database = client.GetDatabase("Database");
        }
    }
}
