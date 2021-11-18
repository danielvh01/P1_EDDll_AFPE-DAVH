using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_DataTransfer.Data;
using System.Text.Json;
using API_DataTransfer.Models;
using Microsoft.AspNetCore.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_DataTransfer.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        User_Collection usersDB = new User_Collection();
        ChatRoom_Collection chatRoomDB = new ChatRoom_Collection();


        [HttpGet("{ID}")]
        public async Task<IActionResult> GetSpecifiedUser([FromRoute] string ID)
        {
            var FindUser = usersDB.GetUserFromID(ID);
            if (FindUser == null)
            {
                return BadRequest();
            }
            return Ok(JsonSerializer.Serialize(FindUser));
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] JsonElement Juser)
        {

            Login _user = JsonSerializer.Deserialize<Login>(Juser.ToString());
            List<User> UserRegistry = usersDB.GetAllUsers().Result.ToList();
            //Verify if the username exists
            for (int i = 0; i < UserRegistry.Count; i++)
            {
                if (UserRegistry.ElementAt(i).Username == _user.Username)
                    return BadRequest();

            }
            User newUser = new User();
            newUser.Id = new MongoDB.Bson.ObjectId();
            newUser.Username = _user.Username;
            newUser.Password = _user.Password;
            await usersDB.AddUsers(newUser);
            return Created("Created", JsonSerializer.Serialize(newUser.Id.ToString()));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] JsonElement Juser)
        {
            string JsonObj = Juser.ToString();
            Login user = JsonSerializer.Deserialize<Login>(JsonObj);
            List<User> UserRegistry = usersDB.GetAllUsers().Result.ToList();
            for (int i = 0; i < UserRegistry.Count; i++)
            {
                if (UserRegistry.ElementAt(i).Username == user.Username && UserRegistry.ElementAt(i).Password == user.Password)
                {
                    var FindUser = UserRegistry.Find(x => x.Username == user.Username);
                    return Ok(JsonSerializer.Serialize(FindUser.Id.ToString()));
                }
            }
            return BadRequest();
        }

        [HttpPut("{ID}")]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] JsonElement Juser)
        {
            User _user = JsonSerializer.Deserialize<User>(Juser.ToString());
            if (_user == null)
            {
                return BadRequest();
            }
            _user.Id = new MongoDB.Bson.ObjectId(id);
            await usersDB.PutUser(_user);
            return Ok();
        }

        [HttpPut("{idSender}/{idReceiver}")]
        public async Task<IActionResult> AddContact(string idSender, string idReceiver)
        {
            var _user = await usersDB.GetUserFromID(idReceiver);
            if (_user == null)
            {
                return BadRequest();
            }
            _user.Id = new MongoDB.Bson.ObjectId(idReceiver);
            _user.ConnectionRequests.Add(idSender);
            await usersDB.PutUser(_user);
            return Ok();
        }

        [HttpPut("accept/{idSender}/{idReceiver}")]
        public async Task<IActionResult> AcceptRequest(string idSender, string idReceiver)
        {
            //Search the user that accepted the request
            var _user = await usersDB.GetUserFromID(idReceiver);
            if (_user == null)
            {
                return BadRequest();
            }
            _user.Id = new MongoDB.Bson.ObjectId(idReceiver);
            //Remove the request from the request list
            _user.ConnectionRequests.Remove(idSender);
            //add to the contacts the user that sent the request
            _user.Contacts.Add(idSender);
            await usersDB.PutUser(_user);

            //Search the user that sent the request
            var _user2 = await usersDB.GetUserFromID(idSender);
            if (_user2 == null)
            {
                return BadRequest();
            }
            _user2.Id = new MongoDB.Bson.ObjectId(idSender);
            //Add to contacts the user that accepted the request
            _user2.Contacts.Add(idReceiver);
            await usersDB.PutUser(_user2);
            return Ok();
        }


        [HttpPut("{ID}")]
        public async Task<IActionResult> RejectRequest([FromRoute] string idSender, string idReceiver)
        {
            //Search the user that accepted the request
            var _user = await usersDB.GetUserFromID(idReceiver);
            if (_user == null)
            {
                return BadRequest();
            }
            _user.Id = new MongoDB.Bson.ObjectId(idReceiver);
            //Remove the request from the request list
            _user.ConnectionRequests.Remove(idSender);
            await usersDB.PutUser(_user);
            return Ok();
        }

        [HttpPost("createChat")]
        public async Task<IActionResult> CreateChat([FromBody] JsonElement JChat)
        {
            ChatRoom _chat = JsonSerializer.Deserialize<ChatRoom>(JChat.ToString());
            await chatRoomDB.AddChat(_chat);
            return Created("Created", JsonSerializer.Serialize(_chat.Id.ToString()));
        }

        [HttpPut("chat/{ID}")]
        public async Task<IActionResult> UpdateChat([FromRoute] string id, [FromBody] JsonElement JChat)
        {
            ChatRoom _chat = JsonSerializer.Deserialize<ChatRoom>(JChat.ToString());
            if (_chat == null)
            {
                return BadRequest();
            }
            _chat.Id = new MongoDB.Bson.ObjectId(id);
            await chatRoomDB.PutChatRoom(_chat);
            return Ok();
        }

    }
}
