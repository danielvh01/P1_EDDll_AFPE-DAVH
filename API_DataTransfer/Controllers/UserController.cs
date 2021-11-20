using API_DataTransfer.Data;
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
            return Ok(FindUser.Username);
        }

        [HttpGet("getByUser/{Username}")]
        public async Task<IActionResult> GetByUser([FromRoute] string Username)
        {
            var FindUser = usersDB.GetAllUsers().Result.ToList().Find(x => x.Username == Username);
            if (FindUser == null)
            {
                return BadRequest();
            }
            return Ok(FindUser.Id.ToString());
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] JsonElement Juser)
        {

            Login _user = JsonSerializer.Deserialize<Login>(Juser.ToString());
            List<User> UserRegistry = await usersDB.GetAllUsers();
            //Verify if the username exists
            if (UserRegistry.Find(x => x.Username == _user.Username) != null)
                return BadRequest();
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
            ICipher<int> cipher = new SDES(Path.GetDirectoryName(@"Configuration\"));
            for (int i = 0; i < UserRegistry.Count; i++)
            {
                User temp = UserRegistry.ElementAt(i);
                if (temp.Username == user.Username && GetString(cipher.Decipher(GetBytes(temp.Password), 8)) == user.Password)
                {
                    return Ok(temp.Id.ToString());
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

        [HttpPut("addingContact")]
        public async Task<IActionResult> AddContact([FromBody] JsonElement request)
        {
            var _request = JsonSerializer.Deserialize<ContactRequest>(request.ToString());
            var _user = await usersDB.GetUserFromID(_request.IDReceiver);
            if (_user == null)
            {
                return BadRequest();
            }
            _user.Id = new MongoDB.Bson.ObjectId(_request.IDReceiver);
            var _sender = new Contact();
            _sender.ID = _request.IDSender;
            _sender.Username = _request.UsernameSender;
            _user.ConnectionRequests.Add(_sender);
            await usersDB.PutUser(_user);
            return Ok();
        }

        [HttpPut("accept")]
        public async Task<IActionResult> AcceptRequest([FromBody] JsonElement request)
        {
            var _request = JsonSerializer.Deserialize<ContactRequest>(request.ToString());
            //Search the user that accepted the request
            var _user = await usersDB.GetUserFromID(_request.IDReceiver);
            if (_user == null)
            {
                return BadRequest();
            }
            _user.Id = new MongoDB.Bson.ObjectId(_request.IDReceiver);
            //Remove the request from the request list
            Contact _sender = new Contact();
            _sender.ID = _request.IDSender;
            _sender.Username = _request.UsernameSender;
            _user.ConnectionRequests.RemoveAt(_user.ConnectionRequests.FindIndex(x => x.ID == _sender.ID && x.Username == _sender.Username));
            _user.Contacts.Add(_sender);
            await usersDB.PutUser(_user);
            //add to the contacts the user that sent the request


            //Search the user that sent the request
            var _user2 = await usersDB.GetUserFromID(_sender.ID);
            if (_user2 == null)
            {
                return BadRequest();
            }
            _user2.Id = new MongoDB.Bson.ObjectId(_sender.ID);
            //Add to contacts the user that accepted the request
            Contact _Receiver = new Contact();
            _Receiver.ID = _request.IDReceiver;
            _Receiver.Username = _request.UsernameReceiver;
            _user2.Contacts.Add(_Receiver);
            await usersDB.PutUser(_user2);
            return Ok();
        }


        [HttpPut("reject")]
        public async Task<IActionResult> RejectRequest([FromBody] JsonElement request)
        {
            var _request = JsonSerializer.Deserialize<ContactRequest>(request.ToString());
            //Search the user that accepted the request
            var _user = await usersDB.GetUserFromID(_request.IDReceiver);
            if (_user == null)
            {
                return BadRequest();
            }
            _user.Id = new MongoDB.Bson.ObjectId(_request.IDReceiver);
            //Remove the request from the request list
            var _sender = new Contact();
            _sender.ID = _request.IDSender;
            _sender.Username = _request.UsernameSender;
            _user.ConnectionRequests.RemoveAt(_user.ConnectionRequests.FindIndex(x => x.ID == _sender.ID && x.Username == _sender.Username));
            await usersDB.PutUser(_user);
            return Ok();
        }

        [HttpPost("createChat")]
        public async Task<IActionResult> CreateChat([FromBody] JsonElement JChat)
        {
            var _chat = JsonSerializer.Deserialize<ChatRoom>(JChat.ToString());
            _chat.Id = MongoDB.Bson.ObjectId.GenerateNewId();
            await chatRoomDB.AddChat(_chat);
            foreach (var user in _chat.Users)
            {
                var x = await usersDB.GetUserFromID(user);
                x.Chats.Add(_chat.Id.ToString());
                await usersDB.PutUser(x);
            }
            return Created("Created", _chat.Id.ToString());
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

        [HttpPut("chat/sendMessage/{ID}")]
        public async Task<IActionResult> SendMessage([FromRoute] string id, [FromBody] JsonElement JMessage)
        {

            Message _message = JsonSerializer.Deserialize<Message>(JMessage.ToString());
            if (_message == null)
            {
                return BadRequest();
            }
            _message.Id = MongoDB.Bson.ObjectId.GenerateNewId();
            var _chat = await chatRoomDB.GetChatFromID(id);
            _chat.Messages.Add(_message);
            await chatRoomDB.PutChatRoom(_chat);
            return Ok();
        }

        [HttpGet("chat/{ID}")]
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
