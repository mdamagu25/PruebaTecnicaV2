using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VentasWebApp.Models;

namespace VentasWebApp.Controllers
{
    public class HomeController : Controller
    {
        ConfiguracionServerModel configuracionServerModel = new ConfiguracionServerModel();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult ArticulosVenta()
        {
            return View();
        }

        public ActionResult Factura()
        {
            return View();
        }

        #region GuardarArticulosVenta
        /// <summary>
        /// Requerimiento manejo de ventas
        /// Autor: Daniela Martinez
        /// Guarda los articulos de venta en bd.
        /// </summary>
        /// 
        [HttpPost]
        public async Task<ActionResult> ArticulosVenta(ArticulosModel model)
        {
            var url = $"{configuracionServerModel.WebServicesHostPublish}api/Articulos";

            var articulos = new ArticulosModel
            {
                Codigo_Articulo = model.Codigo_Articulo,
                Nombre_Articulo = model.Nombre_Articulo,
                Precio_Articulo = model.Precio_Articulo,
                ValidateIVA = model.ValidateIVA,
            };

            using (var client = new HttpClient())
            {
                var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(articulos);

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return View();
                }
                else
                {
                    return Json(new { success = false, numeroError = -1, message = "El producto con el cógido digitado ya ha sido registrado" });
                }
            }
            
        }
        #endregion

        #region ListadeArticulos
        /// <summary>
        /// Requerimiento manejo de ventas
        /// Autor: Daniela Martinez
        /// Genera un json apartir de los articulos que existen en bd.
        /// </summary>

        [HttpGet]
        public JsonResult ListaArt()
        {
            string url = $"{configuracionServerModel.WebServicesHostPublish}api/Articulos";
            IEnumerable<ArticulosModel> data = null;
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    data = JsonConvert.DeserializeObject<IEnumerable<ArticulosModel>>(responseBody);
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region EliminarArticulo
        /// <summary>
        /// Requerimiento manejo de ventas
        /// Autor: Daniela Martinez
        /// Elimina un articulo de la bd por medio del Codigo_Articulo
        /// </summary>

        [HttpPost]
        public ActionResult EliminarArticulo(int Codigo_Articulo)
        {
            string url = $"{configuracionServerModel.WebServicesHostPublish}api/Articulos/{Codigo_Articulo}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.DeleteAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Artículo eliminado con éxito" });
                }
            }
            return Json(new { success = false, message = "El artículo no fue eliminado" });
        }
        #endregion
        
        #region AgregarArticulosaFactura
        /// <summary>
        /// Requerimiento manejo de ventas
        /// Autor: Daniela Martinez
        /// Consulta los articulos y retorna un json para llenar la tabla de factura.
        /// </summary>
        /// 
        [HttpPost]
        public ActionResult Factura(int Codigo_Articulo, int Cantidad)
        {
            string url = $"{configuracionServerModel.WebServicesHostPublish}api/Articulos?codigo={Codigo_Articulo}&cantidad={Cantidad}";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<ArticulosModel>(responseBody);
                    return Json(new { numeroError = 1, data },JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { numeroError = -1, message = "El código del artículo no existe." }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GuardarFactura

        /// <summary>
        /// Requerimiento manejo de ventas
        /// Autor: Daniela Martinez
        /// Guarda el encabezado de la factura y valida si se realizó correctamente y luego guarda el detalle en bd.
        /// </summary>
        /// 
        [HttpPost]
        public async Task<ActionResult> GuardarFactura(List<FacturaModel> datosTabla, int id)
        {
            var url = $"{configuracionServerModel.WebServicesHostPublish}api/Facturas";

            var factura = new FacturaModel
            {
                Id_Cliente = id,
                Fecha = DateTime.Now,
            };
            using (var client = new HttpClient())
            {
                var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(factura);

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    url = $"{configuracionServerModel.WebServicesHostPublish}api/Detalles";

                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    var data = JsonConvert.DeserializeObject<DetalleModel>(responseBody);
                    var detalle = new List<DetalleModel>();

                    foreach (var item in datosTabla)
                    {
                        var detalleModel = new DetalleModel
                        {
                            Id_Factura = data.Id_Factura,
                            Codigo_Articulo = item.Codigo_Articulo,
                            Cantidad = item.Cantidad,
                            Total = item.Total,

                        };
                        detalle.Add(detalleModel);
                    }

                    using (var cliente = new HttpClient())
                    {
                        jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(detalle);

                        content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                        HttpResponseMessage respuesta = await cliente.PostAsync(url, content);

                        if (respuesta.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            return Json(new { success = true, message = "Factura creada con éxito" });
                        }
                    }
                }
            }
            return Json(new { error = true, message = "La factura no se pudo guardar." });
        }
        #endregion

        #region ObtenerArticulo
        /// <summary>
        /// Requerimiento manejo de ventas
        /// Autor: Daniela Martinez
        /// Elimina un articulo de la bd por medio del Codigo_Articulo
        /// </summary>

        [HttpPost]
        public ActionResult ObtenerArticulo(int Codigo_Articulo)
        {
            string url = $"{configuracionServerModel.WebServicesHostPublish}api/Articulos/GetArticulo?codigo={Codigo_Articulo}";
            ArticulosModel data = null;
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    data = JsonConvert.DeserializeObject<ArticulosModel>(responseBody);
                }
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ActualizarArticulo
        /// <summary>
        /// Requerimiento manejo de ventas
        /// Autor: Daniela Martinez
        /// Actualizar un articulo de la bd por medio del Codigo_Articulo
        /// </summary>
        /// <param name="Codigo_Articulo"></param>

        [HttpPost]
        public async Task<ActionResult> ActualizarArticulo(ArticulosModel model)
        {
            var url = $"{configuracionServerModel.WebServicesHostPublish}api/Articulos/PutArticulo";

            var articulos = new ArticulosModel
            {
                Codigo_Articulo = model.Codigo_Articulo,
                Nombre_Articulo = model.Nombre_Articulo,
                Precio_Articulo = model.Precio_Articulo,
                ValidateIVA = model.ValidateIVA,
            };

            using (var client = new HttpClient())
            {
                var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(articulos);

                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
            }

            return View();
        }
        #endregion
    }
}