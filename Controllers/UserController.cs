﻿using Microsoft.AspNetCore.Http;
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
        const string SessionUsername = "_Username";
        private readonly IHostingEnvironment hostingEnvironment;

        public UserController(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
            api = new Starter.Starter();
            Client = api.Start();
        }

        Starter.Starter api;
        HttpClient Client;

        // GET: UserController
        public ActionResult LogOut()
        {
            HttpContext.Session.Remove(SessionID);
            HttpContext.Session.Remove(SessionUsername);
            return View("/Views/Login/_Login.cshtml", new Login());
        }

        // GET: UserController/Details/5
        public ActionResult ChatRoom(string id)
        {
            List<Message> mensajesMostrados = new List<Message>();
            return View("/Views/Chat/Room.cshtml", mensajesMostrados);
        }

        // GET: UserController/Create
        public async Task<ActionResult> Contacts()
        {
            var response = await Client.GetAsync("api/user/" + HttpContext.Session.GetString(SessionID));
            var user = response.Content.ReadAsStringAsync().Result;
            User currentUser = JsonSerializer.Deserialize<User>(user);
            return View("/Views/Chat/Contactos.cshtml", currentUser);
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
            var credencials = new Login();
            credencials.Username = collection["Username"];
            credencials.Password = collection["Password"];
            var json = JsonSerializer.Serialize(credencials);
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            HttpResponseMessage RM = await Client.PostAsync("api/user/login", content);
            //Si encuentra 
            if (RM.IsSuccessStatusCode)
            {
                var request = RM.Content.ReadAsStringAsync().Result;
                var Id = JsonSerializer.Deserialize<string>(request);
                HttpContext.Session.SetString(SessionID, Id);
                HttpContext.Session.SetString(SessionUsername, credencials.Username);
                return RedirectToAction(nameof(ListChat));
            }
            else
            {
                TempData["testmsg"] = "Nombre de usuario o contrasena incorrectos.";
                return View("/Views/Login/_Login.cshtml", new Login());
            }
        }

        [HttpGet]
        public IActionResult _Register()
        {
            //Muestra la vista de registro
            return View("/Views/Login/_Registro.cshtml", new Register());
        }

        [HttpPost]
        public async Task<IActionResult> _Register(IFormCollection collection)
        {
            //Si las credenciales son correctas y no existe el usuario crea el usuario
            if (collection["Password"] == collection["PasswordConfirm"])
            {
                var credencials = new Login();
                credencials.Username = collection["Username"];
                credencials.Password = collection["Password"];
                var json = JsonSerializer.Serialize(credencials);
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage RM = await Client.PostAsync("api/user/", content);
                if (RM.IsSuccessStatusCode)
                {
                    var request = RM.Content.ReadAsStringAsync().Result;
                    var Id = JsonSerializer.Deserialize<string>(request);
                    HttpContext.Session.SetString(SessionID, Id);
                    return RedirectToAction(nameof(ListChat));
                }
                else
                {
                    TempData["testmsg"] = "El nombre de usuario ya ha sido utilizado, por favor escoja otro.";
                    return View("/Views/Login/_Registro.cshtml", new Register());
                }
            }
            else
            {
                TempData["testmsg"] = "Las contrasenas no coinciden.";
                return View("/Views/Login/_Registro.cshtml", new Register());
            }
        }


        // GET: UserController/Edit/5
        public async Task<ActionResult> ListChat()
        {
            HttpResponseMessage RM = await Client.GetAsync("api/user/" + HttpContext.Session.GetString(SessionID));
            User currentuser = JsonSerializer.Deserialize<User>(RM.Content.ReadAsStringAsync().Result);
            ViewBag.Username = currentuser.Username;
            return View("/Views/Chat/ListaChats.cshtml", currentuser.Chats);
        }

        // GET: UserController/Delete/5
        public ActionResult newContact()
        {
            return View("/Views/Chat/newContact.cshtml");
        }

        // POST: UserController/Delete/5
        [HttpPost]
        public async Task<ActionResult> newContact(IFormCollection collection)
        {
            HttpResponseMessage RM = await Client.GetAsync("api/user/" + HttpContext.Session.GetString(SessionID));

            HttpResponseMessage RM2 = await Client.GetAsync("api/user/getByUser/" + collection["Username"]);

            if (RM.IsSuccessStatusCode && RM2.IsSuccessStatusCode)
            {
                User currentUser = JsonSerializer.Deserialize<User>(RM.Content.ReadAsStringAsync().Result);
                string ID = JsonSerializer.Deserialize<string>(RM2.Content.ReadAsStringAsync().Result);
                HttpResponseMessage RM3 = await Client.GetAsync("api/user/" + ID);
                User User = JsonSerializer.Deserialize<User>(RM3.Content.ReadAsStringAsync().Result);
                if (currentUser.Contacts.Find(x => x == User.Username) == null)
                {
                    if (User.ConnectionRequests.Find(x => x == HttpContext.Session.GetString(SessionID)) == null)
                    {
                        if (currentUser.ConnectionRequests.Find(x => x == User.Username) == null)
                        {
                            await Client.PutAsync("api/user/addContact/" + HttpContext.Session.GetString(SessionID) + "/" + ID, null);
                            
                            return RedirectToAction(nameof(Contacts));
                            
                        }
                        else
                        {
                            TempData["testmsg"] = "Tiene una solicitud pendiente de este usuario";
                            return View("/Views/Chat/newContact.cshtml");
                        }
                    }
                    else
                    {
                        TempData["testmsg"] = "Ya tiene una solicitud pendiente";
                        return View("/Views/Chat/newContact.cshtml");
                    }
                }
                else
                {
                    TempData["testmsg"] = "Este usuario ya esta en sus contactos";
                    return View("/Views/Chat/newContact.cshtml");
                }
            }
            else
            {
                TempData["testmsg"] = "No se encontro el usuario";
                return View("/Views/Chat/newContact.cshtml");
            }
        }

        public async Task<IActionResult> AccceptRequest(string ID)
        {
            if(Client.PutAsync("api/user/accept/" + HttpContext.Session.GetString(SessionID) + "/" + ID, null).IsCompletedSuccessfully)
            {
                return View("/Views/Chat/newContact.cshtml");
            }
            else
            {
                TempData["testmsg"] = "No se pudo aceptar la solicitud";
                return View("/Views/Chat/newContact.cshtml");
            }
        }
        public async Task<IActionResult> DenegateRequest(string ID)
        {
            if (Client.PutAsync("api/user/reject/" + HttpContext.Session.GetString(SessionID) + "/" + ID, null).IsCompletedSuccessfully)
            {
                return View("/Views/Chat/newContact.cshtml");
            }
            else
            {
                TempData["testmsg"] = "No se pudo denegar la solicitud";
                return View("/Views/Chat/newContact.cshtml");
            }
        }
    }
}
