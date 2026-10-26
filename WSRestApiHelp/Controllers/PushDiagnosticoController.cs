using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using WSRestApiHelp.Clases;
using WSRestApiHelp.Models;

namespace WSRestApiHelp.Controllers
{
    [Authorize]
    [RoutePrefix("api/Proveedor")]
    public class PushDiagnosticoController : ApiController
    {
        /// <summary>
        /// Envia una push de prueba al dispositivo del usuario autenticado.
        /// No requiere cuerpo ni IdProveedorAgente. Aceptada no significa entregada.
        /// </summary>
        [HttpPost]
        [Route("ProbarNotificacionPush")]
        public IHttpActionResult ProbarNotificacionPush()
        {
            if (User == null || User.Identity == null || !User.Identity.IsAuthenticated ||
                String.IsNullOrWhiteSpace(User.Identity.Name))
                return Unauthorized();

            var diagnosticoId = Guid.NewGuid().ToString();
            try
            {
                using (var db = new helpEntities())
                {
                    // Misma resolucion de identidad que Login/Autenticacion, proyectando solo Id.
                    var ids = db.help_F_Api_ObtenerPassword(User.Identity.Name)
                        .Select(x => x.Id).Distinct().Take(2).ToList();
                    if (ids.Count != 1)
                        return Content(HttpStatusCode.Forbidden, new { Codigo = "USUARIO_NO_IDENTIFICADO" });

                    var agente = db.help_ProveedorAgentes.Find(ids[0]);
                    if (agente == null)
                        return Content(HttpStatusCode.Forbidden, new { Codigo = "USUARIO_NO_IDENTIFICADO" });
                    if (String.IsNullOrWhiteSpace(agente.TokenPush))
                        return Content(HttpStatusCode.Conflict, new {
                            Codigo = "SIN_DISPOSITIVO", Mensaje = "Abra la app e inicie sesion para registrar su dispositivo."
                        });

                    var servicio = new OneSignalPushService();
                    var clave = ConfigurationManager.AppSettings["OneSignalRestApiKey"];
                    Guid appId;
                    if (!servicio.IsConfigured ||
                        !Guid.TryParse(ConfigurationManager.AppSettings["OneSignalAppId"], out appId) ||
                        clave.IndexOf("REPLACE_WITH", StringComparison.OrdinalIgnoreCase) >= 0)
                        return Content(HttpStatusCode.ServiceUnavailable, new {
                            Codigo = "PUSH_NO_CONFIGURADO", Mensaje = "Revise la configuracion de OneSignal en el servidor."
                        });

                    // Limite por usuario y proceso IIS; no es un limitador distribuido.
                    var existente = HttpRuntime.Cache.Add("PushDiagnostico:" + agente.Id, true, null,
                        DateTime.UtcNow.AddMinutes(1), Cache.NoSlidingExpiration,
                        CacheItemPriority.Normal, null);
                    if (existente != null)
                        return Content((HttpStatusCode)429, new {
                            Codigo = "ESPERE_UN_MINUTO", Mensaje = "Espere un minuto entre pruebas."
                        });

                    var resultado = servicio.SendToPlayer(agente.TokenPush,
                        "Prueba push - Ajustadores HN",
                        "Notificacion de prueba enviada desde la API de Ajustadores HN.",
                        new Dictionary<string, object> {
                            { "event", "prueba_push" }, { "Type", "PruebaPush" },
                            { "diagnosticoId", diagnosticoId }
                        });

                    string notificacionId = null;
                    var aceptada = resultado.Success && TryGetNotificationId(resultado.Body, out notificacionId);

                    Trace.TraceInformation("Push prueba {0}: agente={1}, HTTP={2}, aceptada={3}",
                        diagnosticoId, agente.Id, resultado.StatusCode, aceptada);

                    if (!aceptada)
                        return Content(HttpStatusCode.BadGateway, new {
                            Codigo = resultado.StatusCode == 401 || resultado.StatusCode == 403
                                ? "ONESIGNAL_AUTORIZACION_RECHAZADA" : "ONESIGNAL_NO_ACEPTO_NOTIFICACION",
                            Mensaje = "OneSignal no confirmo la creacion. Revise la clave y la suscripcion del dispositivo.",
                            OneSignalHttpStatus = resultado.StatusCode,
                            DiagnosticoId = diagnosticoId
                        });

                    return Ok(new {
                        AceptadaPorOneSignal = true,
                        EntregaConfirmada = false,
                        IdProveedorAgente = agente.Id,
                        NotificacionId = notificacionId,
                        DiagnosticoId = diagnosticoId,
                        Mensaje = "OneSignal acepto la prueba. Confirme su recepcion en el telefono."
                    });
                }
            }
            catch (Exception ex)
            {
                // No devolver ni registrar tokens, claves o respuestas crudas del proveedor.
                Trace.TraceError("Push prueba {0}: error de tipo {1}", diagnosticoId, ex.GetType().Name);
                return Content(HttpStatusCode.InternalServerError, new {
                    Codigo = "ERROR_PRUEBA_PUSH", DiagnosticoId = diagnosticoId,
                    Mensaje = "No se pudo completar la prueba. Revise el registro del servidor."
                });
            }
        }

        internal static bool TryGetNotificationId(string body, out string notificationId)
        {
            notificationId = null;
            if (String.IsNullOrWhiteSpace(body)) return false;
            try
            {
                var json = JObject.Parse(body);
                var id = json["id"];
                Guid parsed;
                if (id == null || id.Type != JTokenType.String ||
                    !Guid.TryParse((string)id, out parsed) || parsed == Guid.Empty) return false;
                notificationId = parsed.ToString();
                return true;
            }
            catch (JsonException) { return false; }
        }
    }
}
