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


namespace P1_EDDll_AFPE_DAVH.Controllers
{
    public class UserController : Controller
    {
        const string SessionID = "_UID";

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
            return View("/Views/Login/Login.cshtml");
        }

        [HttpPost]
        public IActionResult Login(IFormCollection collection)
        {
            //Si las credenciales son correctas iniciará sesión
            //Si encuentra 
            if (true)
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
