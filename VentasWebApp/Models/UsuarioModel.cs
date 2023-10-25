using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VentasWebApp.Models
{
    public class UsuarioModel
    {
        public int Id_Cliente { get; set; }
        public string Nombre_Cliente { get; set; }
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
    }
}