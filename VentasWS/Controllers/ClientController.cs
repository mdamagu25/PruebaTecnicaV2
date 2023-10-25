using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.UI;
using VentasWS.Models;

namespace VentasWS.Controllers
{
    public class ClientController : ApiController
    {
        public VentasServerLogicalData db = new VentasServerLogicalData();

        public IHttpActionResult Get(string user, string pass)
        {
            Cliente cliente = db.Clientes.FirstOrDefault(c => c.Usuario == user && c.Contrasena == pass);
            
            return Ok(cliente);

        }

        public IHttpActionResult Post(Cliente cliente)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return NotFound();
                }
                Cliente nuevoCliente = new Cliente
                {
                    Nombre_Cliente = cliente.Nombre_Cliente,
                    Usuario = cliente.Usuario,
                    Contrasena = cliente.Contrasena
                };
                db.Clientes.Add(nuevoCliente);
                db.SaveChanges();

                return Ok(cliente);
            }catch (Exception ex) {
                return BadRequest(ex.Message);
            }

        }
    }
}
