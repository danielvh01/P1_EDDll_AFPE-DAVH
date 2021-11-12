using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataStructures;

namespace P1_EDDll_AFPE_DAVH.Controllers
{
    public class ChatController : Controller
    {
        const string SessionID = "_UID";


        public static Dictionary<int, string> Rooms = new Dictionary<int, string>()
        {
            {1,"Ingenieria"},
            {2,"Derecho"},
            {3,"Medicina"}
        };

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            //Muestra la vista de inicio de sesión
            return View();
        }

        [HttpPost]
        public IActionResult Login(IFormCollection collection)
        {
            //Si las credenciales son correctas iniciará sesión
            //Si encuentra 
            if(true)
            {
                //HttpContext.Session.SetString(SessionID, GetUser(collection["Username"], collection["Password"]));
                HttpContext.Session.SetString(SessionID, collection["Username"]);
            }
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            //Muestra la vista de registro
            return View();
        }

        [HttpPost]
        public IActionResult Register(IFormCollection collection)
        {
            //Si las credenciales son correctas y no existe el usuario crea el usuario
            return View();
        }

        public IActionResult Room(int room)
        {
            return View("Room", room);
        }
    }
}
