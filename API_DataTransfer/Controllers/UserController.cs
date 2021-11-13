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
        public async Task<IActionResult> GetSpecifiedUser(string user)
        {
            List<User> AllUsers = usersDB.GetAllUsers().Result.ToList();
            var FindUser = AllUsers.Find(x => x.Username == user);            
            return Ok(JsonSerializer.Serialize(await usersDB.GetUserFromID(FindUser.Id.ToString())));
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(JsonElement Juser)
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

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] JsonElement Juser)
        {
            User _user = JsonSerializer.Deserialize<User>(Juser.ToString());
            if (_user == null)
            {
                return BadRequest();
            }
            await usersDB.PutUser(_user);
            return Ok();
        }
    }
}
