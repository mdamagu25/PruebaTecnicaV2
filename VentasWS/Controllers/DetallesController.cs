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
    public class DetallesController : ApiController
    {
        public VentasServerLogicalData db = new VentasServerLogicalData();

        public IHttpActionResult PostDetalle(List<Detalle> detalles)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Detalles.AddRange(detalles);
            db.SaveChanges();

            return Ok();
        }
    }
}