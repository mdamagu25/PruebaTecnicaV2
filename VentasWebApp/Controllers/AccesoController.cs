using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;
using VentasWebApp.Models;

namespace VentasWebApp.Controllers
{
    public class AccesoController : Controller
    {
        ConfiguracionServerModel configuracionServerModel = new ConfiguracionServerModel();
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UsuarioModel model)
        {
            string url = $"{configuracionServerModel.WebServicesHostPublish}api/Client?user={model.Usuario}&pass={model.Contrasena}";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<UsuarioModel>(responseBody);
                    Session["UserId"] = data.Id_Cliente;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Hubo un problema con la validación del usuario.");
                }
            }
            return View();
        }

        public ActionResult Registrar()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Registrar(UsuarioModel model)
        {
            var url = $"{configuracionServerModel.WebServicesHostPublish}api/Client"; 

            var registro = new UsuarioModel
            {
                Nombre_Cliente = model.Nombre_Cliente,
                Usuario = model.Usuario,
                Contrasena = model.Contrasena,
            };

            using (var client = new HttpClient())
            {
                var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(registro);

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return RedirectToAction("Login", "Acceso");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Hubo un problema con la creación del usuario.");
                }
            }
            return View();
        }
    }
}