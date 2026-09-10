using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using System.Web;
using System.Xml.Linq;
using WSRestApiHelp.Models;

namespace WSRestApiHelp.Clases
{
    public static class BpmCargaReclamoLogService
    {
        public const string AmbienteProd = "PROD";
        public const string AmbienteQa = "QA";

        public const string EtapaBinding = "BINDING";
        public const string EtapaValidacionFecha = "VALIDACION_FECHA";
        public const string EtapaValidacionExpediente = "VALIDACION_EXPEDIENTE";
        public const string EtapaSoapFicohsa = "SOAP_FICOHSA";
        public const string EtapaExcepcion = "EXCEPCION";

        public static BpmCargaReclamoLogEntry FromPayload(ApiData.help_CargaReclamoSinau payload)
        {
            return MapPayload(
                payload?.Chasis,
                payload?.puntoServicio,
                payload?.Poliza,
                payload?.Certificado,
                payload?.NombreAsegurado,
                payload?.Sucursal,
                payload?.Producto,
                payload?.Cobertura,
                payload?.Ramo,
                payload?.FechaOcurrencia,
                payload?.Causa,
                payload?.ValorReserva,
                payload?.UsuarioBPM,
                payload?.Latitud,
                payload?.Longitud,
                payload?.NombreConductor,
                payload?.Genero,
                payload?.Parentesco,
                payload?.Observacion);
        }

        public static BpmCargaReclamoLogEntry FromPayload(ApiData.help_CargaReclamoSinauQA payload)
        {
            return MapPayload(
                payload?.Chasis,
                payload?.puntoServicio,
                payload?.Poliza,
                payload?.Certificado,
                payload?.NombreAsegurado,
                payload?.Sucursal,
                payload?.Producto,
                payload?.Cobertura,
                payload?.Ramo,
                payload?.FechaOcurrencia,
                payload?.Causa,
                payload?.ValorReserva,
                payload?.UsuarioBPM,
                payload?.Latitud,
                payload?.Longitud,
                payload?.NombreConductor,
                payload?.Genero,
                payload?.Parentesco,
                payload?.Observacion);
        }

        public static void Registrar(helpEntities db, BpmCargaReclamoLogEntry entry)
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
                    "EXEC dbo.help_P_Log_CargaReclamoBPM_Insert " +
                    "@Ambiente, @Endpoint, @UsuarioApi, @IpCliente, @UserAgent, " +
                    "@HttpStatusCode, @EsExitoso, @Etapa, @Mensaje, " +
                    "@CodigoRespuestaBpm, @DescripcionRespuestaBpm, @NumeroReclamo, @SolicitudBpm, " +
                    "@FechaOcurrenciaEnviada, @JsonRequest, @JsonResponse, @XmlRequestFicohsa, @XmlRespuestaFicohsa, " +
                    "@RefAjustadorAudienciaId, @RefAtencionId, " +
                    "@Chasis, @PuntoServicio, @Poliza, @Certificado, @NombreAsegurado, @Sucursal, " +
                    "@Producto, @Cobertura, @Ramo, @FechaOcurrencia, @Causa, @ValorReserva, @UsuarioBPM, " +
                    "@Latitud, @Longitud, @NombreConductor, @Genero, @Parentesco, @Observacion, @NuevoId OUTPUT",
                    new SqlParameter("@Ambiente", (object)entry.Ambiente ?? DBNull.Value),
                    new SqlParameter("@Endpoint", (object)entry.Endpoint ?? DBNull.Value),
                    new SqlParameter("@UsuarioApi", (object)entry.UsuarioApi ?? DBNull.Value),
                    new SqlParameter("@IpCliente", (object)entry.IpCliente ?? DBNull.Value),
                    new SqlParameter("@UserAgent", (object)entry.UserAgent ?? DBNull.Value),
                    new SqlParameter("@HttpStatusCode", entry.HttpStatusCode),
                    new SqlParameter("@EsExitoso", entry.EsExitoso),
                    new SqlParameter("@Etapa", (object)entry.Etapa ?? DBNull.Value),
                    new SqlParameter("@Mensaje", (object)entry.Mensaje ?? DBNull.Value),
                    new SqlParameter("@CodigoRespuestaBpm", (object)entry.CodigoRespuestaBpm ?? DBNull.Value),
                    new SqlParameter("@DescripcionRespuestaBpm", (object)entry.DescripcionRespuestaBpm ?? DBNull.Value),
                    new SqlParameter("@NumeroReclamo", (object)entry.NumeroReclamo ?? DBNull.Value),
                    new SqlParameter("@SolicitudBpm", (object)entry.SolicitudBpm ?? DBNull.Value),
                    new SqlParameter("@FechaOcurrenciaEnviada", (object)entry.FechaOcurrenciaEnviada ?? DBNull.Value),
                    new SqlParameter("@JsonRequest", (object)entry.JsonRequest ?? DBNull.Value),
                    new SqlParameter("@JsonResponse", (object)entry.JsonResponse ?? DBNull.Value),
                    new SqlParameter("@XmlRequestFicohsa", (object)entry.XmlRequestFicohsa ?? DBNull.Value),
                    new SqlParameter("@XmlRespuestaFicohsa", (object)entry.XmlRespuestaFicohsa ?? DBNull.Value),
                    new SqlParameter("@RefAjustadorAudienciaId", (object)entry.RefAjustadorAudienciaId ?? DBNull.Value),
                    new SqlParameter("@RefAtencionId", (object)entry.RefAtencionId ?? DBNull.Value),
                    new SqlParameter("@Chasis", (object)entry.Chasis ?? DBNull.Value),
                    new SqlParameter("@PuntoServicio", (object)entry.PuntoServicio ?? DBNull.Value),
                    new SqlParameter("@Poliza", (object)entry.Poliza ?? DBNull.Value),
                    new SqlParameter("@Certificado", (object)entry.Certificado ?? DBNull.Value),
                    new SqlParameter("@NombreAsegurado", (object)entry.NombreAsegurado ?? DBNull.Value),
                    new SqlParameter("@Sucursal", (object)entry.Sucursal ?? DBNull.Value),
                    new SqlParameter("@Producto", (object)entry.Producto ?? DBNull.Value),
                    new SqlParameter("@Cobertura", (object)entry.Cobertura ?? DBNull.Value),
                    new SqlParameter("@Ramo", (object)entry.Ramo ?? DBNull.Value),
                    new SqlParameter("@FechaOcurrencia", (object)entry.FechaOcurrencia ?? DBNull.Value),
                    new SqlParameter("@Causa", (object)entry.Causa ?? DBNull.Value),
                    new SqlParameter("@ValorReserva", (object)entry.ValorReserva ?? DBNull.Value),
                    new SqlParameter("@UsuarioBPM", (object)entry.UsuarioBPM ?? DBNull.Value),
                    new SqlParameter("@Latitud", (object)entry.Latitud ?? DBNull.Value),
                    new SqlParameter("@Longitud", (object)entry.Longitud ?? DBNull.Value),
                    new SqlParameter("@NombreConductor", (object)entry.NombreConductor ?? DBNull.Value),
                    new SqlParameter("@Genero", (object)entry.Genero ?? DBNull.Value),
                    new SqlParameter("@Parentesco", (object)entry.Parentesco ?? DBNull.Value),
                    new SqlParameter("@Observacion", (object)entry.Observacion ?? DBNull.Value),
                    nuevoId);
            }
            catch (Exception ex)
            {
                // El log no debe interrumpir el flujo principal del reclamo.
                // Si el SP no tiene @XmlRequestFicohsa (u otro desfase), el insert falla aquí en silencio.
                System.Diagnostics.Trace.TraceError(
                    "help_Log_CargaReclamoBPM insert failed: {0}",
                    ex.InnerException?.Message ?? ex.Message);
            }
        }

        public static void EnriquecerContextoHttp(BpmCargaReclamoLogEntry entry, string usuarioApi)
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

        /// <summary>
        /// Arma el envelope SOAP Carga_Reclamo_Sinau (mismo formato que Ficohsa).
        /// La clave se enmascara en el log (***).
        /// </summary>
        public static string BuildXmlRequestCargaReclamoSinau(
            string usuarioSoap,
            int publicacion,
            IEnumerable<KeyValuePair<string, string>> parametros)
        {
            XNamespace soap = "http://www.w3.org/2003/05/soap-envelope";
            XNamespace tem = "http://tempuri.org/";

            var parametrosElement = new XElement(tem + "parametros");
            if (parametros != null)
            {
                foreach (var p in parametros)
                {
                    parametrosElement.Add(
                        new XElement(tem + "Parametro",
                            new XElement(tem + "nombre", p.Key ?? string.Empty),
                            new XElement(tem + "valor", p.Value ?? string.Empty)));
                }
            }

            var envelope = new XElement(soap + "Envelope",
                new XAttribute(XNamespace.Xmlns + "soap", soap.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "tem", tem.NamespaceName),
                new XElement(soap + "Header",
                    new XElement(tem + "credenciales",
                        new XElement(tem + "usuario", usuarioSoap ?? string.Empty),
                        new XElement(tem + "clave", "***"),
                        new XElement(tem + "publicacion", publicacion.ToString()))),
                new XElement(soap + "Body",
                    new XElement(tem + "Carga_Reclamo_Sinau",
                        parametrosElement)));

            return envelope.ToString(SaveOptions.DisableFormatting);
        }

        public static IList<KeyValuePair<string, string>> BuildParametrosCargaReclamo(
            BpmCargaReclamoLogEntry entry,
            string fechaOcurrenciaEnviada)
        {
            if (entry == null)
            {
                return new List<KeyValuePair<string, string>>();
            }

            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("p_chasis", entry.Chasis),
                new KeyValuePair<string, string>("p_punto_servicio", entry.PuntoServicio),
                new KeyValuePair<string, string>("p_poliza", entry.Poliza),
                new KeyValuePair<string, string>("p_certificado", entry.Certificado),
                new KeyValuePair<string, string>("p_nombre_asegurado", entry.NombreAsegurado),
                new KeyValuePair<string, string>("p_sucursal", entry.Sucursal),
                new KeyValuePair<string, string>("p_producto", entry.Producto),
                new KeyValuePair<string, string>("p_ramo", entry.Ramo),
                new KeyValuePair<string, string>("p_fecha_ocurrencia", fechaOcurrenciaEnviada),
                new KeyValuePair<string, string>("p_causa", entry.Causa),
                new KeyValuePair<string, string>("p_valor_reserva", entry.ValorReserva),
                new KeyValuePair<string, string>("p_usuario_BPM", entry.UsuarioBPM),
                new KeyValuePair<string, string>("p_latitud", entry.Latitud),
                new KeyValuePair<string, string>("p_longitud", entry.Longitud),
                new KeyValuePair<string, string>("p_nombre_conductor", entry.NombreConductor),
                new KeyValuePair<string, string>("p_genero", entry.Genero),
                new KeyValuePair<string, string>("p_parentesco", entry.Parentesco),
                new KeyValuePair<string, string>("p_observacion", entry.Observacion),
                new KeyValuePair<string, string>("p_cobertura", entry.Cobertura)
            };
        }

        private static BpmCargaReclamoLogEntry MapPayload(
            string chasis,
            string puntoServicio,
            string poliza,
            string certificado,
            string nombreAsegurado,
            string sucursal,
            string producto,
            string cobertura,
            string ramo,
            string fechaOcurrencia,
            string causa,
            string valorReserva,
            string usuarioBpm,
            string latitud,
            string longitud,
            string nombreConductor,
            string genero,
            string parentesco,
            string observacion)
        {
            return new BpmCargaReclamoLogEntry
            {
                Chasis = chasis,
                PuntoServicio = puntoServicio,
                Poliza = poliza,
                Certificado = certificado,
                NombreAsegurado = nombreAsegurado,
                Sucursal = sucursal,
                Producto = producto,
                Cobertura = cobertura,
                Ramo = ramo,
                FechaOcurrencia = fechaOcurrencia,
                Causa = causa,
                ValorReserva = valorReserva,
                UsuarioBPM = usuarioBpm,
                Latitud = latitud,
                Longitud = longitud,
                NombreConductor = nombreConductor,
                Genero = genero,
                Parentesco = parentesco,
                Observacion = observacion
            };
        }
    }

    public class BpmCargaReclamoLogEntry
    {
        public string Ambiente { get; set; }
        public string Endpoint { get; set; }
        public string UsuarioApi { get; set; }
        public string IpCliente { get; set; }
        public string UserAgent { get; set; }
        public int HttpStatusCode { get; set; }
        public bool EsExitoso { get; set; }
        public string Etapa { get; set; }
        public string Mensaje { get; set; }
        public string CodigoRespuestaBpm { get; set; }
        public string DescripcionRespuestaBpm { get; set; }
        public string NumeroReclamo { get; set; }
        public string SolicitudBpm { get; set; }
        public string FechaOcurrenciaEnviada { get; set; }
        public string JsonRequest { get; set; }
        public string JsonResponse { get; set; }
        public string XmlRequestFicohsa { get; set; }
        public string XmlRespuestaFicohsa { get; set; }
        public int? RefAjustadorAudienciaId { get; set; }
        public int? RefAtencionId { get; set; }
        public string Chasis { get; set; }
        public string PuntoServicio { get; set; }
        public string Poliza { get; set; }
        public string Certificado { get; set; }
        public string NombreAsegurado { get; set; }
        public string Sucursal { get; set; }
        public string Producto { get; set; }
        public string Cobertura { get; set; }
        public string Ramo { get; set; }
        public string FechaOcurrencia { get; set; }
        public string Causa { get; set; }
        public string ValorReserva { get; set; }
        public string UsuarioBPM { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string NombreConductor { get; set; }
        public string Genero { get; set; }
        public string Parentesco { get; set; }
        public string Observacion { get; set; }
    }
}
