using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VentasWebApp.Models
{
    public class FacturaModel
    {
        public int Codigo_Articulo { get; set; }
        public string Nombre_Articulo { get; set; }
        public decimal Precio_Articulo {  get; set; }
        public decimal IVA {  get; set; }
        public decimal Cantidad { get; set; }
        public decimal Total { get; set; }
        public int Id_Cliente { get; set; }
        public System.DateTime Fecha { get; set; }
    }
}