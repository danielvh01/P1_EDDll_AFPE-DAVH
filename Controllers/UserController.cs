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


namespace P1_EDDll_AFPE_DAVH.Controllers
{
    public class UserController : Controller
    {
        const string SessionID = "_UID";

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
        public IActionResult Login()
        {
            //Muestra la vista de inicio de sesión
            return View("/Views/Login/Login.cshtml", new Login());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(IFormCollection collection) 
        { 
            //Si las credenciales son correctas iniciará sesión
            var user = new API_DataTransfer.Models.User();
            Starter.Starter api = new Starter.Starter();
            Client = api.Start();
            HttpResponseMessage RM = await Client.GetAsync("api/user/" + collection["Username"]);
            //Si encuentra 
            if (RM.IsSuccessStatusCode)
            {
                var request = RM.Content.ReadAsStringAsync().Result;
                User currentUser = JsonSerializer.Deserialize<User>(request);
                HttpContext.Session.SetString(SessionID, collection["Username"]);
            }
            return View();
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

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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
