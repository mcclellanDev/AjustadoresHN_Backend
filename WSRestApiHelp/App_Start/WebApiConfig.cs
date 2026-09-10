using System;
using System.Configuration;
using System.Web.Http;
using System.Web.Http.Cors;
using WSRestApiHelp.Clases;

namespace WSRestApiHelp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Orígenes desde Web.config (Cors_AllowedOrigins). Evitar "*" en producción.
            string origins = ConfigurationManager.AppSettings["Cors_AllowedOrigins"];
            if (string.IsNullOrWhiteSpace(origins))
            {
                origins = "https://testportal.porsalud.net,https://portal.porsalud.net";
            }

            var corsAttr = new EnableCorsAttribute(
                origins: origins.Trim(),
                headers: "*",
                methods: "*");
            config.EnableCors(corsAttr);

            config.MapHttpAttributeRoutes();

            // Orden: rate limit primero, luego JWT.
            config.MessageHandlers.Add(new RateLimitHandler());
            config.MessageHandlers.Add(new TokenValidationHandler());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
