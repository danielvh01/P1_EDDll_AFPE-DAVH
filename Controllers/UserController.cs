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
using P1_EDDll_AFPE_DAVH.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using DataStructures;
using System.IO;
using System.Numerics;
using MongoDB.Bson;

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
        }

        public static Starter.Starter api;
        public HttpClient Client;

        // GET: UserController
        public ActionResult LogOut()
        {
            HttpContext.Session.Remove(SessionID);
            HttpContext.Session.Remove(SessionUsername);
            return View("/Views/Login/_Login.cshtml", new Login());
        }

        // GET: UserController/Details/5
        public async Task<ActionResult> ChatRoom(string id)
        {
            api = new Starter.Starter();
            Client = api.Start();
            var RM2 = await Client.GetAsync("api/user/chat/" + id);
            ViewBag.ID = id;
            ChatRoom chatRoom = JsonSerializer.Deserialize<ChatRoom>(RM2.Content.ReadAsStringAsync().Result);
            
            var response = await Client.GetAsync("api/user/" + HttpContext.Session.GetString(SessionID));
            var user = response.Content.ReadAsStringAsync().Result;
            User currentUser = JsonSerializer.Deserialize<User>(user);
            ViewBag.ka = currentUser.a;
            ViewBag.p = chatRoom.p;
            ViewBag.A = chatRoom.A;
            ViewBag.B = chatRoom.B;
            ViewBag.Username = HttpContext.Session.GetString(SessionUsername);
            if (chatRoom.type == 1)
            {
                if(chatRoom.Users[0] == HttpContext.Session.GetString(SessionID))
                {                    
                    ViewBag.ChatName = await Client.GetAsync("api/user/getUsername/" + chatRoom.Users[1]).Result.Content.ReadAsStringAsync();
                }
                else
                {                    
                    ViewBag.ChatName = await Client.GetAsync("api/user/getUsername/" + chatRoom.Users[0]).Result.Content.ReadAsStringAsync();
                }
                
            }
            else
            {
                ViewBag.ChatName = chatRoom.name;
            }
            List<Message> mensajesMostrados = chatRoom.Messages;
            return View("/Views/Chat/Room.cshtml", mensajesMostrados);
        }

        // GET: UserController/Create
        public async Task<ActionResult> Contacts()
        {
            api = new Starter.Starter();
            Client = api.Start();
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
            api = new Starter.Starter();
            Client = api.Start();
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
                var Id = RM.Content.ReadAsStringAsync().Result;                
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
                api = new Starter.Starter();
                Client = api.Start();
                var credencials = new Login();
                credencials.Username = collection["Username"];
                credencials.Password = collection["Password"];
                var json = JsonSerializer.Serialize(credencials);
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage RM = await Client.PostAsync("api/user/", content);
                if (RM.IsSuccessStatusCode)
                {
                    var Id = RM.Content.ReadAsStringAsync().Result;                    
                    HttpContext.Session.SetString(SessionID, Id);
                    HttpContext.Session.SetString(SessionUsername, credencials.Username);
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

        [HttpPost]
        public async Task<IActionResult> SendMessage(IFormCollection collection)
        {
            api = new Starter.Starter();
            Client = api.Start();
            string sessionUser = HttpContext.Session.GetString(SessionID);
            var ID = collection["Id"];
            var message = collection["MessageText"];
            HttpResponseMessage RM = await Client.GetAsync("api/user/chat/" + ID);

            HttpResponseMessage RM2 = await Client.GetAsync("api/user/" + sessionUser);
            User currentuser = JsonSerializer.Deserialize<User>(RM2.Content.ReadAsStringAsync().Result);

            var request = RM.Content.ReadAsStringAsync().Result;
            var _chat = JsonSerializer.Deserialize<ChatRoom>(request);
            
            if (_chat.type == 1)
            {
                ICipher<int> cipher = new SDES(Path.GetDirectoryName(@"Configuration\"));
                Message mensaje;

                //Como llave sdes mandar la que se genera de diffie helman entre los usuarios
                if (_chat.Users.FindIndex(x => x == sessionUser) == 0)
                {
                    mensaje = new Message();
                    mensaje.Id = ObjectId.GenerateNewId();
                    mensaje.content = cipher.Cipher(GetBytes(message), (int)BigInteger.ModPow(_chat.B, currentuser.a, _chat.p)).ToList();
                    mensaje.UserSender = HttpContext.Session.GetString(SessionUsername);
                }
                else
                {
                    mensaje = new Message();
                    mensaje.Id = ObjectId.GenerateNewId();
                    mensaje.content = cipher.Cipher(GetBytes(message), (int)BigInteger.ModPow(_chat.A, currentuser.a, _chat.p)).ToList();
                    mensaje.UserSender = HttpContext.Session.GetString(SessionUsername);
                }
                await Client.PutAsync("api/user/chat/sendMessage/" + ID, new StringContent(JsonSerializer.Serialize(mensaje).ToString(), Encoding.UTF8, "application/json"));
            }
            else
            {                
                RSA groupCipher = new RSA();
                int[] keys = { currentuser.n, currentuser.e };
                Message mensaje = new Message();
                mensaje.Id = ObjectId.GenerateNewId();
                mensaje.content = groupCipher.Cipher(GetBytes(message), keys).ToList();
                mensaje.UserSender = HttpContext.Session.GetString(SessionUsername);
                mensaje.k1 = currentuser.n;
                mensaje.k2 = currentuser.d;

            }
            var json = JsonSerializer.Serialize(_chat);
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            await Client.PostAsync("api/user/chat/" + ID , content);
            return RedirectToAction(nameof(ChatRoom),ID);
        }


        // GET: UserController/Edit/5
        public async Task<ActionResult> ListChat()
        {
            api = new Starter.Starter();
            Client = api.Start();
            HttpResponseMessage RM = await Client.GetAsync("api/user/" + HttpContext.Session.GetString(SessionID));
            User currentuser = JsonSerializer.Deserialize<User>(RM.Content.ReadAsStringAsync().Result);
            ViewBag.Username = currentuser.Username;
            ViewBag.Id = HttpContext.Session.GetString(SessionID);
            List<ChatRoom> chats = new List<ChatRoom>();
            foreach(var c in currentuser.Chats)
            {
                var RM2 = await Client.GetAsync("api/user/chat/" + c);
                var temp = JsonSerializer.Deserialize<ChatRoom>(RM2.Content.ReadAsStringAsync().Result);
                temp.Id = new ObjectId(c);
                chats.Add(temp);
            }
            return View("/Views/Chat/ListaChats.cshtml", chats);
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
            api = new Starter.Starter();
            Client = api.Start();
            HttpResponseMessage RM = await Client.GetAsync("api/user/" + HttpContext.Session.GetString(SessionID));

            HttpResponseMessage RM2 = await Client.GetAsync("api/user/getByUser/" + collection["Username"]);

            if (RM.IsSuccessStatusCode && RM2.IsSuccessStatusCode)
            {
                User currentUser = JsonSerializer.Deserialize<User>(RM.Content.ReadAsStringAsync().Result);
                string ID = RM2.Content.ReadAsStringAsync().Result;
                HttpResponseMessage RM3 = await Client.GetAsync("api/user/" + ID);
                User User = JsonSerializer.Deserialize<User>(RM3.Content.ReadAsStringAsync().Result);
                if (currentUser.Contacts.Find(x => x.ID == ID) == null)
                {
                    if (User.ConnectionRequests.Find(x => x.ID == HttpContext.Session.GetString(SessionID)) == null)
                    {
                        if (currentUser.ConnectionRequests.Find(x => x.ID == ID) == null)
                        {
                            ContactRequest contact = new ContactRequest();
                            contact.IDSender = HttpContext.Session.GetString(SessionID);
                            contact.UsernameSender = HttpContext.Session.GetString(SessionUsername);
                            contact.IDReceiver = ID;
                            await Client.PutAsync("api/user/addingContact", new StringContent(JsonSerializer.Serialize(contact).ToString(), Encoding.UTF8, "application/json"));
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
                        TempData["testmsg"] = "El usuario ya tiene una solicitud pendiente suya";
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

        public async Task<IActionResult> AccceptRequest(Contact contact)
        {
            api = new Starter.Starter();
            Client = api.Start();
            ContactRequest request = new ContactRequest();
            request.IDReceiver = HttpContext.Session.GetString(SessionID);
            request.UsernameReceiver = HttpContext.Session.GetString(SessionUsername);

            request.IDSender = contact.ID;
            request.UsernameSender = contact.Username;
            var RM = await Client.PutAsync("api/user/accept", new StringContent(JsonSerializer.Serialize(request).ToString(), Encoding.UTF8, "application/json"));
            if (RM.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Contacts));
            }
            else
            {
                TempData["testmsg"] = "No se pudo aceptar la solicitud";
                return RedirectToAction(nameof(Contacts));
            }
        }
        public async Task<IActionResult> DenegateRequest(Contact contact)
        {
            api = new Starter.Starter();
            Client = api.Start();
            ContactRequest contactR = new ContactRequest();
            contactR.IDSender = contact.ID;
            contactR.UsernameSender = contact.Username;
            contactR.IDReceiver = HttpContext.Session.GetString(SessionID);
            var RM = await Client.PutAsync("api/user/reject", new StringContent(JsonSerializer.Serialize(contactR).ToString(), Encoding.UTF8, "application/json"));
            if (RM.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Contacts));
            }
            else
            {
                TempData["testmsg"] = "No se pudo denegar la solicitud";
                return RedirectToAction(nameof(Contacts));
            }
        }

        public ActionResult ChatType()
        {
            return View("/Views/Chat/ChatType.cshtml");
        }

        public async Task<ActionResult> _ChatType(int type)
        {
            api = new Starter.Starter();
            Client = api.Start();
            var RM = await Client.GetAsync("api/user/" + HttpContext.Session.GetString(SessionID));
            User currentUser = JsonSerializer.Deserialize<User>(RM.Content.ReadAsStringAsync().Result);
            if (type == 1)
            {
                return View("/Views/Chat/CreateChatP.cshtml", currentUser.Contacts);
            }
            else
            {
                return View("/Views/Chat/ChatType.cshtml");
            }
        }

        public async Task<IActionResult> CreateChatP(Contact contact)
        {
            api = new Starter.Starter();
            Client = api.Start();
            var RM = await Client.GetAsync("api/user/" + HttpContext.Session.GetString(SessionID));
            User currentUser = JsonSerializer.Deserialize<User>(RM.Content.ReadAsStringAsync().Result);
            foreach (var chat in currentUser.Chats)
            {
                var RM2 = await Client.GetAsync("api/user/chat/" + chat);
                ChatRoom chatRoom = JsonSerializer.Deserialize<ChatRoom>(RM2.Content.ReadAsStringAsync().Result);
                if (chatRoom.type == 1)
                {
                    if (chatRoom.Users[0] == contact.ID || chatRoom.Users[1] == contact.ID)
                    {
                        TempData["testmsg"] = "Ya existe un chat con este usuario";
                        return View("/Views/Chat/Room.cshtml", chatRoom.Id);
                    }
                }
            }
            
            var RM3 = await Client.GetAsync("api/user/" + contact.ID);
            User user = JsonSerializer.Deserialize<User>(RM3.Content.ReadAsStringAsync().Result);
            ChatRoom newChat = new ChatRoom();
            newChat.Id = ObjectId.GenerateNewId();
            newChat.Users.Add(HttpContext.Session.GetString(SessionID));
            newChat.Users.Add(contact.ID);
            newChat.type = 1;
            newChat.A = (int)BigInteger.ModPow(newChat.g, currentUser.a, newChat.p);
            newChat.B = (int)BigInteger.ModPow(newChat.g, user.a, newChat.p);
            var json = JsonSerializer.Serialize(newChat);
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            HttpResponseMessage RM4 = await Client.PostAsync("api/user/createChat", content);
            if (RM4.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(ChatRoom), new { id = await RM4.Content.ReadAsStringAsync() });
            }
            else
            {
                var message = await RM4.Content.ReadAsStringAsync();
                TempData["testmsg"] = "No se pudo crear el chat" + message;
                return View("/Views/Chat/CreateChatP.cshtml", currentUser.Contacts);
            }
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        bool IsPrime(int number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
                if (number % i == 0)
                    return false;

            return true;
        }

    }
}
