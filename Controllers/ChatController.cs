using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P1_EDDll_AFPE_DAVH.Controllers
{
    public class ChatController : Controller
    {

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
