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

        public IActionResult Room(int room)
        {
            return View("Room", room);
        }
    }
}
