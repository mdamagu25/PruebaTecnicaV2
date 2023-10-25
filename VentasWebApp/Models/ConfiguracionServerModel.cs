using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace VentasWebApp.Models
{
    public class ConfiguracionServerModel
    {
        public string WebServicesHostPublish = ConfigurationManager.AppSettings["WebServicesHostPublish"];
    }
}