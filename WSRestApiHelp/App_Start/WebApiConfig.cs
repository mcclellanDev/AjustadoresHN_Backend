using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using WSRestApiHelp.Clases;

namespace WSRestApiHelp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuración y servicios de API web
            //var corsAttr = new EnableCorsAttribute("*", "*", "*");
            EnableCorsAttribute corsAttr = new EnableCorsAttribute(
                origins: "*",     // Reemplaza con los orígenes permitidos
                headers: "*",     // Reemplaza con los encabezados permitidos
                methods: "*"      // Reemplaza con los métodos permitidos
            );
            config.EnableCors(corsAttr);
            // Rutas de API web
            config.MapHttpAttributeRoutes();

            config.MessageHandlers.Add(new TokenValidationHandler());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
