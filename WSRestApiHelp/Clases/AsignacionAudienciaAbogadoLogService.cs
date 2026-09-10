using System;
using System.Data.SqlClient;
using System.Text.Json;
using System.Web;
using WSRestApiHelp.Models;

namespace WSRestApiHelp.Clases
{
    public static class AsignacionAudienciaAbogadoLogService
    {
        public const string EndpointActualizarAudicion = "ActualizarAudicion";

        public const string EtapaValidacionFecha = "VALIDACION_FECHA";
        public const string EtapaAgendamientoBd = "AGENDAMIENTO_BD";
        public const string EtapaValidacionAtencion = "VALIDACION_ATENCION";
        public const string EtapaNotificacion = "NOTIFICACION";
        public const string EtapaExito = "EXITO";
        public const string EtapaExcepcion = "EXCEPCION";

        public static AsignacionAudienciaAbogadoLogEntry FromParameters(
            int idAjustadorAudiencia,
            int refProveedorAgenteAbogadoId,
            int agendarAudiencia,
            DateTime fechaHora,
            string fecha,
            string hora,
            string lugar,
            int idAgente)
        {
            return new AsignacionAudienciaAbogadoLogEntry
            {
                RefAjustadorAudienciaId = idAjustadorAudiencia,
                RefProveedorAgenteAbogadoId = refProveedorAgenteAbogadoId,
                AgendarAudiencia = agendarAudiencia,
                FechaHora = fechaHora,
                Fecha = fecha,
                Hora = hora,
                Lugar = lugar,
                IdAgente = idAgente
            };
        }

        public static void Registrar(helpEntities db, AsignacionAudienciaAbogadoLogEntry entry)
        {
            if (db == null || entry == null)
            {
                return;
            }

            try
            {
                var nuevoId = new SqlParameter("@NuevoId", System.Data.SqlDbType.Int)
                {
                    Direction = System.Data.ParameterDirection.Output
                };

                db.Database.ExecuteSqlCommand(
                    "EXEC dbo.help_P_Log_AsignacionAudienciaAbogado_Insert " +
                    "@Endpoint, @UsuarioApi, @IpCliente, @UserAgent, " +
                    "@HttpStatusCode, @EsExitoso, @Etapa, @Mensaje, " +
                    "@JsonRequest, @JsonResponse, " +
                    "@RefAjustadorAudienciaId, @RefAtencionId, @RefProveedorAgenteAbogadoId, @IdAgente, " +
                    "@AbogadoNombre, @NotificacionPushEnviada, " +
                    "@AgendarAudiencia, @FechaHora, @Fecha, @Hora, @Lugar, @FechaHoraCalculada, @NuevoId OUTPUT",
                    new SqlParameter("@Endpoint", (object)entry.Endpoint ?? DBNull.Value),
                    new SqlParameter("@UsuarioApi", (object)entry.UsuarioApi ?? DBNull.Value),
                    new SqlParameter("@IpCliente", (object)entry.IpCliente ?? DBNull.Value),
                    new SqlParameter("@UserAgent", (object)entry.UserAgent ?? DBNull.Value),
                    new SqlParameter("@HttpStatusCode", entry.HttpStatusCode),
                    new SqlParameter("@EsExitoso", entry.EsExitoso),
                    new SqlParameter("@Etapa", (object)entry.Etapa ?? DBNull.Value),
                    new SqlParameter("@Mensaje", (object)entry.Mensaje ?? DBNull.Value),
                    new SqlParameter("@JsonRequest", (object)entry.JsonRequest ?? DBNull.Value),
                    new SqlParameter("@JsonResponse", (object)entry.JsonResponse ?? DBNull.Value),
                    new SqlParameter("@RefAjustadorAudienciaId", (object)entry.RefAjustadorAudienciaId ?? DBNull.Value),
                    new SqlParameter("@RefAtencionId", (object)entry.RefAtencionId ?? DBNull.Value),
                    new SqlParameter("@RefProveedorAgenteAbogadoId", (object)entry.RefProveedorAgenteAbogadoId ?? DBNull.Value),
                    new SqlParameter("@IdAgente", (object)entry.IdAgente ?? DBNull.Value),
                    new SqlParameter("@AbogadoNombre", (object)entry.AbogadoNombre ?? DBNull.Value),
                    new SqlParameter("@NotificacionPushEnviada", (object)entry.NotificacionPushEnviada ?? DBNull.Value),
                    new SqlParameter("@AgendarAudiencia", (object)entry.AgendarAudiencia ?? DBNull.Value),
                    new SqlParameter("@FechaHora", (object)entry.FechaHora ?? DBNull.Value),
                    new SqlParameter("@Fecha", (object)entry.Fecha ?? DBNull.Value),
                    new SqlParameter("@Hora", (object)entry.Hora ?? DBNull.Value),
                    new SqlParameter("@Lugar", (object)entry.Lugar ?? DBNull.Value),
                    new SqlParameter("@FechaHoraCalculada", (object)entry.FechaHoraCalculada ?? DBNull.Value),
                    nuevoId);
            }
            catch
            {
                // El log no debe interrumpir el flujo principal.
            }
        }

        public static void EnriquecerContextoHttp(AsignacionAudienciaAbogadoLogEntry entry, string usuarioApi)
        {
            if (entry == null)
            {
                return;
            }

            entry.UsuarioApi = usuarioApi;

            var request = HttpContext.Current?.Request;
            if (request == null)
            {
                return;
            }

            entry.IpCliente = request.UserHostAddress;
            entry.UserAgent = request.UserAgent;
        }

        public static string Serializar(object value)
        {
            if (value == null)
            {
                return null;
            }

            return JsonSerializer.Serialize(value);
        }
    }

    public class AsignacionAudienciaAbogadoLogEntry
    {
        public string Endpoint { get; set; }
        public string UsuarioApi { get; set; }
        public string IpCliente { get; set; }
        public string UserAgent { get; set; }
        public int HttpStatusCode { get; set; }
        public bool EsExitoso { get; set; }
        public string Etapa { get; set; }
        public string Mensaje { get; set; }
        public string JsonRequest { get; set; }
        public string JsonResponse { get; set; }
        public int? RefAjustadorAudienciaId { get; set; }
        public int? RefAtencionId { get; set; }
        public int? RefProveedorAgenteAbogadoId { get; set; }
        public int? IdAgente { get; set; }
        public string AbogadoNombre { get; set; }
        public bool? NotificacionPushEnviada { get; set; }
        public int? AgendarAudiencia { get; set; }
        public DateTime? FechaHora { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string Lugar { get; set; }
        public DateTime? FechaHoraCalculada { get; set; }
    }
}
