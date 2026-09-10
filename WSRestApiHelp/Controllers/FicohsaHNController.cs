using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.EnterpriseServices.Internal;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;
using System.Xml.Linq;
using WSRestApiHelp.Clases;
using WSRestApiHelp.Models;
using System.Text.Json;

namespace WSRestApiHelp.Controllers
{
    [Authorize]
    [RoutePrefix("api/FicohsaHN")]
    public class FicohsaHNController : ApiController
    {
     
        //CONSULTA WS HN
        private WSFICOHSASERVICIO_HN.WSPorSaludSoapClient WSFH_HN = new WSFICOHSASERVICIO_HN.WSPorSaludSoapClient();
        private WSFICOHSAHELP_HN.WSPorSaludSoapClient WSFH_QA = new WSFICOHSAHELP_HN.WSPorSaludSoapClient();
        public int Public_HN = 3;//Produccion 3, Pruebas 4
        public int Public_HNQA = 4;//Produccion 3, Pruebas 4

        private helpEntities db = new helpEntities();
        public string Msj { get; set; }

        private WSFICOHSAHELP_HN_PRO.WSPorSaludSoapClient WSFH = new WSFICOHSAHELP_HN_PRO.WSPorSaludSoapClient();


        public List<WSFICOHSASERVICIO_HN.clConsoltAsegurado> DatosAsegurado_HN = new List<WSFICOHSASERVICIO_HN.clConsoltAsegurado>();

        public List<object> Data = new List<object>();

        //CONSULTA METODO HN

        [HttpPost]
        [Route("Consultar_Asegurado_HN")] // /api/FicohsaHN/Consultar_Asegurado_HN/
        public IHttpActionResult Consultar_Asegurado_HN(ApiData.help_Consultar_Asegurado CA)
        {

            WSFICOHSASERVICIO_HN.Consulta_Consolidada_de_AseguradosRequest Consulta_HN = new WSFICOHSASERVICIO_HN.Consulta_Consolidada_de_AseguradosRequest()
            {
                credenciales = new WSFICOHSASERVICIO_HN.credenciales() { usuario = "userBanfarma", clave = "2014BANf@rm@8@nc@", publicacion = Public_HN },
                Nombre = CA.Nombre,
                Apellido = CA.Apellido,
                Poliza = CA.Poliza,
                Identificacion = CA.Identificacion,
                Placa = CA.Placa,
                Chasis = CA.Chasis
            };

            var Lista_Asegurado_Help_HN = WSFH_HN.Consulta_Consolidada_de_Asegurados(Consulta_HN.credenciales,
                                                                                     Consulta_HN.Nombre,
                                                                                     Consulta_HN.Apellido,
                                                                                     Consulta_HN.Poliza,
                                                                                     Consulta_HN.Certificado,
                                                                                     Consulta_HN.Identificacion,
                                                                                     Consulta_HN.Placa,
                                                                                     Consulta_HN.Chasis).ToList();

            if (Lista_Asegurado_Help_HN.FirstOrDefault().cod_error == "-1")
            {
                Data.Add(new
                        {
                            Mensaje = Lista_Asegurado_Help_HN.FirstOrDefault().mensaje.ToString()
                        }
                );
            }
            else
            {
                foreach (var i in Lista_Asegurado_Help_HN.First().aseguradoC.ToList())
                {
                    Data.Add(new WSFICOHSASERVICIO_HN.clConsoltAsegurado
                    {
                        TABLA_ARCHIVO = i.TABLA_ARCHIVO,
                        FECHA_DATOS = i.FECHA_DATOS,
                        PAIS = i.PAIS,
                        TIPO_ID_CONVENIO = i.TIPO_ID_CONVENIO,
                        EMPRESA = i.EMPRESA,
                        SUB_EMPRESA = i.SUB_EMPRESA,
                        TIPO_PLAN_ID = i.TIPO_PLAN_ID,
                        TIPO_PLAN_NOMBRE = i.TIPO_PLAN_NOMBRE,
                        POLIZA_INTERNA = i.POLIZA_INTERNA,
                        NOMBRE = i.NOMBRE,
                        SEXO = i.SEXO,
                        FECHA_NACIMIENTO = i.FECHA_NACIMIENTO,
                        TIPO_PERSONA = i.TIPO_PERSONA,
                        TIPO_IDENTIFICACION = i.TIPO_IDENTIFICACION,
                        IDENTIFICACION = i.IDENTIFICACION,
                        RTN_NIT = i.RTN_NIT,
                        TIPO_ID_CLIENTE = i.TIPO_ID_CLIENTE,
                        ID_CLIENTE = i.ID_CLIENTE,
                        TIPO_ID_PERSONA = i.TIPO_ID_PERSONA,
                        ID_PERSONA = i.ID_PERSONA,
                        TELEFONO1 = i.TELEFONO1,
                        TELEFONO2 = i.TELEFONO2,
                        CIUDAD = i.CIUDAD,
                        DIRECCION = i.DIRECCION,
                        CORREO = i.CORREO,
                        TIPO_BENEFICIARIO = i.TIPO_BENEFICIARIO,
                        CLASE = i.CLASE,
                        RAMO = i.RAMO,
                        ID_PLAN = i.ID_PLAN,
                        PLAN = i.PLAN,
                        POLIZA = i.POLIZA,
                        CERTIFICADO = i.CERTIFICADO,
                        ID_CONTRATANTE = i.ID_CONTRATANTE,
                        CONTRATANTE_NOMBRE = i.CONTRATANTE_NOMBRE,
                        ID_ASEGURADORA = i.ID_ASEGURADORA,
                        ASEGURADORA_NOMBRE = i.ASEGURADORA_NOMBRE,
                        FECHA_INGRESO = i.FECHA_INGRESO,
                        FECHA_INI = i.FECHA_INI,
                        FECHA_FIN = i.FECHA_FIN,
                        FECHA_PAGO = i.FECHA_PAGO,
                        DIAS_MORA = i.DIAS_MORA,
                        REQ_ADMINISTRATIVO = i.REQ_ADMINISTRATIVO,
                        PRE_EXISTENCIA = i.PRE_EXISTENCIA,
                        CORREDOR_EMAIL = i.CORREDOR_EMAIL,
                        CORREDOR_NOMBRE = i.CORREDOR_NOMBRE,
                        ESTADO = i.ESTADO,
                        CAMPO_EXTRA_GM_1 = i.CAMPO_EXTRA_GM_1,
                        CAMPO_EXTRA_GM_2 = i.CAMPO_EXTRA_GM_2,
                        CAMPO_EXTRA_GM_3 = i.CAMPO_EXTRA_GM_3,
                        CAMPO_EXTRA_GM_4 = i.CAMPO_EXTRA_GM_4,
                        CAMPO_EXTRA_GM_5 = i.CAMPO_EXTRA_GM_5,
                        ID_AGENTE = i.ID_AGENTE,
                        AGENTE_NOMBRE = i.AGENTE_NOMBRE,
                        AGENTE_TELEFONO = i.AGENTE_TELEFONO,
                        AGENTE_EMAIL = i.AGENTE_EMAIL,
                        ASISTENCIA = i.ASISTENCIA,
                        ANO = i.ANO,
                        TIPO_VEHICULO = i.TIPO_VEHICULO,
                        MARCA = i.MARCA,
                        MODELO = i.MODELO,
                        COLOR = i.COLOR,
                        PLACA = i.PLACA,
                        CHASIS = i.CHASIS,
                        MOTOR = i.MOTOR,
                        COLISION = i.COLISION,
                        DANOS_TASA = i.DANOS_TASA,
                        ROBO = i.ROBO,
                        ROBO_TASA = i.ROBO_TASA,
                        SECCION1 = i.SECCION1,
                        SECCION2 = i.SECCION2,
                        SECCION3 = i.SECCION3,
                        CAMPO_EXTRA_AU_1 = i.CAMPO_EXTRA_AU_1,
                        CAMPO_EXTRA_AU_2 = i.CAMPO_EXTRA_AU_2,
                        CAMPO_EXTRA_AU_3 = i.CAMPO_EXTRA_AU_3,
                        CAMPO_EXTRA_AU_4 = i.CAMPO_EXTRA_AU_4,
                        CAMPO_EXTRA_AU_5 = i.CAMPO_EXTRA_AU_5
                    });
                }
            }

            return Ok(Data);

        }

        /// <summary>
        /// Valida datos fundamentales del expediente contra Consulta_Consolidada_de_Asegurados (QA)
        /// usando solo el Chasis. Mapeo: MapeoExpedienteWebService.xlsx
        /// </summary>
        [HttpPost]
        [Route("ValidarDatosReclamoBpm")] // /api/FicohsaHN/ValidarDatosReclamoBpm/
        public IHttpActionResult ValidarDatosReclamoBpm([FromBody] ApiData.ValidarDatosReclamoBpmRequest request)
        {
            try
            {
                if (request == null || request.IdAtencion <= 0)
                {
                    return BadRequest("Debe enviar IdAtencion.");
                }

                int agenteAutenticado;
                var acceso = AgenteAccessHelper.RequireAtencionOwnedByAgente(this, db, request.IdAtencion, out agenteAutenticado);
                if (acceso != null)
                    return acceso;

                var expediente = db.help_F_Api_Datos_Expediente_Siniestro(request.IdAtencion).FirstOrDefault();
                if (expediente == null)
                {
                    return BadRequest("No se encontro expediente para el IdAtencion indicado.");
                }

                string chasis = !string.IsNullOrWhiteSpace(request.Chasis)
                    ? request.Chasis.Trim()
                    : BpmValidacionReclamoService.ResolverChasisExpediente(expediente);

                if (string.IsNullOrWhiteSpace(chasis))
                {
                    return BadRequest("El expediente no tiene Chasis y no se envio Chasis en la solicitud.");
                }

                // Consulta consolidada: usar PROD (publicacion 3 / WSFH_HN).
                // En QA (publicacion 4) Ficohsa responde ORA-00904 "RONUM" en PKG_WSINFOASEGPS.
                var lista = WSFH_HN.Consulta_Consolidada_de_Asegurados(
                    new WSFICOHSASERVICIO_HN.credenciales
                    {
                        usuario = "userBanfarma",
                        clave = "2014BANf@rm@8@nc@",
                        publicacion = Public_HN
                    },
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    chasis);

                var root = lista?.FirstOrDefault();
                if (root == null)
                {
                    return Ok(new
                    {
                        AfiliadoEncontrado = false,
                        ListoParaBpm = false,
                        ChasisConsultado = chasis,
                        CodError = (string)null,
                        Mensaje = "Sin respuesta del Web Service.",
                        AmbienteConsulta = "PROD",
                        IdAtencion = request.IdAtencion,
                        IdAjusteAudiencia = expediente.IdAjusteAudiencia,
                        Campos = Array.Empty<object>(),
                        SugeridoParaBpm = (object)null,
                        CamposNoValidadosAlInicio = BpmValidacionReclamoService.CamposNoValidadosAlInicio
                    });
                }

                if (root.cod_error == "-1")
                {
                    return Ok(new
                    {
                        AfiliadoEncontrado = false,
                        ListoParaBpm = false,
                        ChasisConsultado = chasis,
                        CodError = root.cod_error,
                        Mensaje = root.mensaje,
                        AmbienteConsulta = "PROD",
                        IdAtencion = request.IdAtencion,
                        IdAjusteAudiencia = expediente.IdAjusteAudiencia,
                        Campos = Array.Empty<object>(),
                        SugeridoParaBpm = (object)null,
                        CamposNoValidadosAlInicio = BpmValidacionReclamoService.CamposNoValidadosAlInicio
                    });
                }

                var asegurado = root.aseguradoC?.FirstOrDefault();
                if (asegurado == null)
                {
                    return Ok(new
                    {
                        AfiliadoEncontrado = false,
                        ListoParaBpm = false,
                        ChasisConsultado = chasis,
                        CodError = root.cod_error,
                        Mensaje = string.IsNullOrWhiteSpace(root.mensaje)
                            ? "Consulta sin detalle de asegurado."
                            : root.mensaje,
                        AmbienteConsulta = "PROD",
                        IdAtencion = request.IdAtencion,
                        IdAjusteAudiencia = expediente.IdAjusteAudiencia,
                        Campos = Array.Empty<object>(),
                        SugeridoParaBpm = (object)null,
                        CamposNoValidadosAlInicio = BpmValidacionReclamoService.CamposNoValidadosAlInicio
                    });
                }

                // Mapear a tipo QA del validador (mismas propiedades de negocio).
                var aseguradoQa = MapearAseguradoProdAQa(asegurado);
                var campos = BpmValidacionReclamoService.CompararExpedienteConAsegurado(expediente, aseguradoQa);
                bool listoParaBpm = campos.All(c => c.Coincide);

                return Ok(new
                {
                    AfiliadoEncontrado = true,
                    ListoParaBpm = listoParaBpm,
                    ChasisConsultado = chasis,
                    CodError = root.cod_error,
                    Mensaje = root.mensaje,
                    AmbienteConsulta = "PROD",
                    IdAtencion = request.IdAtencion,
                    IdAjusteAudiencia = expediente.IdAjusteAudiencia,
                    Campos = campos,
                    SugeridoParaBpm = BpmValidacionReclamoService.BuildSugeridoParaBpm(aseguradoQa, expediente),
                    CamposNoValidadosAlInicio = BpmValidacionReclamoService.CamposNoValidadosAlInicio
                });
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return BadRequest(ex.InnerException.Message);
                }

                return BadRequest(ex.Message);
            }
        }

        private static WSFICOHSAHELP_HN.clConsoltAsegurado MapearAseguradoProdAQa(
            WSFICOHSASERVICIO_HN.clConsoltAsegurado fuente)
        {
            if (fuente == null)
            {
                return null;
            }

            return new WSFICOHSAHELP_HN.clConsoltAsegurado
            {
                CHASIS = fuente.CHASIS,
                POLIZA = fuente.POLIZA,
                CERTIFICADO = fuente.CERTIFICADO,
                CONTRATANTE_NOMBRE = fuente.CONTRATANTE_NOMBRE,
                CAMPO_EXTRA_GM_3 = fuente.CAMPO_EXTRA_GM_3,
                CAMPO_EXTRA_AU_2 = fuente.CAMPO_EXTRA_AU_2,
                NOMBRE = fuente.NOMBRE,
                PLACA = fuente.PLACA,
                RAMO = fuente.RAMO,
                ESTADO = fuente.ESTADO
            };
        }

        [HttpPost]
        [Route("Carga_Reclamo_Sinau_BPM_Fico")] // /api/FicohsaHN/Carga_Reclamo_Sinau_BPM_Fico/
        public IHttpActionResult Carga_Reclamo_Sinau_BPM_Fico([FromBody] ApiData.help_CargaReclamoSinau payload)
        {
            return ProcesarCargaReclamoBpm(
                BpmCargaReclamoLogService.FromPayload(payload),
                payload,
                BpmCargaReclamoLogService.AmbienteProd,
                "Carga_Reclamo_Sinau_BPM_Fico",
                false);
        }

        //SUBIR ARCHIVOS WS HN QA
        [HttpPost]
        [Route("Carga_Reclamo_Sinau_BPM_FicoQA")] // /api/FicohsaHN/Carga_Reclamo_Sinau_BPM_Fico/
        public IHttpActionResult Carga_Reclamo_Sinau_BPM_FicoQA([FromBody] ApiData.help_CargaReclamoSinauQA payload)
        {
            return ProcesarCargaReclamoBpm(
                BpmCargaReclamoLogService.FromPayload(payload),
                payload,
                BpmCargaReclamoLogService.AmbienteQa,
                "Carga_Reclamo_Sinau_BPM_FicoQA",
                true);
        }

        private IHttpActionResult ProcesarCargaReclamoBpm<TPayload>(
            BpmCargaReclamoLogEntry logEntry,
            TPayload payload,
            string ambiente,
            string endpoint,
            bool esQa)
        {
            logEntry.Ambiente = ambiente;
            logEntry.Endpoint = endpoint;
            logEntry.JsonRequest = BpmCargaReclamoLogService.Serializar(payload);
            BpmCargaReclamoLogService.EnriquecerContextoHttp(logEntry, User?.Identity?.Name);

            if (payload == null)
            {
                var bindingErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message : e.ErrorMessage)
                    .Where(message => !string.IsNullOrWhiteSpace(message));

                var errorMessage = string.Join(" | ", bindingErrors);
                if (string.IsNullOrWhiteSpace(errorMessage))
                {
                    errorMessage = "No se recibio el JSON del reclamo. Envie Content-Type application/json con las propiedades en la raiz del objeto (Chasis, puntoServicio, Poliza, etc.), no anidadas bajo 'FS'.";
                }

                return RegistrarLogYResponder(logEntry, BadRequest(errorMessage), BpmCargaReclamoLogService.EtapaBinding, errorMessage);
            }

            DateTime fechaOcurrencia;
            if (!TryResolveFechaOcurrencia(logEntry.FechaOcurrencia, out fechaOcurrencia))
            {
                const string mensajeFecha = "FechaOcurrencia invalida o vacia.";
                return RegistrarLogYResponder(logEntry, BadRequest(mensajeFecha), BpmCargaReclamoLogService.EtapaValidacionFecha, mensajeFecha);
            }

            int idTablaAjustador;
            if (!int.TryParse(logEntry.Observacion, out idTablaAjustador) || idTablaAjustador == 0)
            {
                const string mensajeExpediente = "Codigo de tabla del expediente ajustador audiencia no encontrado, o no es compatible para el envio de BPM";
                logEntry.RefAjustadorAudienciaId = idTablaAjustador == 0 ? (int?)null : idTablaAjustador;
                return RegistrarLogYResponder(logEntry, BadRequest(mensajeExpediente), BpmCargaReclamoLogService.EtapaValidacionExpediente, mensajeExpediente);
            }

            logEntry.RefAjustadorAudienciaId = idTablaAjustador;

            var validacion = db.help_AjustadorAudiencia.Where(a => a.Id == idTablaAjustador).ToList();
            if (validacion.Count == 0)
            {
                const string mensajeNoEncontrado = "Expediente no encontrado en ajustador audiencia.";
                return RegistrarLogYResponder(logEntry, BadRequest(mensajeNoEncontrado), BpmCargaReclamoLogService.EtapaValidacionExpediente, mensajeNoEncontrado);
            }

            logEntry.RefAtencionId = db.Database.SqlQuery<int?>(
                "SELECT RefAtencionId FROM dbo.help_AjustadorAudiencia WHERE Id = @p0",
                idTablaAjustador).FirstOrDefault();

            try
            {
                ObjectParameter respuestaFecha = new ObjectParameter("Respuesta", typeof(DateTime));
                ObjectParameter mensajeFechaSp = new ObjectParameter("Mensaje", typeof(string));
                db.help_P_Api_RetornarFechaFicohsaUTC(fechaOcurrencia, respuestaFecha, mensajeFechaSp);

                string fechaocu = respuestaFecha.Value?.ToString();
                logEntry.FechaOcurrenciaEnviada = fechaocu;

                int publicacionSoap = esQa ? Public_HNQA : Public_HN;
                var parametrosLog = BpmCargaReclamoLogService.BuildParametrosCargaReclamo(logEntry, fechaocu);
                logEntry.XmlRequestFicohsa = BpmCargaReclamoLogService.BuildXmlRequestCargaReclamoSinau(
                    "userBanfarma",
                    publicacionSoap,
                    parametrosLog);

                XElement xml;
                if (esQa)
                {
                    var requestQa = new WSFICOHSAHELP_HN.Carga_Reclamo_SinauRequest
                    {
                        credenciales = new WSFICOHSAHELP_HN.credenciales { usuario = "userBanfarma", clave = "2014BANf@rm@8@nc@", publicacion = Public_HNQA },
                        parametros = CrearParametrosSoapQa(logEntry, fechaocu)
                    };

                    var respuestaQa = WSFH_QA.Carga_Reclamo_Sinau(requestQa.credenciales, requestQa.parametros);
                    xml = XElement.Parse(respuestaQa.OuterXml);
                }
                else
                {
                    var requestProd = new WSFICOHSASERVICIO_HN.Carga_Reclamo_SinauRequest
                    {
                        credenciales = new WSFICOHSASERVICIO_HN.credenciales { usuario = "userBanfarma", clave = "2014BANf@rm@8@nc@", publicacion = Public_HN },
                        parametros = CrearParametrosSoapProd(logEntry, fechaocu)
                    };

                    var respuestaProd = WSFH_HN.Carga_Reclamo_Sinau(requestProd.credenciales, requestProd.parametros);
                    xml = XElement.Parse(respuestaProd.OuterXml);
                }

                logEntry.XmlRespuestaFicohsa = xml.ToString();

                string codigo = xml.Element("codigo")?.Value;
                string descripcion = xml.Element("descripcion")?.Value;
                logEntry.CodigoRespuestaBpm = codigo;
                logEntry.DescripcionRespuestaBpm = descripcion;

                if (codigo != "0")
                {
                    var respuestaError = new
                    {
                        codigo = Convert.ToInt32(codigo),
                        descripcion
                    };

                    Data.Add(respuestaError);
                    logEntry.JsonResponse = BpmCargaReclamoLogService.Serializar(respuestaError);
                    logEntry.Mensaje = descripcion;

                    db.help_P_Log_EnvioBPMS(null, null, idTablaAjustador, DateTime.Now, descripcion, logEntry.JsonRequest);

                    return RegistrarLogYResponder(
                        logEntry,
                        Ok(Data),
                        BpmCargaReclamoLogService.EtapaSoapFicohsa,
                        descripcion,
                        esExitoso: false,
                        httpStatusCode: 200);
                }

                string numeroReclamo = xml.Element("numero_reclamo")?.Value;
                string solicitudBpm = xml.Element("solicitud_bpm")?.Value;
                logEntry.NumeroReclamo = numeroReclamo;
                logEntry.SolicitudBpm = solicitudBpm;

                var respuestaExito = new
                {
                    codigo = Convert.ToInt32(codigo),
                    descripcion,
                    numero_reclamo = Convert.ToInt32(numeroReclamo),
                    solicitud_bpm = Convert.ToInt32(solicitudBpm)
                };

                Data.Add(respuestaExito);
                logEntry.JsonResponse = BpmCargaReclamoLogService.Serializar(respuestaExito);
                logEntry.Mensaje = descripcion;

                db.help_P_Log_EnvioBPMS(numeroReclamo, solicitudBpm, idTablaAjustador, DateTime.Now, descripcion, logEntry.JsonRequest);

                return RegistrarLogYResponder(
                    logEntry,
                    Ok(Data),
                    BpmCargaReclamoLogService.EtapaSoapFicohsa,
                    descripcion,
                    esExitoso: true,
                    httpStatusCode: 200);
            }
            catch (Exception ex)
            {
                return RegistrarLogYResponder(
                    logEntry,
                    InternalServerError(ex),
                    BpmCargaReclamoLogService.EtapaExcepcion,
                    ex.Message,
                    esExitoso: false,
                    httpStatusCode: 500);
            }
        }

        private static WSFICOHSASERVICIO_HN.Parametro[] CrearParametrosSoapProd(BpmCargaReclamoLogEntry entry, string fechaocu)
        {
            return new[]
            {
                new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_chasis", valor = entry.Chasis },
                new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_punto_servicio", valor = entry.PuntoServicio },
                new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_poliza", valor = entry.Poliza },
                new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_certificado", valor = entry.Certificado },
                new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_nombre_asegurado", valor = entry.NombreAsegurado },
                new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_sucursal", valor = entry.Sucursal },
                new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_producto", valor = entry.Producto },
                new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_ramo", valor = entry.Ramo },
                new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_fecha_ocurrencia", valor = fechaocu },
                new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_causa", valor = entry.Causa },
                new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_valor_reserva", valor = entry.ValorReserva },
                new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_usuario_BPM", valor = entry.UsuarioBPM },
                new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_latitud", valor = entry.Latitud },
                new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_longitud", valor = entry.Longitud },
                new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_nombre_conductor", valor = entry.NombreConductor },
                new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_genero", valor = entry.Genero },
                new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_parentesco", valor = entry.Parentesco },
                new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_observacion", valor = entry.Observacion },
                new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_cobertura", valor = entry.Cobertura }
            };
        }

        private static WSFICOHSAHELP_HN.Parametro[] CrearParametrosSoapQa(BpmCargaReclamoLogEntry entry, string fechaocu)
        {
            return new[]
            {
                new WSFICOHSAHELP_HN.Parametro { nombre = "p_chasis", valor = entry.Chasis },
                new WSFICOHSAHELP_HN.Parametro { nombre = "p_punto_servicio", valor = entry.PuntoServicio },
                new WSFICOHSAHELP_HN.Parametro { nombre = "p_poliza", valor = entry.Poliza },
                new WSFICOHSAHELP_HN.Parametro { nombre = "p_certificado", valor = entry.Certificado },
                new WSFICOHSAHELP_HN.Parametro { nombre = "p_nombre_asegurado", valor = entry.NombreAsegurado },
                new WSFICOHSAHELP_HN.Parametro { nombre = "p_sucursal", valor = entry.Sucursal },
                new WSFICOHSAHELP_HN.Parametro { nombre = "p_producto", valor = entry.Producto },
                new WSFICOHSAHELP_HN.Parametro { nombre = "p_ramo", valor = entry.Ramo },
                new WSFICOHSAHELP_HN.Parametro { nombre = "p_fecha_ocurrencia", valor = fechaocu },
                new WSFICOHSAHELP_HN.Parametro { nombre = "p_causa", valor = entry.Causa },
                new WSFICOHSAHELP_HN.Parametro { nombre = "p_valor_reserva", valor = entry.ValorReserva },
                new WSFICOHSAHELP_HN.Parametro { nombre = "p_usuario_BPM", valor = entry.UsuarioBPM },
                new WSFICOHSAHELP_HN.Parametro { nombre = "p_latitud", valor = entry.Latitud },
                new WSFICOHSAHELP_HN.Parametro { nombre = "p_longitud", valor = entry.Longitud },
                new WSFICOHSAHELP_HN.Parametro { nombre = "p_nombre_conductor", valor = entry.NombreConductor },
                new WSFICOHSAHELP_HN.Parametro { nombre = "p_genero", valor = entry.Genero },
                new WSFICOHSAHELP_HN.Parametro { nombre = "p_parentesco", valor = entry.Parentesco },
                new WSFICOHSAHELP_HN.Parametro { nombre = "p_observacion", valor = entry.Observacion },
                new WSFICOHSAHELP_HN.Parametro { nombre = "p_cobertura", valor = entry.Cobertura }
            };
        }

        private IHttpActionResult RegistrarLogYResponder(
            BpmCargaReclamoLogEntry logEntry,
            IHttpActionResult response,
            string etapa,
            string mensaje,
            bool? esExitoso = null,
            int? httpStatusCode = null)
        {
            logEntry.Etapa = etapa;
            logEntry.Mensaje = mensaje;
            logEntry.EsExitoso = esExitoso ?? (response is OkNegotiatedContentResult<object> || response is OkNegotiatedContentResult<List<object>>);
            logEntry.HttpStatusCode = httpStatusCode ?? ObtenerHttpStatusCode(response);
            if (string.IsNullOrWhiteSpace(logEntry.JsonResponse))
            {
                logEntry.JsonResponse = BpmCargaReclamoLogService.Serializar(new { Message = mensaje });
            }

            BpmCargaReclamoLogService.Registrar(db, logEntry);
            return response;
        }

        private static int ObtenerHttpStatusCode(IHttpActionResult response)
        {
            if (response is BadRequestErrorMessageResult)
            {
                return 400;
            }

            if (response is InternalServerErrorResult)
            {
                return 500;
            }

            if (response is UnauthorizedResult)
            {
                return 401;
            }

            return 200;
        }

        //FIN QA

        [HttpPost]
        [Route("Subir_Archivos_WSFH")] // /api/FicohsaHN/Subir_Archivos_WSFH/
        public IHttpActionResult Subir_Archivos_WSFH(ApiData.SubirArchivosFicohsa SubirArchivo)
        {
            byte[] FotoData = System.Convert.FromBase64String(SubirArchivo.filetxt);

            WSFICOHSASERVICIO_HN.UploadFileRequest Consulta_Subir_Archivo_Siniestro = new WSFICOHSASERVICIO_HN.UploadFileRequest()
            {
                credenciales = new WSFICOHSASERVICIO_HN.credenciales() { usuario = "userBanfarma", clave = "2014BANf@rm@8@nc@", publicacion = Public_HN },
                f = FotoData,
                cod_solicitud = SubirArchivo.CodigoSolicitud_BPM,
                fileName = SubirArchivo.NombreTipoExtensionFile
            };

            var Lista_Subir_Archivo = WSFH_HN.UploadFile(Consulta_Subir_Archivo_Siniestro.credenciales, Consulta_Subir_Archivo_Siniestro.cod_solicitud, Consulta_Subir_Archivo_Siniestro.f, Consulta_Subir_Archivo_Siniestro.fileName);

            Data.Add(new {
                Respuesta = Lista_Subir_Archivo
            });

            return Ok(Data);
        }

        [HttpPost]
        [Route("Consultar_Siniestro_Requisitos_HN")] // /api/FicohsaHN/Consultar_Sniestro_Requisitos_HN/
        public IHttpActionResult Consultar_Siniestro_Requisitos_HN(string NumeroSiniestro)
        {
            WSFICOHSASERVICIO_HN.Consulta_RequisitosRequest consulta_requisitos_HN = new WSFICOHSASERVICIO_HN.Consulta_RequisitosRequest()
            {
                credenciales = new WSFICOHSASERVICIO_HN.credenciales() { usuario = "userBanfarma", clave = "2014BANf@rm@8@nc@", publicacion = Public_HN },
                parametros = new WSFICOHSASERVICIO_HN.Parametro[]
                {
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_numsin", valor = NumeroSiniestro }
                }
            };

            var Lista_Consulta_Requisitos = WSFH_HN.Consulta_Requisitos(consulta_requisitos_HN.credenciales, consulta_requisitos_HN.parametros);

            XElement xml = XElement.Parse(Lista_Consulta_Requisitos.OuterXml);

            if (xml.Element("CODIGO").Value == "1")
            {
                Data.Add(new 
                {
                    CODIGO = xml.Element("CODIGO").Value,
                    DESCRIPCION = xml.Element("DESCRIPCION").Value,
                });
            }
            else
            {

                foreach (var item in xml.Descendants("REQUISITO"))
                {

                    Data.Add(new 
                    {
                        ESTADO = item.Element("ESTADO").Value,
                        CODIGO = item.Element("CODIGO").Value,
                        DESCRIPCION = item.Element("DESCRIPCION").Value,
                        FECHA_SOLICITUD = item.Element("FECHA_SOLICITUD").Value
                    }
                    );
                }

            }

            return Ok(Data);
        }

        [HttpPost]
        [Route("Consultar_Estado_Sinietro")] // /api/FicohsaHN/Consultar_Estado_Sinietro/
        public IHttpActionResult Consultar_Estado_Sinietro(string NumeroSiniestro)
        {

            WSFICOHSASERVICIO_HN.Estado_SiniestroRequest Consulta_Siniestro_HN = new WSFICOHSASERVICIO_HN.Estado_SiniestroRequest()
            {
                credenciales = new WSFICOHSASERVICIO_HN.credenciales() { usuario = "userBanfarma", clave = "2014BANf@rm@8@nc@", publicacion = Public_HN },
                parametros = new WSFICOHSASERVICIO_HN.Parametro[]
                {
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_numsin", valor = NumeroSiniestro }
                }
            };

            var Lista_Consulta_Estado_Siniestro = WSFH_HN.Estado_Siniestro(Consulta_Siniestro_HN.credenciales, Consulta_Siniestro_HN.parametros);

            XElement xml = XElement.Parse(Lista_Consulta_Estado_Siniestro.OuterXml);

            if (xml.Element("CODIGO").Value != "1")
            {
                Data.Add(new 
                {
                    CODIGO = Convert.ToInt32(xml.Element("CODIGO").Value),
                    DESCRIPCION = xml.Element("descripcion").Value,
                });
            }
            else
            {
                Data.Add(new
                {
                    RECLAMO = Convert.ToInt32(xml.Element("RECLAMO").Value),
                    CONTRATANTE = xml.Element("CONTRATANTE").Value,
                    POLIZA = Convert.ToInt32(xml.Element("POLIZA").Value),
                    CERTIFICADO = Convert.ToInt32(xml.Element("CERTIFICADO").Value),
                    ESTADO = xml.Element("ESTADO").Value,
                    MARCA = xml.Element("MARCA").Value,
                    MODELO = xml.Element("MODELO").Value,
                    PLACA = xml.Element("PLACA").Value,
                    LUGAR_ACCIDENTE = xml.Element("LUGAR_ACCIDENTE").Value,
                    FECHA_OCURRENCIA = xml.Element("FECHA_OCURRENCIA").Value,
                    TALLER = xml.Element("TALLER").Value,
                });
            }

            return Ok(Data);
        }


        [HttpPost]
        [Route("Valida_Lista_Coberturas")] // /api/FicohsaHN/Valida_Lista_Coberturas/
        public IHttpActionResult Valida_Lista_Coberturas(ApiData.CoberturaData cobertura)
        {

            WSFICOHSAHELP_HN_PRO.valida_lista_coberturasRequest Consulta = new WSFICOHSAHELP_HN_PRO.valida_lista_coberturasRequest()
            {
                credenciales = new WSFICOHSAHELP_HN_PRO.credenciales() { usuario = "userBanfarma", clave = "2014BANf@rm@8@nc@", publicacion = Public_HN },
                nIdepol = null,
                nNumOper = null,
                nNumcer = Convert.ToInt32(cobertura.pNumCertificado),//13424,
                nIdeBien = null,
                nIdeAseg =null,
                nNumpol = Convert.ToInt32(cobertura.pNumPoliza),//1000000332,
                pCodProd = "AU01"
            };

            var listaCoberturas = WSFH.valida_lista_coberturas(
                Consulta.credenciales,
                Consulta.nIdepol,
                Consulta.nNumOper,
                Consulta.nNumcer,
                Consulta.nIdeBien,
                Consulta.nIdeAseg,
                Consulta.nNumpol,
                Consulta.pCodProd
                ).ToList();

           
           
            if (listaCoberturas.FirstOrDefault().cod_error == "-1")
            {
                Data.Add(new
                {
                    Mensaje = listaCoberturas.FirstOrDefault().mensaje.ToString()
                }
                );
            }
            else
            {
                foreach (var item in listaCoberturas.First().clDetListaCobert.ToList())
                {
                    Data.Add(new WSFICOHSAHELP_HN.clDetListaCobert
                    {
                        CODIGO = item.CODIGO,
                        DESCRIPCION = item.DESCRIPCION,
                        COBERTURA = item.COBERTURA,
                        SUMA = item.SUMA,
                    });
                }
            }

            return Ok(Data);

        }

        [HttpPost]
        [Route("Consulta_Causas_HN")]
        public IHttpActionResult Consulta_Causas_HN(ApiData.ConsultaCausasData request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.cCodCobert))
            {
                return BadRequest("Debe enviar el codigo de cobertura.");
            }

            var resultado = WSFH_QA.consultaCausas(
                new WSFICOHSAHELP_HN.credenciales()
                {
                    usuario = "userBanfarma",
                    clave = "2014BANf@rm@8@nc@",
                    publicacion = Public_HNQA
                },
                string.IsNullOrWhiteSpace(request.pCodProd) ? "AU01" : request.pCodProd,
                request.cCodCobert
            ).ToList();

            var data = new List<object>();

            if (resultado.FirstOrDefault()?.cod_error == "-1")
            {
                data.Add(new { Mensaje = resultado.First().mensaje });
            }
            else if (resultado.FirstOrDefault()?.clDetConsultaCausas != null)
            {
                foreach (var item in resultado.First().clDetConsultaCausas)
                {
                    data.Add(new WSFICOHSAHELP_HN.clDetConsultaCausas
                    {
                        CODCAUSA = item.CODCAUSA,
                        DESCRCAUSA = item.DESCRCAUSA
                    });
                }
            }

            return Ok(data);
        }

        private static bool TryResolveFechaOcurrencia(string rawFecha, out DateTime fecha)
        {
            fecha = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(rawFecha))
            {
                return false;
            }

            return DateTime.TryParse(
                rawFecha,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal,
                out fecha
            ) || DateTime.TryParse(rawFecha, out fecha);
        }

    }
}