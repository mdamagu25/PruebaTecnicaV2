using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using VentasWS.Models;

namespace VentasWS.Controllers
{
    public class ArticulosController : ApiController
    {
        public VentasServerLogicalData db = new VentasServerLogicalData();

        public bool AgregarArticulo(Articulo articulo)
        {
            decimal IVA = 0;
            if (!ModelState.IsValid)
            {
                return false;
            }
            else
            {
                decimal vTotal;
                if (articulo.ValidateIVA)
                {
                    IVA = articulo.Precio_Articulo * 0.13m;
                    vTotal = articulo.Precio_Articulo + IVA;
                }
                else
                {
                    vTotal = articulo.Precio_Articulo;
                }

                Articulo nuevoArticulo = new Articulo
                {
                    Codigo_Articulo = articulo.Codigo_Articulo,
                    Nombre_Articulo = articulo.Nombre_Articulo,
                    Precio_Articulo = articulo.Precio_Articulo,
                    IVA = IVA,
                    Total = vTotal
                };
                db.Articulos.Add(nuevoArticulo);
                db.SaveChanges();
                return true;
            }
        }

        public IQueryable<Articulo> GetArticulos()
        {
            return db.Articulos;
        }

        public IHttpActionResult GetArticulo(int codigo)
        {
            Articulo articulo = db.Articulos.Find(codigo);
            if (articulo == null)
            {
                return NotFound();
            }

            return Ok(articulo);
        }
        public IHttpActionResult DeleteArticulo(int id)
        {
            try
            {
                Articulo articulo = db.Articulos.Find(id);
                if (articulo == null)
                {
                    return NotFound();
                }

                db.Articulos.Remove(articulo);
                db.SaveChanges();
                return Ok();
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        public IHttpActionResult GetArticulo(int codigo, int cantidad)
        {
            Articulo articulo = db.Articulos.Find(codigo);
            if (articulo == null)
            {
                return NotFound();
            }

            decimal precio = articulo.Precio_Articulo;
            decimal iva = articulo.IVA;
            decimal total = ((precio + iva) * cantidad);

            Articulo articuloConTotal = new Articulo
            {
                Codigo_Articulo = articulo.Codigo_Articulo,
                Nombre_Articulo = articulo.Nombre_Articulo,
                Precio_Articulo = articulo.Precio_Articulo,
                IVA = articulo.IVA,
                Total = total
            };

            return Ok(articuloConTotal);
        }

        public IHttpActionResult PutArticulo(Articulo articulo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Entry(articulo).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticuloExists(articulo.Codigo_Articulo))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }
        private bool ArticuloExists(int id)
        {
            return db.Articulos.Count(e => e.Codigo_Articulo == id) > 0;
        }
    }
}
