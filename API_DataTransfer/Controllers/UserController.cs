﻿using API_DataTransfer.Data;
using API_DataTransfer.Models;
using DataStructures;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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
            var FindUser = await usersDB.GetUserFromID(ID);
            if (FindUser == null)
            {
                return BadRequest();
            }
            FindUser.Id = new MongoDB.Bson.ObjectId(ID);
            return Ok(JsonSerializer.Serialize(FindUser));
        }

        [HttpGet("getUsername/{ID}")]
        public async Task<IActionResult> GetUsername([FromRoute] string ID)
        {
            var FindUser = await usersDB.GetUserFromID(ID);
            if (FindUser == null)
            {
                return BadRequest();
            }
            FindUser.Id = new MongoDB.Bson.ObjectId(ID);
            return Ok(FindUser.Username);
        }

        [HttpGet("getByUser/{Username}")]
        public async Task<IActionResult> GetByUser([FromRoute] string Username)
        {
            var FindUser = usersDB.GetAllUsers().Result.ToList().Find(x => x.Username == Username).Id;
            if (FindUser == null)
            {
                return BadRequest();
            }
            return Ok(JsonSerializer.Serialize(FindUser.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] JsonElement Juser)
        {

            Login _user = JsonSerializer.Deserialize<Login>(Juser.ToString());
            List<User> UserRegistry = await usersDB.GetAllUsers();
            //Verify if the username exists
            for (int i = 0; i < UserRegistry.Count; i++)
            {
                if (UserRegistry.ElementAt(i).Username == _user.Username)
                    return BadRequest();

            }
            User newUser = new User();
            SDES cipher = new SDES(Path.GetDirectoryName(@"Configuration\"));
            byte[] PasswordBytes = GetBytes(_user.Password);
            newUser.Username = _user.Username;
            newUser.Password = GetString(cipher.Cipher(PasswordBytes,8));
            await usersDB.AddUsers(newUser);
            return Created("Created", newUser.Id.ToString());
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] JsonElement Juser)
        {
            string JsonObj = Juser.ToString();
            Login user = JsonSerializer.Deserialize<Login>(JsonObj);
            List<User> UserRegistry = usersDB.GetAllUsers().Result.ToList();
            SDES cipher = new SDES(Path.GetDirectoryName(@"Configuration\"));
            byte[] PasswordBytes = GetBytes(user.Password);
            string passwordCiphered = GetString(cipher.Cipher(PasswordBytes, 8));
            for (int i = 0; i < UserRegistry.Count; i++)
            {
                if (UserRegistry.ElementAt(i).Username == user.Username && UserRegistry.ElementAt(i).Password == passwordCiphered)
                {
                    var FindUser = UserRegistry.Find(x => x.Username == user.Username);
                    return Ok(FindUser.Id.ToString());
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

        [HttpPut("addContact/{idReceiver}")]
        public async Task<IActionResult> AddContact([FromRoute] string _Receiver,[FromBody]JsonElement Sender)
        {
            var _sender = JsonSerializer.Deserialize<Contact>(Sender.ToString());            

            var _user = await usersDB.GetUserFromID(_Receiver);
            if (_user == null)
            {
                return BadRequest();
            }
            _user.Id = new MongoDB.Bson.ObjectId(_Receiver);
            _user.ConnectionRequests.Add(_sender);
            await usersDB.PutUser(_user);
            return Ok();
        }

        [HttpPut("accept")]
        public async Task<IActionResult> AcceptRequest(JsonElement Sender, JsonElement Receiver)
        {
            var _sender = JsonSerializer.Deserialize<Contact>(Sender.ToString());
            var _Receiver = JsonSerializer.Deserialize<Contact>(Receiver.ToString());
            //Search the user that accepted the request
            var _user = await usersDB.GetUserFromID(_Receiver.ID);
            if (_user == null)
            {
                return BadRequest();
            }
            _user.Id = new MongoDB.Bson.ObjectId(_Receiver.ID);
            //Remove the request from the request list
            _user.ConnectionRequests.Remove(_sender);
            //add to the contacts the user that sent the request
            _user.Contacts.Add(_sender);
            await usersDB.PutUser(_user);

            //Search the user that sent the request
            var _user2 = await usersDB.GetUserFromID(_sender.ID);
            if (_user2 == null)
            {
                return BadRequest();
            }
            _user2.Id = new MongoDB.Bson.ObjectId(_sender.ID);
            //Add to contacts the user that accepted the request
            _user2.Contacts.Add(_Receiver);
            await usersDB.PutUser(_user2);
            return Ok();
        }


        [HttpPut("reject/{idReceiver}")]
        public async Task<IActionResult> RejectRequest([FromBody] JsonElement Sender, [FromRoute] string idReceiver)
        {
            //Search the user that accepted the request
            var _sender = JsonSerializer.Deserialize<Contact>(Sender.ToString());            
            var _user = await usersDB.GetUserFromID(idReceiver);
            if (_user == null)
            {
                return BadRequest();
            }
            _user.Id = new MongoDB.Bson.ObjectId(idReceiver);
            //Remove the request from the request list
            _user.ConnectionRequests.Remove(_sender);
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


        [HttpGet("  {ID}")]
        public async Task<IActionResult> GetSpecifiedChat([FromRoute] string ID)
        {
            var FindChat = await chatRoomDB.GetChatFromID(ID);
            if (FindChat == null)
            {
                return BadRequest();
            }
            FindChat.Id = new MongoDB.Bson.ObjectId(ID);           
            return Ok(JsonSerializer.Serialize(FindChat));
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }


    }
}
