using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_DataTransfer.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace API_DataTransfer.Data
{
    public class ChatRoom_Collection
    {
        MongoDBR repo = new MongoDBR();

        IMongoCollection<ChatRoom> collection;

        //If collection is not created yet, it will create one. Otherwise, will collect the actual one.

        public ChatRoom_Collection()
        {
            collection = repo.database.GetCollection<ChatRoom>("ChatRooms");
        }

        public async Task AddChat(ChatRoom chatroom)
        {
            await collection.InsertOneAsync(chatroom);
        }

        public async Task<List<ChatRoom>> GetAllChatRooms()
        {
            return await collection.FindAsync(new BsonDocument()).Result.ToListAsync();
        }

        public async Task<ChatRoom> GetChatFromID(string ID)
        {
            return await collection.FindAsync(new BsonDocument { { "_id", new ObjectId(ID) } }).Result.FirstAsync();
        }

        public async Task PutChatRoom(ChatRoom _chatRoom)
        {
            var filter = Builders<ChatRoom>.Filter.Eq(x => x.Id, _chatRoom.Id);
            await collection.ReplaceOneAsync(filter, _chatRoom);
        }
    }
}
