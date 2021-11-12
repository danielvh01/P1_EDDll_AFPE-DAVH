using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_DataTransfer.Data;
using System.Text.Json;
using API_DataTransfer.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_DataTransfer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        User_Collection usersDB = new User_Collection();
        
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(JsonSerializer.Serialize(await usersDB.GetUsersList()));
        }

        // GET api/<UserController>/5
        [HttpGet("{username}")]
        public async Task<IActionResult> GetSpecifiedUser(string user)
        {
            List<User> AllUsers = usersDB.GetUsersList().Result.ToList();
            var FindUser = AllUsers.Find(x => x.Username == user);
            return Ok(JsonSerializer.Serialize(await usersDB.GetUserFromID(FindUser.Id.ToString())));
        }

        // POST api/<UserController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
