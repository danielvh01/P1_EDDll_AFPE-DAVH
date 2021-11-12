using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using API_DataTransfer.Models;

namespace API_DataTransfer.Data
{
    public class User_Collection
    {
        MongoDBR repo = new MongoDBR();

        IMongoCollection<User> collection;

        //If collection is not created yet, it will create one. Otherwise, will collect the actual one.

        public User_Collection(){
            collection = repo.database.GetCollection<User>("Users");
        }

        public async Task AddUsers(User _User){
            await collection.InsertOneAsync(_User);
        }

        public async Task<List<User>> GetContactsList()
        {
            return await collection.FindAsync(new BsonDocument()).Result.ToListAsync();
        }

        public async Task<User> GetUserFromID(string ID)
        {
            return await collection.FindAsync(new BsonDocument { { "_id", new ObjectId(ID) } }).Result.FirstAsync();
        }

        public async Task PutUser(User _user)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, _user.Id);
            await collection.ReplaceOneAsync(filter,_user);
        }
    }
}
