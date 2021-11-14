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


        [HttpGet("{username}")]
        public async Task<IActionResult> GetSpecifiedUser([FromRoute] string username)
        {
            List<User> AllUsers = usersDB.GetAllUsers().Result.ToList();
            if (AllUsers.Count == 0)
            {
                return BadRequest();
            }
            var FindUser = AllUsers.Find(x => x.Username == username);
            if (FindUser == null)
            {
                return BadRequest();
            }
            return Ok(JsonSerializer.Serialize(await usersDB.GetUserFromID(FindUser.Id.ToString())));
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] JsonElement Juser)
        {

            User _user = JsonSerializer.Deserialize<User>(Juser.ToString());
            List<User> UserRegistry = usersDB.GetAllUsers().Result.ToList();
            //Verify if the username exists
            for (int i = 0; i < UserRegistry.Count; i++)
            {
                if (UserRegistry.ElementAt(i).Username == _user.Username)
                    return BadRequest();

            }
            await usersDB.AddUsers(_user);
            return Created("Created", true);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] JsonElement Juser)
        {
            string JsonObj = Juser.ToString();
            User user = JsonSerializer.Deserialize<User>(JsonObj);
            List<User> UserRegistry = usersDB.GetAllUsers().Result.ToList();
            for (int i = 0; i < UserRegistry.Count; i++)
            {
                if (UserRegistry.ElementAt(i).Username == user.Username && UserRegistry.ElementAt(i).Password == user.Password)
                {
                    var FindUser = UserRegistry.Find(x => x.Username == user.Username);
                    return Ok(JsonSerializer.Serialize(await usersDB.GetUserFromID(FindUser.Id.ToString())));
                }
            }
            return BadRequest();
        }

        [HttpPut("{ID}")]
        public async Task<IActionResult> UpdateUser([FromRoute]string id, [FromBody] JsonElement Juser)
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
    }
}
