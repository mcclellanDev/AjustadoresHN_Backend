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
using System.Xml.Linq;
using WSRestApiHelp.Clases;
using WSRestApiHelp.Models;

namespace WSRestApiHelp.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
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

        [HttpPost]
        [Route("Carga_Reclamo_Sinau_BPM_Fico")] // /api/FicohsaHN/Carga_Reclamo_Sinau_BPM_Fico/
        public IHttpActionResult Carga_Reclamo_Sinau_BPM_Fico([FromBody] ApiData.help_CargaReclamoSinau payload)
        {
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

                return BadRequest(errorMessage);
            }

            DateTime fechaOcurrencia;
            if (!TryResolveFechaOcurrencia(payload.FechaOcurrencia, out fechaOcurrencia))
            {
                return BadRequest("FechaOcurrencia invalida o vacia.");
            }

            string fechaocu = "";

            ObjectParameter R = new ObjectParameter("Respuesta", typeof(DateTime));
            ObjectParameter M = new ObjectParameter("Mensaje", typeof(string));
            db.help_P_Api_RetornarFechaFicohsaUTC(fechaOcurrencia, R, M);

            int IdTablaAjustador = Convert.ToInt32(payload.Observacion);

            if (IdTablaAjustador != 0) {


                var Validacion = db.help_AjustadorAudiencia.Where(a => a.Id == IdTablaAjustador).ToList();

                if (Validacion.Count() > 0)
                {
                    //if (Validacion.First().CodigoBPMFicohsa != null) {


                    fechaocu = R.Value.ToString();

                    WSFICOHSASERVICIO_HN.Carga_Reclamo_SinauRequest Consulta_Credenciales_CRS_HN = new WSFICOHSASERVICIO_HN.Carga_Reclamo_SinauRequest()
                    {
                        credenciales = new WSFICOHSASERVICIO_HN.credenciales() { usuario = "userBanfarma", clave = "2014BANf@rm@8@nc@", publicacion = Public_HN },
                        parametros = new WSFICOHSASERVICIO_HN.Parametro[]
                        {
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_chasis", valor = payload.Chasis },
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_punto_servicio", valor = payload.puntoServicio },
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_poliza", valor = payload.Poliza },
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_certificado", valor = payload.Certificado },
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_nombre_asegurado", valor = payload.NombreAsegurado },
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_sucursal", valor = payload.Sucursal },
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_producto", valor = payload.Producto },
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_ramo", valor = payload.Ramo },
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_fecha_ocurrencia", valor = fechaocu },
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_causa", valor = payload.Causa },
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_valor_reserva", valor = payload.ValorReserva },
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_usuario_BPM", valor = payload.UsuarioBPM },
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_latitud", valor = payload.Latitud },
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_longitud", valor = payload.Longitud },
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_nombre_conductor", valor = payload.NombreConductor },
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_genero", valor = payload.Genero },
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_parentesco", valor = payload.Parentesco },
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_observacion", valor = payload.Observacion },
                    new WSFICOHSASERVICIO_HN.Parametro { nombre = "p_cobertura", valor = payload.Cobertura }
                            }
                        };

                        var Lista_Asegurado_BPM = WSFH_HN.Carga_Reclamo_Sinau(Consulta_Credenciales_CRS_HN.credenciales, Consulta_Credenciales_CRS_HN.parametros);

                        XElement xml = XElement.Parse(Lista_Asegurado_BPM.OuterXml);

                        string CODIGO = xml.Element("codigo").Value.ToString();

                        if (CODIGO != "0")
                        {
                            Data.Add(new
                            {
                                codigo = Convert.ToInt32(xml.Element("codigo").Value),
                                descripcion = xml.Element("descripcion").Value,
                            });
                            
                            db.help_P_Log_EnvioBPMS(null, null, Convert.ToInt32(payload.Observacion), DateTime.Now, xml.Element("descripcion").Value.ToString(), payload.ToString());
                        }
                        else
                        {
                            Data.Add(new
                            {
                                codigo = Convert.ToInt32(xml.Element("codigo").Value),
                                descripcion = xml.Element("descripcion").Value,
                                numero_reclamo = Convert.ToInt32(xml.Element("numero_reclamo").Value),
                                solicitud_bpm = Convert.ToInt32(xml.Element("solicitud_bpm").Value)

                            });

                            db.help_P_Log_EnvioBPMS(xml.Element("numero_reclamo").Value.ToString(), xml.Element("solicitud_bpm").Value.ToString(), Convert.ToInt32(payload.Observacion), DateTime.Now, xml.Element("descripcion").Value.ToString(), payload.ToString());
                    }

                        return Ok(Data);

                    /*
                                        }

                                        else {

                                            return BadRequest("Codigo BPM ya fue enviado o creado, no se puede duplicar y reenviar");

                                        }
                                    */
                }
                else {

                    return BadRequest("Expediente no encontrado en ajustador audiencia.");
                
                }

            }
            else {

                return BadRequest("Codigo de tabla del expediente ajustador audiencia no encontrado, o no es compatible para el envio de BPM");

            }

            
        }

        //SUBIR ARCHIVOS WS HN QA
        [HttpPost]
        [Route("Carga_Reclamo_Sinau_BPM_FicoQA")] // /api/FicohsaHN/Carga_Reclamo_Sinau_BPM_Fico/
        public IHttpActionResult Carga_Reclamo_Sinau_BPM_FicoQA([FromBody] ApiData.help_CargaReclamoSinauQA payload)
        {
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

                return BadRequest(errorMessage);
            }

            DateTime fechaOcurrencia;
            if (!TryResolveFechaOcurrencia(payload.FechaOcurrencia, out fechaOcurrencia))
            {
                return BadRequest("FechaOcurrencia invalida o vacia.");
            }

            string fechaocu = "";

            ObjectParameter R = new ObjectParameter("Respuesta", typeof(DateTime));
            ObjectParameter M = new ObjectParameter("Mensaje", typeof(string));
            db.help_P_Api_RetornarFechaFicohsaUTC(fechaOcurrencia, R, M);

            int IdTablaAjustador = Convert.ToInt32(payload.Observacion);

            if (IdTablaAjustador != 0)
            {


                var Validacion = db.help_AjustadorAudiencia.Where(a => a.Id == IdTablaAjustador).ToList();

                if (Validacion.Count() > 0)
                {
                    //if (Validacion.First().CodigoBPMFicohsa != null) {


                    fechaocu = R.Value.ToString();

                    WSFICOHSAHELP_HN.Carga_Reclamo_SinauRequest Consulta_Credenciales_CRS_HNQA = new WSFICOHSAHELP_HN.Carga_Reclamo_SinauRequest()
                    {
                        credenciales = new WSFICOHSAHELP_HN.credenciales() { usuario = "userBanfarma", clave = "2014BANf@rm@8@nc@", publicacion = Public_HNQA },
                        parametros = new WSFICOHSAHELP_HN.Parametro[]
                        {
                            new WSFICOHSAHELP_HN.Parametro { nombre = "p_chasis", valor = payload.Chasis },
                            new WSFICOHSAHELP_HN.Parametro { nombre = "p_punto_servicio", valor = payload.puntoServicio },
                            new WSFICOHSAHELP_HN.Parametro { nombre = "p_poliza", valor = payload.Poliza },
                            new WSFICOHSAHELP_HN.Parametro { nombre = "p_certificado", valor = payload.Certificado },
                            new WSFICOHSAHELP_HN.Parametro { nombre = "p_nombre_asegurado", valor = payload.NombreAsegurado },
                            new WSFICOHSAHELP_HN.Parametro { nombre = "p_sucursal", valor = payload.Sucursal },
                            new WSFICOHSAHELP_HN.Parametro { nombre = "p_producto", valor = payload.Producto },
                            new WSFICOHSAHELP_HN.Parametro { nombre = "p_ramo", valor = payload.Ramo },
                            new WSFICOHSAHELP_HN.Parametro { nombre = "p_fecha_ocurrencia", valor = fechaocu },
                            new WSFICOHSAHELP_HN.Parametro { nombre = "p_causa", valor = payload.Causa },
                            new WSFICOHSAHELP_HN.Parametro { nombre = "p_valor_reserva", valor = payload.ValorReserva },
                            new WSFICOHSAHELP_HN.Parametro { nombre = "p_usuario_BPM", valor = payload.UsuarioBPM },
                            new WSFICOHSAHELP_HN.Parametro { nombre = "p_latitud", valor = payload.Latitud },
                            new WSFICOHSAHELP_HN.Parametro { nombre = "p_longitud", valor = payload.Longitud },
                            new WSFICOHSAHELP_HN.Parametro { nombre = "p_nombre_conductor", valor = payload.NombreConductor },
                            new WSFICOHSAHELP_HN.Parametro { nombre = "p_genero", valor = payload.Genero },
                            new WSFICOHSAHELP_HN.Parametro { nombre = "p_parentesco", valor = payload.Parentesco },
                            new WSFICOHSAHELP_HN.Parametro { nombre = "p_observacion", valor = payload.Observacion },
                            new WSFICOHSAHELP_HN.Parametro { nombre = "p_cobertura", valor = payload.Cobertura }
                            }
                    };

                    var Lista_Asegurado_BPM = WSFH_QA.Carga_Reclamo_Sinau(Consulta_Credenciales_CRS_HNQA.credenciales, Consulta_Credenciales_CRS_HNQA.parametros);

                    XElement xml = XElement.Parse(Lista_Asegurado_BPM.OuterXml);

                    string CODIGO = xml.Element("codigo").Value.ToString();

                    if (CODIGO != "0")
                    {
                        Data.Add(new
                        {
                            codigo = Convert.ToInt32(xml.Element("codigo").Value),
                            descripcion = xml.Element("descripcion").Value,
                        });

                        db.help_P_Log_EnvioBPMS(null, null, Convert.ToInt32(payload.Observacion), DateTime.Now, xml.Element("descripcion").Value.ToString(), payload.ToString());
                    }
                    else
                    {
                        Data.Add(new
                        {
                            codigo = Convert.ToInt32(xml.Element("codigo").Value),
                            descripcion = xml.Element("descripcion").Value,
                            numero_reclamo = Convert.ToInt32(xml.Element("numero_reclamo").Value),
                            solicitud_bpm = Convert.ToInt32(xml.Element("solicitud_bpm").Value)

                        });

                        db.help_P_Log_EnvioBPMS(xml.Element("numero_reclamo").Value.ToString(), xml.Element("solicitud_bpm").Value.ToString(), Convert.ToInt32(payload.Observacion), DateTime.Now, xml.Element("descripcion").Value.ToString(), payload.ToString());
                    }

                    return Ok(Data);

                    /*
                                        }

                                        else {

                                            return BadRequest("Codigo BPM ya fue enviado o creado, no se puede duplicar y reenviar");

                                        }
                                    */
                }
                else
                {

                    return BadRequest("Expediente no encontrado en ajustador audiencia.");

                }

            }
            else
            {

                return BadRequest("Codigo de tabla del expediente ajustador audiencia no encontrado, o no es compatible para el envio de BPM");

            }


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