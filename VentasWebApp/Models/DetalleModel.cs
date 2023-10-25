using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VentasWebApp.Models
{
    public class DetalleModel
    {
        public int Id_Detalle { get; set; }
        public int Id_Factura { get; set; }
        public int Codigo_Articulo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Total { get; set; }
    }
}