using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using VentasWS.Models;

namespace VentasWS.Controllers
{
    public class FacturasController : ApiController
    {
        public VentasServerLogicalData db = new VentasServerLogicalData();

        public IHttpActionResult PostFactura(Factura factura)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Facturas.Add(factura);
            db.SaveChanges();

            return Ok(factura);
        }
    }
}