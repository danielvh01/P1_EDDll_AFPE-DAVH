using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using MongoDB.Driver;
using P1_EDDll_AFPE_DAVH.Starter;
using API_DataTransfer.Data;
using P1_EDDll_AFPE_DAVH.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace P1_EDDll_AFPE_DAVH.Controllers
{
    public class UserController : Controller
    {
        const string SessionID = "_UID";
        private readonly IHostingEnvironment hostingEnvironment;

        public UserController(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }


        HttpClient Client;

        // GET: UserController
        public ActionResult Index()
        {
            return View();
        }

        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public IActionResult _Login()
        {
            //Muestra la vista de inicio de sesión
            return View("/Views/Login/_Login.cshtml", new Login());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> _Login(IFormCollection collection) 
        { 
            //Si las credenciales son correctas iniciará sesión
            var user = new API_DataTransfer.Models.User();
            Starter.Starter api = new Starter.Starter();
            Client = api.Start();
            var credencials = new Login();
            credencials.Username = collection["Username"];
            credencials.Password = collection["Password"];
            var json = JsonSerializer.Serialize(credencials);
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            HttpResponseMessage RM = await Client.PostAsync("api/user/login",content);
            //Si encuentra 
            if (RM.IsSuccessStatusCode)
            {
                var request = RM.Content.ReadAsStringAsync().Result;
                var Id= JsonSerializer.Deserialize<string>(request);
                HttpContext.Session.SetString(SessionID, Id);
                return RedirectToAction(nameof(ListChat));
            }
            else
            {
                TempData["testmsg"] = "Nombre de usuario o contraseña incorrectos.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            //Muestra la vista de registro
            return View("/Views/Login/Register.cshtml", new Register());
        }

        [HttpPost]
        public IActionResult Register(IFormCollection collection)
        {
            //Si las credenciales son correctas y no existe el usuario crea el usuario
            return View();
        }


        // GET: UserController/Edit/5
        public async Task<ActionResult> ListChat()
        {
            HttpResponseMessage RM = await Client.GetAsync("api/user/" + HttpContext.Session.GetString(SessionID));
            if(RM.IsSuccessStatusCode)
            {
                User currentuser = JsonSerializer.Deserialize<User>(RM.Content.ReadAsStringAsync().Result);
                ViewBag.Username = currentuser.Username;
                return View("/Views/Chat/ListaChats.cshtml", currentuser.Chats);
            }
            else
            {

            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
