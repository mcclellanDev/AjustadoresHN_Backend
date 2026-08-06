using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.UI.WebControls;
using WSRestApiHelp.Clases;
using WSRestApiHelp.Models;
using static WSRestApiHelp.Clases.ApiData;

namespace WSRestApiHelp.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    //[Authorize]
    [RoutePrefix("api/Proveedor")]
    public class ProveedorController : ApiController
    {
        //13 es Produccion
        //10 es Pruebas

        private helpEntities db = new helpEntities();

        public List<object> Data = new List<object>();

        private static string ObtenerBase64Limpio(string data)
        {
            if (String.IsNullOrWhiteSpace(data))
            {
                return data;
            }

            var commaIndex = data.IndexOf(',');
            if (data.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && commaIndex >= 0)
            {
                return data.Substring(commaIndex + 1);
            }

            return data;
        }



        [HttpGet]
        [Route("ListaVehiculosFicohsa")]
        public IHttpActionResult ListaVehiculosFicohsa()
        {
            try
            {
                //List<help_F_ListaMarcasVehiculosFicohsa_Result> list = this.db.help_F_ListaMarcasVehiculosFicohsa().ToList<help_F_ListaMarcasVehiculosFicohsa_Result>();
                var listaVehiculos = db.help_F_Api_Listar_Vehiculos_Marcas_Ficohsa().ToList();
                listaVehiculos.ForEach(item => this.Data.Add((object)new
                 {
                     CODIGO = item.CODMARCA,
                     DESCRIPCION = item.DESCMARCA,

                 }));

                return Ok(Data);

                    /*
                if (list.Count<help_F_ListaMarcasVehiculosFicohsa_Result>() <= 0)
                    return (IHttpActionResult)this.BadRequest("Este numero de atencion no es valido o no tiene imagines o firmas para este expediente.");
                list.ForEach((Action<help_F_ListaMarcasVehiculosFicohsa_Result>)(item => this.Data.Add((object)new
                {
                    CODIGO = item.CODMARCA,
                    DESCRIPCION = item.DESCMARCA
                })));
                return (IHttpActionResult)this.Ok<List<object>>(this.Data);
                */
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? (IHttpActionResult)this.BadRequest(ex.InnerException.Message) : (IHttpActionResult)this.BadRequest(ex.Message);
            }
        }



        [HttpGet]
        [Route("ObtenerCausasPorCobertura")]
        public IHttpActionResult ObtenerCausasPorCobertura(string codigoCobertura)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(codigoCobertura))
                {
                    return BadRequest("Debe enviar el codigo de cobertura.");
                }

                var causas = db.Database.SqlQuery<CausaPorCoberturaData>(
                    @"SELECT Id,
                             COD_PRODUCTO,
                             DESCRIPCION_PROD,
                             COD_COBERTURA,
                             DESCRIPCION_COBE,
                             COD_CAUSA,
                             DESCRIPCION_CAUS
                      FROM dbo.help_F_GetCausasPorCobertura(@codigoCobertura)",
                    new SqlParameter("@codigoCobertura", codigoCobertura)
                ).ToList();

                return Ok(causas);
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? (IHttpActionResult)this.BadRequest(ex.InnerException.Message) : (IHttpActionResult)this.BadRequest(ex.Message);
            }
        }



        [HttpGet]
        [Route("ListadoAjustadores")] // /api/Proveedor/ListadoAjustadores/
        public IHttpActionResult ListadoAjustadores(int IdPais) {
            try
            {
                var listaAjustadores = db.help_F_Api_ListadoAjustadores(IdPais).ToList();
                listaAjustadores.ForEach(item => {
                    Data.Add(new {
                        IdAjustador = item.IdAjustador,
                        AgenteNombre = item.AgenteNombre,
                        IdProveedor = item.IdProveedor,
                        Paid = item.Pais
                    });
                });

                return Ok(Data);

            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }

            }

        }

        [HttpGet]
        [Route("ListadoAbogados")] // /api/Proveedor/ListadoAjustadores/
        public IHttpActionResult ListadoAbogados(int IdPais)
        {
            try
            {
                var listaAjustadores = db.help_F_Api_ListadoAbogados(IdPais).ToList();
                listaAjustadores.ForEach(item => {
                    Data.Add(new
                    {
                        IdAjustador = item.IdAbogado,
                        AgenteNombre = item.AbogadoNombre,
                        IdProveedor = item.IdProveedor,
                        Paid = item.Pais
                    });
                });

                return Ok(Data);

            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }

            }

        }

        [HttpGet]
        [Route("ObtenerMisAtenciones")] // /api/Proveedor/ObtenerMisAtenciones/
        public IHttpActionResult ObtenerMisAtenciones(int IdProveedorAgente) {

            try
            {
                if (IdProveedorAgente == 0)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                List<object> Data = new List<object>();

                var misAtenciones = db.help_F_Api_Listar_Mis_Atenciones(IdProveedorAgente).OrderBy(x => x.FechaInicio).ToList();

                if (misAtenciones.Count() > 0) {

                    misAtenciones.ForEach(item => {
                        Data.Add(new
                        {
                            IdAtencion = item.Id,
                            Servicio = item.LbTipoServicio,
                            Categoria = item.LbCategoriaServicio,
                            Proveedor = item.NombreProveedor,
                            Zona = item.Zona,
                            Direccion = item.Direccion,
                            DireccionDestino = item.DireccionDestino,
                            Convenio = item.LbConvenio,
                            Ciudad = item.Ciudad,
                            Colonia = item.Colonia,
                            Cliente = item.Cliente,
                            Poliza = item.Poliza,
                            Plan = item.Plan,
                            UsurioRegistro = item.UsuarioRegistroNombre,
                            Fecha = item.FechaInicio,
                            Estado = item.LbEstado,
                            ColorEstado = item.ColorEstado
                        });
                    });

                    return Ok(Data);
                }
                else {

                    return BadRequest("El codigo que ingreso no tiene un proveedor agente registrado en los expedientes.");
                }


            }
            catch (Exception Ex)
            {


                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }

        }


        [HttpGet]
        [Route("ObtenerMisAtencionesActivas")] // /api/Proveedor/ObtenerMisAtencionesActivas/
        public IHttpActionResult ObtenerMisAtencionesActivas(int IdProveedorAgente)
        {

            try
            {
                if (IdProveedorAgente == 0)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                List<object> Data = new List<object>();

                var misAtenciones = db.help_F_Api_Listar_Mis_Atenciones_Activas(IdProveedorAgente).OrderBy(x => x.FechaInicio).ToList();

                if (misAtenciones.Count() > 0)
                {
                    EnviarNotificacionesAtencionesNuevas(IdProveedorAgente, misAtenciones);

                    misAtenciones.ForEach(item => {
                        Data.Add(new
                        {
                            IdAtencion = item.Id,
                            Servicio = item.LbTipoServicio,
                            Categoria = item.LbCategoriaServicio,
                            Proveedor = item.NombreProveedor,
                            Zona = item.Zona,
                            Direccion = item.Direccion,
                            DireccionDestino = item.DireccionDestino,
                            Convenio = item.LbConvenio,
                            Ciudad = item.Ciudad,
                            Colonia = item.Colonia,
                            Cliente = item.Cliente,
                            Poliza = item.Poliza,
                            Plan = item.Plan,
                            UsurioRegistro = item.UsuarioRegistroNombre,
                            Fecha = item.FechaInicio,
                            Estado = item.LbEstado,
                            ColorEstado = item.ColorEstado,
                            LogEstado = item.LogEstado
                        });
                    });

                    return Ok(Data);
                }
                else
                {

                    return BadRequest("El codigo que ingreso no tiene un proveedor agente registrado en los expedientes.");
                }


            }
            catch (Exception Ex)
            {


                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }

        }

        private void EnviarNotificacionesAtencionesNuevas(
            int idProveedorAgente,
            IEnumerable<help_F_Api_Listar_Mis_Atenciones_Activas_Result> atenciones)
        {
            try
            {
                foreach (var atencion in atenciones.Where(x => x.LogEstado == 1))
                {
                    var mensaje = String.Format(
                        "Le ha sido asignada la atención {0} para {1} a nombre de {2}",
                        atencion.Id,
                        atencion.LbTipoServicio,
                        atencion.Cliente);

                    EnviarNotificacionUnaVez(
                        atencion.Id,
                        idProveedorAgente,
                        "nueva_atencion",
                        "Nueva atención asignada",
                        mensaje,
                        new Dictionary<string, object>
                        {
                            { "event", "nueva_atencion" },
                            { "idAtencion", atencion.Id },
                            { "servicio", atencion.LbTipoServicio },
                            { "cliente", atencion.Cliente }
                        });
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Error al preparar pushes de atenciones nuevas: {0}", ex.Message);
            }
        }

        private void EnviarNotificacionUnaVez(
            int idAtencion,
            int idProveedorAgente,
            string evento,
            string titulo,
            string mensaje,
            Dictionary<string, object> data)
        {
            try
            {
                using (var notificationDb = new helpEntities())
                {
                    var agente = notificationDb.help_ProveedorAgentes.Find(idProveedorAgente);
                    if (agente == null || String.IsNullOrWhiteSpace(agente.TokenPush))
                    {
                        return;
                    }

                    var registro = new help_NotificacionesPush
                    {
                        IdProveedorAgente = idProveedorAgente,
                        IdAtencion = idAtencion,
                        Evento = evento,
                        Estado = "Pending",
                        Intentos = 1,
                        FechaCreacion = DateTime.Now,
                        FechaUltimoIntento = DateTime.Now
                    };

                    notificationDb.help_NotificacionesPush.Add(registro);

                    try
                    {
                        notificationDb.SaveChanges();
                    }
                    catch (DbUpdateException ex)
                    {
                        if (EsRegistroNotificacionDuplicado(ex))
                        {
                            return;
                        }

                        Trace.TraceWarning(
                            "No se pudo registrar la notificación {0} de la atención {1}: {2}",
                            evento,
                            idAtencion,
                            ex.GetBaseException().Message);
                        return;
                    }

                    try
                    {
                        var pushService = new OneSignalPushService();
                        var resultado = pushService.SendToPlayer(
                            agente.TokenPush,
                            titulo,
                            mensaje,
                            data);

                        if (resultado.Success)
                        {
                            registro.Estado = "Sent";
                            registro.FechaEnvio = DateTime.Now;
                            registro.UltimoError = null;
                        }
                        else
                        {
                            registro.Estado = "Failed";
                            registro.UltimoError = LimitarTexto(resultado.Body, 1000);
                            Trace.TraceWarning(
                                "OneSignal {0} de la atención {1} no enviada. StatusCode: {2}",
                                evento,
                                idAtencion,
                                resultado.StatusCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        registro.Estado = "Failed";
                        registro.UltimoError = LimitarTexto(ex.Message, 1000);
                        Trace.TraceWarning(
                            "Error al enviar push {0} de la atención {1}: {2}",
                            evento,
                            idAtencion,
                            ex.Message);
                    }

                    try
                    {
                        notificationDb.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning(
                            "No se pudo actualizar el estado de la notificación {0}: {1}",
                            registro.Id,
                            ex.GetBaseException().Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning(
                    "Error al preparar push {0} de la atención {1}: {2}",
                    evento,
                    idAtencion,
                    ex.Message);
            }
        }

        private static bool EsRegistroNotificacionDuplicado(DbUpdateException ex)
        {
            var sqlException = ex.GetBaseException() as SqlException;
            return sqlException != null &&
                   (sqlException.Number == 2601 || sqlException.Number == 2627);
        }

        private static string LimitarTexto(string texto, int longitudMaxima)
        {
            if (String.IsNullOrEmpty(texto) || texto.Length <= longitudMaxima)
            {
                return texto;
            }

            return texto.Substring(0, longitudMaxima);
        }

        [HttpPost]
        [Route("EliminaDanioExtra")]
        public IHttpActionResult EliminaDanioExtra(int Id)
        {
            try
            {
                this.db.help_P_Api_EliminaDanioExtra(new int?(Id));
                this.Data.Add((object)new
                {
                    Respuesta = 1,
                    Mensaje = "Tipo reparación eliminado"
                });
                return (IHttpActionResult)this.Ok<List<object>>(this.Data);
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? (IHttpActionResult)this.BadRequest(ex.InnerException.Message) : (IHttpActionResult)this.BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ActualizarIndexFront")]
        public IHttpActionResult ActualizarIndexFront(int Id, int indexFront)
        {
            try
            {
                this.db.help_P_Api_ActualizarIndexFront(new int?(Id), new int?(indexFront));
                this.Data.Add((object)new
                {
                    Respuesta = 1,
                    Mensaje = "Index actualizado"
                });
                return (IHttpActionResult)this.Ok<List<object>>(this.Data);
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? (IHttpActionResult)this.BadRequest(ex.InnerException.Message) : (IHttpActionResult)this.BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ActualizarAudicion")]
        public IHttpActionResult ActualizarAudicion(int IdAjustadorAudiencia, int RefProveedorAgenteAbogadoId, int AgendarAudiencia, DateTime FechaHora, string Fecha, string Hora, string Lugar, int idAgente)
        {
            try
            {
                var dateString = Fecha +" "+ Hora;

                DateTime dateTime = DateTime.Parse(dateString);

                this.db.help_P_Api_Datos_De_Agendamiento_Audiencia(
                    IdAjustadorAudiencia, 
                    RefProveedorAgenteAbogadoId,
                    AgendarAudiencia, 
                    dateTime, 
                    Lugar
                    );

                int IdAtencion = db.Database.SqlQuery<int>(
                    @"SELECT TOP 1 ISNULL(RefAtencionId, 0)
                      FROM dbo.help_AjustadorAudiencia
                      WHERE Id = @IdAjustadorAudiencia",
                    new SqlParameter("@IdAjustadorAudiencia", IdAjustadorAudiencia)
                ).FirstOrDefault();

                if (IdAtencion <= 0)
                {
                    return BadRequest("No se pudo obtener la atención asociada al agendamiento de audiencia.");
                }

                SendToMail sendToMail = new SendToMail();

                sendToMail.Email_NotificacionAccidenteConAudiencia(IdAtencion, idAgente, Fecha, Hora);

                if (AgendarAudiencia == 1)
                {
                    if (idAgente > 0)
                    {
                        var abogadoNombre = db.help_ProveedorAgentes
                            .Where(x => x.Id == RefProveedorAgenteAbogadoId)
                            .Select(x => x.NombreAgente)
                            .FirstOrDefault();

                        if (String.IsNullOrWhiteSpace(abogadoNombre))
                        {
                            abogadoNombre = "el abogado asignado";
                        }

                        var fechaFormateada = dateTime.ToString(
                            "dddd, d 'de' MMMM 'de' yyyy 'a las' h:mm tt",
                            new CultureInfo("es-HN"));

                        var mensajePush = String.Format(
                            "La audiencia de la atención {0} fue agendada para {1}, con fecha {2}",
                            IdAtencion,
                            abogadoNombre,
                            fechaFormateada);

                        EnviarNotificacionUnaVez(
                            IdAtencion,
                            idAgente,
                            "audiencia_agendada",
                            "Audiencia agendada",
                            mensajePush,
                            new Dictionary<string, object>
                            {
                                { "event", "audiencia_agendada" },
                                { "idAtencion", IdAtencion },
                                { "fecha", Fecha },
                                { "hora", Hora },
                                { "lugar", Lugar },
                                { "abogadoNombre", abogadoNombre }
                            });
                    }
                    else
                    {
                        Trace.TraceWarning(
                            "No se recibió agente válido para notificar la audiencia {0} de la atención {1}.",
                            IdAjustadorAudiencia,
                            IdAtencion);
                    }
                }
                // this.db.help_P_Api_ActualizarIndexFront(new int?(Id), new int?(indexFront));
                this.Data.Add((object)new
                {
                    Respuesta = 1,
                    Mensaje = "Audicion actualizada"
                });               

                return (IHttpActionResult)this.Ok<List<object>>(this.Data);
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? (IHttpActionResult)this.BadRequest(ex.InnerException.Message) : (IHttpActionResult)this.BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("InsertarConvenioReparacionTallerExtra")]
        public IHttpActionResult InsertarConvenioReparacionTallerExtra(
      ApiData.DanioOtroData DanioOtroData)
        {
            try
            {
                if (DanioOtroData == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                ObjectParameter respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter mensaje = new ObjectParameter("mensaje", typeof(string));
                this.db.help_P_Api_CrearOtroDanio_Ficohsa(new int?(DanioOtroData.RefAtencionId), DanioOtroData.DescripcionDeDanio, new DateTime?(DanioOtroData.FechaRegistro), new int?(DanioOtroData.UsuarioId), DanioOtroData.TipoEntidad, new short?(DanioOtroData.TipoReparacion), DanioOtroData.CodigoDanioVehiculo, new int?(DanioOtroData.indexFront), respuesta, mensaje);
                return Convert.ToInt32(respuesta.Value) > 0 ? (IHttpActionResult)this.Ok<string>("Danio de convenio de reparacion ingresado con exito.") : (IHttpActionResult)this.BadRequest(mensaje.Value.ToString());
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? (IHttpActionResult)this.BadRequest(ex.InnerException.Message) : (IHttpActionResult)this.BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ObtenerDaniosExtras")]
        public IHttpActionResult ObtenerDaniosExtras(int RefAtencionId, string TipoEntidad)
        {
            try
            {
                ObjectParameter objectParameter1 = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter objectParameter2 = new ObjectParameter("mensaje", typeof(string));
                List<help_OtrosDanio_Ficohsa> list = ((IQueryable<help_OtrosDanio_Ficohsa>)this.db.help_OtrosDanio_Ficohsa).Where<help_OtrosDanio_Ficohsa>((Expression<Func<help_OtrosDanio_Ficohsa, bool>>)(x => x.RefAtencionId == (int?)RefAtencionId && x.TipoEntidad == TipoEntidad)).ToList<help_OtrosDanio_Ficohsa>();
                return list.Count<help_OtrosDanio_Ficohsa>() > 0 ? (IHttpActionResult)this.Ok<List<help_OtrosDanio_Ficohsa>>(list) : (IHttpActionResult)this.BadRequest("La atencion no tiene un extras registrados, por favor complete el formulario y súbalo.");
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? (IHttpActionResult)this.BadRequest(ex.InnerException.Message) : (IHttpActionResult)this.BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("ActualizarTipoReparacion")]
        public IHttpActionResult ActualizarTipoReparacion(int Id, short Tipo, int indexFront)
        {
            try
            {
                this.db.help_P_Api_ActualizarTipoReparacion(new int?(Id), new short?(Tipo), new int?(indexFront));
                this.Data.Add((object)new
                {
                    Respuesta = 1,
                    Mensaje = "Tipo reparación actualizado"
                });
                return (IHttpActionResult)this.Ok<List<object>>(this.Data);
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? (IHttpActionResult)this.BadRequest(ex.InnerException.Message) : (IHttpActionResult)this.BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("InsertarFiniquitoManual")]
        public IHttpActionResult InsertarFiniquitoManual(ApiData.help_FiniquitoManual Finiquito)
        {
            try
            {
                if (Finiquito == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                ObjectParameter respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter mensaje = new ObjectParameter("mensaje", typeof(string));
                byte[] firmaCliente = Convert.FromBase64String(Finiquito.FirmaCliente);
                this.db.help_P_Api_InsertFiniquito_Ficohsa(Finiquito.NumeroReclamo, Finiquito.FechaDesde, Finiquito.FechaHasta, Finiquito.NombreCliente, Finiquito.TipoCoberturaFicohsa, Finiquito.FechaFirma, firmaCliente, Finiquito.NombreQuienRecibe, Finiquito.IdentidadQuienRecibe, Finiquito.NumeroCheque, Finiquito.FechaDelCheque, Finiquito.ValorDelCheque, Finiquito.NombreAFavor, Finiquito.Poliza, Finiquito.RefAtencionId, Finiquito.Marca, Finiquito.Modelo, Finiquito.Anio, Finiquito.Placa, Finiquito.Chasis, Finiquito.Motor, respuesta, mensaje);
                return Convert.ToInt32(respuesta.Value) == 1 ? (IHttpActionResult)this.Ok<string>("Finiquito creado con exito.") : (IHttpActionResult)this.BadRequest(mensaje.Value.ToString());
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? (IHttpActionResult)this.BadRequest(ex.InnerException.Message) : (IHttpActionResult)this.BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("ObtenerDatosExpedientes")] // /api/Proveedor/ObtenerMisAtenciones/
        public IHttpActionResult ObtenerDatosExpedientes(int IdAtencion)
        {
            try
            {
                if (IdAtencion == 0)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                List<object> Data = new List<object>();
                ObjectParameter Respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter Mensaje = new ObjectParameter("Mensaje", typeof(int));
                db.help_P_Api_Precarga_Ajustador_Audiencia_App(IdAtencion, Respuesta, Mensaje);

                //if (cambio == 0)
                //{
                var misAtenciones = db.help_F_Api_Datos_Expediente_Siniestro(IdAtencion).ToList();

                if (misAtenciones.Count() > 0)
                {
                    return Ok(misAtenciones);
                }
                else
                {
                    return BadRequest("El codigo que ingreso no tiene un proveedor agente registrado en los expedientes.  ");
                }

                //}
                //else {

                //    var misAtenciones = db.help_F_Api_Datos_Expediente_Siniestro_Menos_Datos(IdAtencion).ToList();

                //    if (misAtenciones.Count() > 0)
                //    {
                //        return Ok(misAtenciones);
                //    }
                //    else
                //    {
                //        return BadRequest("El codigo que ingreso no tiene un proveedor agente registrado en los expedientes.");
                //    }

                //}




            }
            catch (Exception Ex)
            {


                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }

        }

        /*
         Debug hecho por Omar McClellan y Manuel Palma
        Comentado en 80/03/2024 para agregar la precarga del ajustador a las atenciones temporalmente
        [HttpGet]
        [Route("ObtenerDatosExpedientes")] // /api/Proveedor/ObtenerMisAtenciones/
        public IHttpActionResult ObtenerDatosExpedientes(int IdAtencion)
        {
            try
            {
                if (IdAtencion == 0)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                List<object> Data = new List<object>();

                //if (cambio == 0)
                //{
                    var misAtenciones = db.help_F_Api_Datos_Expediente_Siniestro(IdAtencion).ToList();

                    if (misAtenciones.Count() > 0)
                    {
                        return Ok(misAtenciones);
                    }
                    else
                    {
                        return BadRequest("El codigo que ingreso no tiene un proveedor agente registrado en los expedientes.  ");
                    }

                //}
                //else {

                //    var misAtenciones = db.help_F_Api_Datos_Expediente_Siniestro_Menos_Datos(IdAtencion).ToList();

                //    if (misAtenciones.Count() > 0)
                //    {
                //        return Ok(misAtenciones);
                //    }
                //    else
                //    {
                //        return BadRequest("El codigo que ingreso no tiene un proveedor agente registrado en los expedientes.");
                //    }

                //}

                


            }
            catch (Exception Ex)
            {


                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }

        }

        */

        [HttpGet]
        [Route("ObtenerCacheCliente")] // /api/Proveedor/ObtenerCacheCliente/
        public IHttpActionResult ObtenerCacheCliente(int IdAtencion)
        {
            try
            {
                if (IdAtencion == 0)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                //var Variable = db.help_AjustadorAudiencia.Where(x => x.RefAtencionId == IdAtencion).ToList().FirstOrDefault();

                //if (Variable == null)
                //    throw new HttpResponseException(HttpStatusCode.BadRequest);


                //if (Variable.CodigoBPMFicohsa.Count() == 0)
                //{

                    var miCache = db.help_F_GetCacheCliente(IdAtencion).ToList();
                    var message = "La atención " + IdAtencion + " no tiene aún datos";

                    if (miCache.Count() > 0)
                    {
                        return Ok(miCache);
                    }
                    else
                    {
                        return Ok(message.ToString()); //return NotFound();
                    }

                //}
                //else { 
                  
                //    return BadRequest("Codigo BPM del expediente ya fue en enviado o creado.");
                
                //}
                   
                //if (cambio == 0)
                //{
               

            }
            catch (Exception Ex)
            {


                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }

        }

        [HttpGet]
        [Route("ObtenerDatosDeAtencion")] // /api/Proveedor/ObtenerDatosDeAtencion/
        public IHttpActionResult ObtenerDatosDeAtencion(int IdAtencion)
        {
            try
            {
                if (IdAtencion == 0)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                List<object> Data = new List<object>();

                //if (cambio == 0)
                //{
                var misAtenciones = db.help_F_Api_Lista_Mi_Atencion_FICOHSA_APP_Ajustadores(IdAtencion).ToList();

                if (misAtenciones.Count() > 0)
                {
                    return Ok(misAtenciones);
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception Ex)
            {


                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }

        }

        [HttpPost]
        [Route("GuardarInformeSiniestros")] // /api/Proveedor/ObtenerMisAtenciones/
        public IHttpActionResult GuardarInformeSiniestros(ApiData.help_AjustadorAudienciaRequest AjustadorAudiencia) {

            try
            {
                if (AjustadorAudiencia == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);


                ObjectParameter Respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter Mensaje = new ObjectParameter("mensaje", typeof(string));


                string NumeroReclamoFicohda = "";


                db.help_P_Api_Registrar_Siniestros_Ajustador_Cliente(AjustadorAudiencia.RefAtencionId,
                                                                     AjustadorAudiencia.RefProveedorAgenteId,
                                                                     null,
                                                                     AjustadorAudiencia.AgendarAudiencia,
                                                                     AjustadorAudiencia.AseguradoUsoPoliza,
                                                                     AjustadorAudiencia.TerceroResponsable,
                                                                     AjustadorAudiencia.LesionadosSinAudiencia,
                                                                     AjustadorAudiencia.DescripcionAudiencia,
                                                                     AjustadorAudiencia.RefEntidadComunicativaId,
                                                                     AjustadorAudiencia.RefTipoAudienciaId,
                                                                     AjustadorAudiencia.Poliza,
                                                                     AjustadorAudiencia.Identificacion,
                                                                     AjustadorAudiencia.Nombre,
                                                                     AjustadorAudiencia.ConductorAfiliado,
                                                                     AjustadorAudiencia.ConductorDetenido,
                                                                     AjustadorAudiencia.Descripcion,
                                                                     AjustadorAudiencia.MarcaVehiculo,
                                                                     AjustadorAudiencia.ModeloVehiculo,
                                                                     AjustadorAudiencia.AnioVehiculo,
                                                                     AjustadorAudiencia.PlacaVehiculo,
                                                                     AjustadorAudiencia.ChasisVehiculo,
                                                                     AjustadorAudiencia.ColorVehiculo,
                                                                     AjustadorAudiencia.VehiculoDetenido,
                                                                     AjustadorAudiencia.DescripcionVehiculo,
                                                                     AjustadorAudiencia.TercerosHeridos,
                                                                     AjustadorAudiencia.TercerosMuertos,
                                                                     AjustadorAudiencia.DescripcionTercerosHeridos,
                                                                     AjustadorAudiencia.DescripcionTercerosMuertos,
                                                                     AjustadorAudiencia.DanioFrontal,
                                                                     AjustadorAudiencia.DanioTrasero,
                                                                     AjustadorAudiencia.DanioLateralDerecho,
                                                                     AjustadorAudiencia.DanioLataralIzquierdo,
                                                                     AjustadorAudiencia.VehiculoVolcado,
                                                                     AjustadorAudiencia.DescripcionDanio,
                                                                     AjustadorAudiencia.RefPaisId,
                                                                     AjustadorAudiencia.RefCiudadId,
                                                                     AjustadorAudiencia.RefDeptoId,
                                                                     AjustadorAudiencia.RefMunicipioId,
                                                                     AjustadorAudiencia.FechaHora,
                                                                     AjustadorAudiencia.Lugar,
                                                                     AjustadorAudiencia.RefUsuarioId,
                                                                     DateTime.Now,
                                                                     AjustadorAudiencia.TallerMecanicoId,
                                                                     AjustadorAudiencia.Blindado,
                                                                     AjustadorAudiencia.ObservacionTaller,
                                                                     AjustadorAudiencia.ReclamoAsegurado,
                                                                     AjustadorAudiencia.Observaciones,
                                                                     AjustadorAudiencia.Latitud,
                                                                     AjustadorAudiencia.Longitud,
                                                                     AjustadorAudiencia.NombreConductor,
                                                                     AjustadorAudiencia.IdentidaConductor,
                                                                     AjustadorAudiencia.DPI_Pasaporte,
                                                                     AjustadorAudiencia.TelefonoConductor,
                                                                     AjustadorAudiencia.CelularConductor,
                                                                     AjustadorAudiencia.Edad,
                                                                     AjustadorAudiencia.Licencia,
                                                                     AjustadorAudiencia.TipoLicencia,
                                                                     AjustadorAudiencia.Vigencia,
                                                                     AjustadorAudiencia.DireccionConductor,
                                                                     AjustadorAudiencia.Sexo,
                                                                     AjustadorAudiencia.RefTipoConductorId,
                                                                     NumeroReclamoFicohda,
                                                                     AjustadorAudiencia.CorreoElectronico,
                                                                     AjustadorAudiencia.JSON_PropiedadDaniadaSiniestro,
                                                                     AjustadorAudiencia.JSON_TercerosSiniestro,
                                                                     Respuesta,
                                                                     Mensaje
                                                                     );

                return Ok(Convert.ToInt32(Respuesta.Value));
            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }

        [HttpPost]
        [Route("GuardarInformeSiniestros_HN")] // /api/Proveedor/ObtenerMisAtenciones/
        public IHttpActionResult GuardarInformeSiniestros_HN(ApiData.help_AjustadorAudienciaRequestHN AjustadorAudiencia)
        {

            try
            {
                if (AjustadorAudiencia == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);


                ObjectParameter R = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter M = new ObjectParameter("mensaje", typeof(string));


                string NumeroReclamoFicohda = "";

                db.help_P_Api_Registrar_Siniestros_Ajustador_Cliente_HN(AjustadorAudiencia.RefAtencionId, 
                                                                        AjustadorAudiencia.RefProveedorAgenteId,
                                                                        null, 
                                                                        AjustadorAudiencia.AgendarAudiencia, 
                                                                        AjustadorAudiencia.AseguradoUsoPoliza, 
                                                                        AjustadorAudiencia.TerceroResponsable, 
                                                                        AjustadorAudiencia.LesionadosSinAudiencia, 
                                                                        AjustadorAudiencia.DescripcionAudiencia, 
                                                                        AjustadorAudiencia.Poliza, 
                                                                        AjustadorAudiencia.Identificacion, 
                                                                        AjustadorAudiencia.Nombre, 
                                                                        AjustadorAudiencia.ConductorAfiliado, 
                                                                        AjustadorAudiencia.ConductorDetenido, 
                                                                        AjustadorAudiencia.Descripcion, 
                                                                        AjustadorAudiencia.MarcaVehiculo, 
                                                                        AjustadorAudiencia.ModeloVehiculo, 
                                                                        AjustadorAudiencia.AnioVehiculo, 
                                                                        AjustadorAudiencia.PlacaVehiculo, 
                                                                        AjustadorAudiencia.ChasisVehiculo, 
                                                                        AjustadorAudiencia.ColorVehiculo, 
                                                                        AjustadorAudiencia.VehiculoDetenido, 
                                                                        AjustadorAudiencia.DescripcionVehiculo, 
                                                                        AjustadorAudiencia.TercerosHeridos, 
                                                                        AjustadorAudiencia.TercerosMuertos, 
                                                                        AjustadorAudiencia.DescripcionTercerosHeridos, 
                                                                        AjustadorAudiencia.DescripcionTercerosMuertos, 
                                                                        AjustadorAudiencia.DanioFrontal, 
                                                                        AjustadorAudiencia.DanioTrasero, 
                                                                        AjustadorAudiencia.DanioLateralDerecho, 
                                                                        AjustadorAudiencia.DanioLataralIzquierdo, 
                                                                        AjustadorAudiencia.VehiculoVolcado, 
                                                                        AjustadorAudiencia.DescripcionDanio, 
                                                                        AjustadorAudiencia.RefPaisId, 
                                                                        AjustadorAudiencia.RefCiudadId, 
                                                                        AjustadorAudiencia.RefDeptoId, 
                                                                        AjustadorAudiencia.RefMunicipioId, 
                                                                        AjustadorAudiencia.FechaHora, 
                                                                        AjustadorAudiencia.Lugar, 
                                                                        AjustadorAudiencia.RefProveedorAgenteId,
                                                                        DateTime.Now, 
                                                                        AjustadorAudiencia.TallerMecanicoId, 
                                                                        AjustadorAudiencia.Blindado, 
                                                                        AjustadorAudiencia.ObservacionTaller, 
                                                                        AjustadorAudiencia.ReclamoAsegurado, 
                                                                        AjustadorAudiencia.Observaciones, 
                                                                        AjustadorAudiencia.Latitud, 
                                                                        AjustadorAudiencia.Longitud, 
                                                                        AjustadorAudiencia.NombreConductor, 
                                                                        AjustadorAudiencia.IdentidaConductor, 
                                                                        AjustadorAudiencia.DPI_Pasaporte,
                                                                        AjustadorAudiencia.TelefonoConductor, 
                                                                        AjustadorAudiencia.CelularConductor, 
                                                                        AjustadorAudiencia.Edad, 
                                                                        AjustadorAudiencia.Licencia, 
                                                                        AjustadorAudiencia.TipoLicencia, 
                                                                        AjustadorAudiencia.Vigencia, 
                                                                        AjustadorAudiencia.DireccionConductor, 
                                                                        AjustadorAudiencia.Sexo, 
                                                                        AjustadorAudiencia.RefTipoConductorId,
                                                                        NumeroReclamoFicohda,
                                                                        AjustadorAudiencia.DireccionEnvioCorrespondencia,
                                                                        AjustadorAudiencia.CorreoElectronico,
                                                                        AjustadorAudiencia.RefTipoConductorId,
                                                                        AjustadorAudiencia.RefTipoLicenciaId,
                                                                        AjustadorAudiencia.NombreAtribuyeAccidente,
                                                                        AjustadorAudiencia.AutoridadInvolucrada,
                                                                        AjustadorAudiencia.UbicacionVehiculoDetenido,
                                                                        AjustadorAudiencia.PruebaAlcoholemia,
                                                                        AjustadorAudiencia.RefTipoCombustibleId,
                                                                        AjustadorAudiencia.AC,
                                                                        AjustadorAudiencia.Rines,
                                                                        AjustadorAudiencia.BolsaAire,
                                                                        AjustadorAudiencia.CierreCentralizado,
                                                                        AjustadorAudiencia.Mecanico,
                                                                        AjustadorAudiencia.RetrovisorElectronico,
                                                                        AjustadorAudiencia.Overfenders,
                                                                        AjustadorAudiencia.ColaPato,
                                                                        AjustadorAudiencia.CintaDecorativa,
                                                                        AjustadorAudiencia.LicenciaEstado,
                                                                        AjustadorAudiencia.RefTipoSolicitanteInformeAjusteId,
                                                                        AjustadorAudiencia.ObservacionesFinales,
                                                                        AjustadorAudiencia.TipoAcuerdoFicohsa,
                                                                        AjustadorAudiencia.DondeSeEncuentraVehiculo,
                                                                        AjustadorAudiencia.NumeroUnidad,
                                                                        AjustadorAudiencia.Parentesco,
                                                                        AjustadorAudiencia.FechaNacimientoConductor,
                                                                        AjustadorAudiencia.CulpableCompromisoPago,
                                                                        AjustadorAudiencia.ObservacionCompromisoPago,
                                                                        AjustadorAudiencia.PorqueNoUsoServicioAsistencia,
                                                                        AjustadorAudiencia.Kilometraje,
                                                                        AjustadorAudiencia.OtrosTalleres,
                                                                        R,
                                                                        M
                                                                        );



                return Ok(Convert.ToInt32(R.Value));
            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }

        [HttpPost]
        [Route("GuardarCacheCliente")] // /api/Proveedor/GuardarCacheCliente/
        public IHttpActionResult GuardarCacheCliente(ApiData.help_CacheCliente Data)
        {

            try
            {

                if (Data.IdAtencion == null)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }



                db.help_P_Insert_Cache_Cliente(Data.IdAtencion, Data.Nombre, Data.AutoridadInvolucrada, Data.NombreConductor, Data.IdentidaConductor, Data.Poliza,
                    Data.NombreAtribuyeAccidente, Data.TelefonoConductor, Data.CelularConductor, Data.CorreoElectronico, Data.Licencia, Data.Blindado, Data.FechaNacimientoConductor,
                    Data.Edad, Data.AseguradoUsoPoliza, Data.PorqueNoUsoServicioAsistencia, Data.PruebaAlcoholemia, Data.AgendarAudiencia, Data.VehiculoDetenido,
                    Data.ConductorDetenido, Data.VehiculoVolcado, Data.DireccionEnvioCorrespondencia, Data.DireccionConductor, Data.DondeSeEncuentraVehiculo, 
                    Data.TerceroResponsable, Data.UbicacionVehiculoDetenido, Data.DescripcionAudiencia, Data.LicenciaEstado, Data.Vigencia, Data.RefTipoLicenciaId,
                    Data.TipoLicencia, Data.NumeroUnidad, Data.DescripcionTercerosMuertos, Data.DescripcionTercerosHeridos, Data.Descripcion, Data.DescripcionDanio, Data.RefTipoConductorId,
                    Data.TercerosMuertos, Data.TercerosHeridos, Data.Parentesco, Data.Latitud, Data.Longitud, Data.AnioVehiculo, Data.ModeloVehiculo, Data.PlacaVehiculo, 
                    Data.ColorVehiculo, Data.MarcaVehiculo, Data.ChasisVehiculo, Data.Motor);
                return Ok("Cache guardada con exito.");
            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }

        [HttpPost]
        [Route("GuardarIdTablaAjustador")] // /api/Proveedor/GuardarIdTablaAjustador/
        public IHttpActionResult GuardarIdTablaAjustador(int IdAtencion, int IdTablaAjustador)
        {

            try
            {
                if (IdAtencion == null) { 
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                db.help_P_Insert_Id_Tabla_Ajuste(IdAtencion, IdTablaAjustador);
                return Ok("Id guardada con exito.");
            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }


        [HttpPost]
        [Route("ActualizaLogAtencion")] // /api/Proveedor/ActualizaLogAtencion/
        public IHttpActionResult ActualizaLogAtencion(int IdAtencion)
        {

            try
            {
                if (IdAtencion == null)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                ObjectParameter Respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter Mensaje = new ObjectParameter("mensaje", typeof(string));
                Mensaje.Value = "Atencián Actualizada con éxito";
                db.help_P_ActualizaEstadoLogAtencion(IdAtencion);

                return Ok(Mensaje.Value.ToString());

            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }

        

        [HttpPost]
        [Route("ObtenerIdTablaAjustador")] // /api/Proveedor/ObtenerIdTablaAjustador/
        public IHttpActionResult ObtenerIdTablaAjustador(int IdAtencion)
        {

            try
            {
                if (IdAtencion == null)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                ObjectParameter Respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter Mensaje = new ObjectParameter("mensaje", typeof(string));

                var idTablaQuery = db.help_F_GetIdTablaAjustadorAtencion(IdAtencion).ToList();
                int? IdAgente = db.Database.SqlQuery<int?>(
                    @"SELECT TOP 1 RefProveedorAgenteId
                      FROM dbo.help_AjustadorAudiencia
                      WHERE RefAtencionId = @IdAtencion
                      ORDER BY Id DESC",
                    new SqlParameter("@IdAtencion", IdAtencion)
                ).FirstOrDefault();

                idTablaQuery.ForEach(item =>
                {
                    Data.Add(new
                    {
                        IdAudiencia = item.idTablaAjustador,
                        IdAgente = IdAgente
                    });
                });


                return Ok(Data);
                /*
                

                
                if (IdAgente != null)
                {
                    Mensaje.Value = IdAgente;
                }
                else
                {
                    Mensaje.Value = 0;
                }

               

                return Ok(Mensaje.Value.ToString());
                */

            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }
        /**/


        [HttpPost]
        [Route("GuardaInformerAjustador_HN")] // /api/Proveedor/ObtenerMisAtenciones/
        public IHttpActionResult GuardaInformerAjustador_HN(ApiData.help_AjustadorInformeAjustadortHN AjustadorAudiencia)
        {

            try
            {
                if (AjustadorAudiencia == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);


                ObjectParameter R = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter M = new ObjectParameter("mensaje", typeof(string));


                db.help_P_Api_Registrar_Datos_Ajustador_Cliente_HN(AjustadorAudiencia.AC, 
                                                                   AjustadorAudiencia.BolsaAire, 
                                                                   AjustadorAudiencia.CierreCentralizado,
                                                                   AjustadorAudiencia.CintaDecorativa,
                                                                   AjustadorAudiencia.ColaPato,
                                                                   AjustadorAudiencia.CompromisoPago,
                                                                   AjustadorAudiencia.DPI_Pasaporte,
                                                                   AjustadorAudiencia.DanioFrontal,
                                                                   AjustadorAudiencia.DanioTrasero,
                                                                   AjustadorAudiencia.DanioLateralDerecho,
                                                                   AjustadorAudiencia.DanioLataralIzquierdo,
                                                                   AjustadorAudiencia.VehiculoVolcado,
                                                                   AjustadorAudiencia.Descripcion,
                                                                   AjustadorAudiencia.DescripcionDanio,
                                                                   null,
                                                                   AjustadorAudiencia.Identificacion,
                                                                   AjustadorAudiencia.Kilometraje,
                                                                   AjustadorAudiencia.Mecanico,
                                                                   AjustadorAudiencia.ObservacionCompromisoPago,
                                                                   AjustadorAudiencia.Observaciones,
                                                                   AjustadorAudiencia.ObservacionesFinales,
                                                                   AjustadorAudiencia.ObservacionTaller,
                                                                   AjustadorAudiencia.Overfenders,
                                                                   AjustadorAudiencia.Parentesco,
                                                                   AjustadorAudiencia.RefAtencionId,
                                                                   AjustadorAudiencia.RefProveedorAgenteId,
                                                                   AjustadorAudiencia.RefTipoCombustibleId,
                                                                   AjustadorAudiencia.RefTipoSolicitanteInformeAjusteId,
                                                                   AjustadorAudiencia.RetrovisorElectronico,
                                                                   AjustadorAudiencia.Rines,
                                                                   AjustadorAudiencia.Sexo,
                                                                   AjustadorAudiencia.TallerMecanicoId,
                                                                   AjustadorAudiencia.TipoAcuerdoFicohsa,
                                                                   AjustadorAudiencia.Blindado,
                                                                   R,
                                                                   M
                                                                   );



                return Ok(Convert.ToInt32(R.Value));
            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }

        [HttpPost]
        [Route("GuardarTercerosSiniestro")] // /api/Proveedor/GuardarTercerosSiniestro/
        public IHttpActionResult GuardarTercerosSiniestro(List<ApiData.help_TercerosSiniestros> tercerosiniestro)
        {

            try
            {
                if (tercerosiniestro == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);


                ObjectParameter Respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter Mensaje = new ObjectParameter("mensaje", typeof(string));

                foreach (var item in tercerosiniestro)
                {
                    db.help_P_Api_GuardarTercerosSiniestros(item.RefIdAjustadorAudiencia,
                                                       item.NombrePropetario,
                                                       item.NombrePiloto,
                                                       item.Marca,
                                                       item.Modelo,
                                                       item.Placa,
                                                       item.Chasis,
                                                       item.Telefono,
                                                       item.DaniosObservacion,
                                                       item.RefAgenteProveedorId,
                                                       Respuesta,
                                                       Mensaje);
                }
               


                

                return Ok("Tercero Sinisestro guardado con exito.");
            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }

        [HttpPost]
        [Route("GuardarPropiedadDaniadaSiniestro")] // /api/Proveedor/GuardarPropiedadDaniadaSiniestro/
        public IHttpActionResult GuardarPropiedadDaniadaSiniestro(List<ApiData.help_PropiedadDaniadaSiniestro> propiedaddaniada)
        {
            try
            {
                if (propiedaddaniada == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);


                ObjectParameter Respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter Mensaje = new ObjectParameter("mensaje", typeof(string));

                foreach (var item in propiedaddaniada)
                {
                    db.help_P_Api_GuardarPropiedadDaniadaSiniestro(item.RefAjustadorAudienciaId,
                                                              item.Nombre,
                                                              item.Direccion,
                                                              item.DaniosPrivado,
                                                              DateTime.Now,
                                                              item.RefAjustadorId,
                                                              Respuesta,
                                                              Mensaje);
                }
               

                return Ok("Tercero Sinisestro guardado con exito.");
            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }

        [HttpPost]
        [Route("GuardarPropiedadDaniadaSiniestro_HN")] // /api/Proveedor/GuardarPropiedadDaniadaSiniestro_HN/
        public IHttpActionResult GuardarPropiedadDaniadaSiniestro_HN(List<ApiData.help_PropiedadDaniadaSiniestro> propiedaddaniada)
        {
            try
            {
                if (propiedaddaniada == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                string error1 = null;
                ObjectParameter Respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter Mensaje = new ObjectParameter("mensaje", typeof(string));

                foreach (var item in propiedaddaniada)
                {


                    db.help_P_Api_Insert_PropiedadDaniadaSiniestro_HN(item.RefAjustadorAudienciaId,
                                                              item.Nombre,
                                                              item.Direccion,
                                                              item.DaniosPrivado,
                                                              DateTime.Now,
                                                              item.RefAjustadorId,
                                                              item.Telefono,
                                                              item.DescripcionDelBien,
                                                              item.DireccionDelBien,
                                                              item.DescripcionDelDanio,
                                                              item.NombreDelBienAfectado,
                                                              Respuesta,
                                                              Mensaje);

                    if (Convert.ToInt32(Respuesta.Value) == 1)
                    {
                        
                        error1 = "Success";
                    }
                    else
                    {

                        error1 = "ERROR: " + Mensaje.Value.ToString();
                        break;
                    }

                }

                return Ok(error1);

                
            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }


        [HttpPost]
        [Route("GuardarTercerosReclamo")] // /api/Proveedor/GuardarTercerosReclamo/
        public IHttpActionResult GuardarTercerosReclamo(ApiData.help_TercerosReclamo terceroreclamo)
        {

            try
            {
                if (terceroreclamo == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);


                ObjectParameter Respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter Mensaje = new ObjectParameter("mensaje", typeof(string));


                db.help_P_Api_GuardarTercerosReclamo(terceroreclamo.RefAjustadorAudienciaId, 
                                                     terceroreclamo.NombreCompleto, 
                                                     terceroreclamo.Identificacion, 
                                                     terceroreclamo.DPI_Pasaporte, 
                                                     terceroreclamo.Telefono, 
                                                     terceroreclamo.Celular, 
                                                     terceroreclamo.Vigencia, 
                                                     terceroreclamo.TipoLicencia, 
                                                     terceroreclamo.Licencia, 
                                                     terceroreclamo.Edad, 
                                                     terceroreclamo.Sexo, 
                                                     terceroreclamo.Culpalble, 
                                                     terceroreclamo.Afectado, 
                                                     terceroreclamo.Marca, 
                                                     terceroreclamo.Modelo, 
                                                     terceroreclamo.TipoVehiculo, 
                                                     terceroreclamo.Anio, 
                                                     terceroreclamo.Placa, 
                                                     terceroreclamo.Color, 
                                                     terceroreclamo.Chasis, 
                                                     terceroreclamo.Motor, 
                                                     terceroreclamo.RefAgenteProveedorId, 
                                                     terceroreclamo.TipoTercero,
                                                     terceroreclamo.Direccion,
                                                     Respuesta, 
                                                     Mensaje);




                return Ok("Tercero Reclamo guardado con exito.");
            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }


        [HttpPost]
        [Route("ActualizarCordenadasAjustador")] // /api/Proveedor/ActualizarCordenadasAjustador/
        public IHttpActionResult ActualizarCordenadasAjustador(double? Longitud,double? Latitud,int IdProveedorAgente)
        {

            try
            {
                if (Longitud == null || Latitud == null || IdProveedorAgente == 0)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);


                ObjectParameter Respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter Mensaje = new ObjectParameter("mensaje", typeof(string));


                db.help_P_Api_Actualizar_Coordenadas_Ajustador(Latitud,
                                                     Longitud,
                                                     IdProveedorAgente,
                                                     Respuesta,
                                                     Mensaje);




                return Ok("Coordenadas Ingresadas con exito.");
            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }


        [HttpPost]
        [Route("SubirFotosSiniestro")]
        public IHttpActionResult SubirFotosSiniestro([FromBody] List<FotoSiniestroRequest> Fotos) {
            try
            {
                if (Fotos == null || !Fotos.Any())
                    return BadRequest("Ingrese al menos una fotografia.");

                foreach (var item in Fotos)
                {
                    if (String.IsNullOrWhiteSpace(item.Foto) || item.IdAtencion == 0)
                    {
                        return BadRequest("Ingrese una imagen y un numero de atencion validos.");
                    }

                    var atencion = db.help_Atenciones.Find(item.IdAtencion);
                    if (atencion == null)
                    {
                        return BadRequest("No existe la atencion indicada: " + item.IdAtencion);
                    }

                    byte[] FotoData = System.Convert.FromBase64String(ObtenerBase64Limpio(item.Foto));

                    help_Atenciones_Fotografias AtencionFoto = new help_Atenciones_Fotografias();
                    AtencionFoto.Fotografia = FotoData;
                    AtencionFoto.NombreFoto = "Foto" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + item.IdAtencion;
                    AtencionFoto.TipoFoto = "image/png";
                    AtencionFoto.RefUsuarioId = 1;
                    AtencionFoto.FechaRegistro = DateTime.Now;
                    AtencionFoto.help_Atenciones = db.help_Atenciones.Find(item.IdAtencion);
                    AtencionFoto.RefTipoId = item.RefTipoFotoId;
                    AtencionFoto.Decripcion = item.Descripcion;
                    AtencionFoto.help_Atenciones = atencion;
                    db.help_Atenciones_Fotografias.Add(AtencionFoto);
                    db.SaveChanges();
                }

                return Ok("Capturas guardadas con exito.");

            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }

        [HttpPost]
        [Route("SubirFirmas")]
        public IHttpActionResult SubirFirmas([FromBody] List<FotoSiniestroRequest> Fotos)
        {
            try
            {
                if (Fotos == null || !Fotos.Any())
                    return BadRequest("Ingrese al menos una firma.");

                foreach (var item in Fotos)
                {
                    if (String.IsNullOrWhiteSpace(item.Foto) || item.IdAtencion == 0)
                    {
                        return BadRequest("Ingrese una imagen y un numero de atencion validos.");
                    }

                    var atencion = db.help_Atenciones.Find(item.IdAtencion);
                    if (atencion == null)
                    {
                        return BadRequest("No existe la atencion indicada: " + item.IdAtencion);
                    }

                    byte[] FotoData = System.Convert.FromBase64String(ObtenerBase64Limpio(item.Foto));

                    help_Atenciones_Fotografias AtencionFoto = new help_Atenciones_Fotografias();
                    AtencionFoto.Fotografia = FotoData;
                    AtencionFoto.NombreFoto = "Foto" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + item.IdAtencion;
                    AtencionFoto.TipoFoto = "image/png";
                    AtencionFoto.RefUsuarioId = 1;
                    AtencionFoto.FechaRegistro = DateTime.Now;
                    AtencionFoto.help_Atenciones = db.help_Atenciones.Find(item.IdAtencion);
                    AtencionFoto.RefTipoId = item.RefTipoFotoId;
                    AtencionFoto.NombreFirmante = item.NombreFirmante;
                    AtencionFoto.FechaFirma = item.FechaFirma;
                    AtencionFoto.help_Atenciones = atencion;
                    db.help_Atenciones_Fotografias.Add(AtencionFoto);
                    db.SaveChanges();
                }

                return Ok("Capturas guardadas con exito.");

            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }

        [HttpPost]
        [Route("SubirFirmaAjustador")]
        public IHttpActionResult SubirFirmaAjustador([FromBody] FotoFirmaAjustadorRequest Firma)
        {
            try
            {
                if (Firma == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                if (String.IsNullOrWhiteSpace(Firma.FotoFirma) || Firma.IdAgente == 0)
                {
                    return BadRequest("Ingrese una imagen y un codigo de agente valido.");
                }
                else
                {
                    var proveedoragente = db.help_ProveedorAgentes.Find(Firma.IdAgente);
                    if (proveedoragente == null)
                    {
                        return BadRequest("No existe el agente indicado: " + Firma.IdAgente);
                    }

                    byte[] FotoData = System.Convert.FromBase64String(ObtenerBase64Limpio(Firma.FotoFirma));
                    proveedoragente.FirmaAjustador = FotoData;
                    db.Entry(proveedoragente).State = EntityState.Modified;
                    db.SaveChanges();
                    
                    return Ok("Firma del ajustador guardada con exito.");

                }

            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }


        [HttpPost]
        [Route("SubirFotografiaAjustador")]
        public IHttpActionResult SubirFotografiaAjustador([FromBody] FotoAjustadorRequest Foto)
        {
            try
            {
                if (Foto == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                if (String.IsNullOrWhiteSpace(Foto.Fotografia) || Foto.IdAgente == 0)
                {
                    return BadRequest("Ingrese una fotografia y un codigo de agente valido.");
                }
                else
                {
                    var proveedoragente = db.help_ProveedorAgentes.Find(Foto.IdAgente);
                    if (proveedoragente == null)
                    {
                        return BadRequest("No existe el agente indicado: " + Foto.IdAgente);
                    }

                    proveedoragente.Fotografia = Foto.Fotografia;
                    db.Entry(proveedoragente).State = EntityState.Modified;
                    db.SaveChanges();

                    return Ok("Fotografia del ajustador guardada con exito.");

                }

            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }

        /*
        [HttpGet]
        [Route("imagen/{Id}")]
        public HttpResponseMessage ConvertByteToImage(int Id)
        {
            Respuesta r = new Respuesta();
            try
            {
                var extension = db.Pruebas_FN_ImagenUnEmpleado(Id).Select(a => a.Extension).FirstOrDefault();

                byte[] dd = db.Pruebas_FN_ImagenUnEmpleado(Id).Select(e => e.FotografiaEmpleado).FirstOrDefault();
                if (dd == null)
                {
                    r.status = true;
                    r.message = null;
                }
                else
                {
                    var imreBase64Data = Convert.ToBase64String(dd);
                    string imgDataUrl = string.Format("data:" + extension + ";base64,{0}", imreBase64Data);
                    r.status = true;
                    r.message = imgDataUrl;
                }

                return Request.CreateResponse(HttpStatusCode.OK, r);
            }
            catch
            {
                r.status = false;
                r.message = "Hubo un problema, por favor hablar con sistemas.";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, r);
            }
        }
        */

        [HttpPost]
        [Route("SubirVideo")]
        public async Task<HttpResponseMessage> SubirVideo(int? IdProveedor, int? IdAtencion)
        {
            Respuesta r = new Respuesta();


            try
            {
                if (Request.Content.IsMimeMultipartContent() && !Request.Content.IsMimeMultipartContent() || !Request.Content.IsMimeMultipartContent() == null)
                {
                    r.status = false;
                    r.message = "Falta la imagen o se envío un formato incorrecto.";
                    return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
                }
                else
                {

                    try
                    {
                        if (Request.Content.Headers.ContentType == null)
                        {

                            r.status = true;
                            r.message = "Usuario actualizado exitosamente.";
                        }
                        else { 
                        
                        
                        // Read the form data.
                        string root = System.Web.HttpContext.Current.Server.MapPath("~/App_Data");
                            var provider = new MultipartFormDataStreamProvider(root);
                            byte[] data = null;
                            await Request.Content.ReadAsMultipartAsync(provider);
                            // This illustrates how to get the file names.
                            foreach (MultipartFileData file in provider.FileData)
                            {
                                Trace.WriteLine(file.Headers.ContentDisposition.FileName);//get FileName
                                Trace.WriteLine("Server file path: " + file.LocalFileName);//get File Path
                                using (MemoryStream ms = new MemoryStream())
                                {
                                using (FileStream fs = File.OpenRead(file.LocalFileName))
                                    {
                                        await fs.CopyToAsync(ms);
                                    }
                                    data = ms.ToArray();
                                }
                            }

                            r.status = true;
                            r.message = "Usuario actualizado exitosamente.";
                        }
                        
                        return Request.CreateResponse(HttpStatusCode.OK, r);

                    }
                    catch (Exception e)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
                    }
                }
            }
            catch (Exception ex)
            {
                r.status = false;
                r.message = "Hubo un problema, por favor hablar con sistemas.";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
            
        }
        /*
        [HttpPost]
        [Route("SubirVideo")]
        public IHttpActionResult SubirVideo(byte[] VideoData,int IdAtencion)
        {
            try
            {
                if (VideoData == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                if (VideoData  != null && IdAtencion != 0)
                {
                    return BadRequest("Ingrese un video y un numero de atencion validos.");
                }
                else
                {
                    help_Atenciones_Fotografias AtencionFoto = new help_Atenciones_Fotografias();

                        db.Database.CommandTimeout = 3000;

                        AtencionFoto.Fotografia = VideoData;
                        AtencionFoto.NombreFoto = "Video" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + IdAtencion;
                        AtencionFoto.TipoFoto = ".MP4";
                        AtencionFoto.RefUsuarioId = 1;
                        AtencionFoto.FechaRegistro = DateTime.Now;
                        AtencionFoto.help_Atenciones = db.help_Atenciones.Find(IdAtencion);
                        AtencionFoto.RefTipoId = 73;
                        db.help_Atenciones_Fotografias.Add(AtencionFoto);
                        db.SaveChanges();
                    

                    return Ok("Video guardado con exito.");

                }

            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }
        */

        [HttpPost]
        [Route("MostrarVideo")]
        public IHttpActionResult MostrarVideo(int? IdAtencion) {
          
            try
            {
                if (IdAtencion == 0)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                var datavideo = db.help_Atenciones_Fotografias.Where(x => x.help_Atenciones.Id == IdAtencion && x.RefTipoId == 73).ToList();

                if (datavideo.Count() > 0)
                {
                    return Ok(datavideo);
                }
                else
                {
                    return BadRequest("La atencion no tiene un video registrado, por favor grabe un video y subalo.");
                }

                
            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }


        /// <summary>
        /// Estado igual 8 = Tomado Por Proveedor, 9 = Cancelada Por Proveedor, 10 = Reasignada Por Proveedor.
        /// </summary>
        /*
        [HttpPost]
        [Route("TomarORCancelarAtencion")]
        [SwaggerOperation(Summary = "Estado igual 8 = Tomado Por Proveedor, 9 = Cancelada Por Proveedor, 10 = Reasignada Por Proveedor.")]
        public IHttpActionResult TomarORCancelarAtencion(int IdAtencion, int Estado) {
            try
            {
                var idatencionservicio = db.help_Atenciones_Servicios.Where(x => x.RefAtencionId == IdAtencion && x.RefTipoServicioId == 32).FirstOrDefault();

                if (idatencionservicio == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                db.help_SP_Update_Status_Aten_Serv(idatencionservicio.Id, Estado);
                this.db.help_SP_SetEstadoProveedor(idatencionservicio.RefProveedorId, 1);

                string mensaje = "";
                if (Estado == 8)
                {
                    mensaje = "Servicio Tomado";
                }
                else {
                    mensaje = "Servicio Cancelado";
                }

                return Ok(mensaje);
            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }

        

        [HttpPost]
        [Route("ReAsignarExpediente")]
        public IHttpActionResult ReAsignarExpediente(int IdAtencion, int IdAgenteProveedor)
        {
            try
            {
                db.help_P_ReAsignarProveedor(IdAtencion, IdAgenteProveedor);

                var idatencionservicio = db.help_Atenciones_Servicios.Where(x => x.RefAtencionId == IdAtencion && x.RefTipoServicioId == 32).FirstOrDefault();

                db.help_SP_Update_Status_Aten_Serv(idatencionservicio.Id, 10);
                this.db.help_SP_SetEstadoProveedor(idatencionservicio.RefProveedorId, 1);

                return Ok("Proveedor ReAsignado");
            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }
        */

        [HttpPost]
        [Route("TokenPushOneSignal")]
        public IHttpActionResult TokenPushOneSignal(int IdProveedorAgente,string TokenPush)
        {
            try
            {
                if (IdProveedorAgente == 0 || TokenPush == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                db.help_P_Api_Actualizar_Token_Agente_Proveedor(IdProveedorAgente,"",TokenPush,2);

                return Ok("Token Actualizado con exito.");
            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }

        [HttpPost]
        [Route("GuardarTiposPersonasSiniestros")] // /api/Proveedor/GuardarTiposPersonasSiniestros/
        public IHttpActionResult GuardarTiposPersonasSiniestros(ApiData.help_PersonasSiniestrosRequest personasiniestro)
        {

            try
            {
                if (personasiniestro == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);


                ObjectParameter Respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter Mensaje = new ObjectParameter("mensaje", typeof(string));


                db.help_P_Api_Insert_Personas_Siniestro(personasiniestro.Nombre, 
                                                        personasiniestro.Telefono, 
                                                        personasiniestro.Direccion, 
                                                        personasiniestro.DescripcionLesion, 
                                                        personasiniestro.DireccionHospitalizacion, 
                                                        personasiniestro.TipoPersonaSiniestro, 
                                                        personasiniestro.RefAjustadorAudienciaId,
                                                        Respuesta,
                                                        Mensaje);




                return Ok("Persona guardado con exito.");
            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }

        [HttpPost]
        [Route("ListaFotografiasFirmasAtencion")] // /api/Proveedor/ListaFotografiasFirmasAtencion/
        public IHttpActionResult ListaFotografiasFirmasAtencion(ApiData.ListaDatosFirma datosfirma) {
            try
            {

                if (datosfirma ==  null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                var listaAjustadores = db.Database.SqlQuery<FotoFirmaAtencionData>(
                    @"SELECT Fotografia AS FotoFirma,
                             NombreFoto,
                             TipoFoto AS Extension
                      FROM dbo.help_Atenciones_Fotografias
                      WHERE RefAtencionId = @IdAtencion
                        AND RefTipoId = @TipoFotoFirma",
                    new SqlParameter("@IdAtencion", datosfirma.IdAtencion),
                    new SqlParameter("@TipoFotoFirma", datosfirma.TipoFotoFirma)
                ).ToList();

                if (listaAjustadores.Count() > 0)
                {

                    listaAjustadores.ForEach(item =>
                    {
                        Data.Add(new
                        {
                            FotoFirma = item.FotoFirma,
                            NombreFoto = item.NombreFoto,
                            Extension = item.Extension
                        });
                    });


                    return Ok(Data);
                }
                else {

                    return BadRequest("Este numero de atencion no es valido o no tiene imagines o firmas para este expediente.");

                }

            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }

            }
        }

        [HttpGet]
        [Route("ListaTipoAcuerdos")] // /api/Proveedor/ListaTipoAcuerdos/
        public IHttpActionResult ListaTipoAcuerdos()
        {
            try
            {

                var datosacuerdos = db.help_Tipo_Acuerdo_Ficohsa.ToList();

                if (datosacuerdos.Count() > 0)
                {

                    datosacuerdos.ForEach(item =>
                    {
                        Data.Add(new
                        {
                            Id = item.Id,
                            Descripcion = item.DESCRIPCION,
                            Codigo = item.CODIGO
                        });
                    });


                    return Ok(Data);
                }
                else
                {

                    return BadRequest("Este numero de atencion no es valido o no tiene imagines o firmas para este expediente.");

                }

            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }

            }
        }


        [HttpPost]
        [Route("ActualizarCodigoBPMAjustador")] // /api/Proveedor/ActualizarCodigoBPMAjustador/
        public IHttpActionResult ActualizarCodigoBPMAjustador(int IdTablaAjustador,string CodigoBPMFicohsa,string CodigoReclamoFicohsa) {
            try
            {
                if (IdTablaAjustador == 0)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                ObjectParameter Respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter Mensaje = new ObjectParameter("mensaje", typeof(string));

                db.help_P_Actualizar_Ajustador_Audiencia(IdTablaAjustador, CodigoBPMFicohsa, CodigoReclamoFicohsa, Respuesta, Mensaje);

                if (Convert.ToInt32(Respuesta.Value) > 0)
                {
                    SendToMail Enviar = new SendToMail();
                    int IdAtencion = db.Database.SqlQuery<int>(
                        @"SELECT TOP 1 ISNULL(RefAtencionId, 0)
                          FROM dbo.help_AjustadorAudiencia
                          WHERE Id = @IdTablaAjustador",
                        new SqlParameter("@IdTablaAjustador", IdTablaAjustador)
                    ).FirstOrDefault();

                    if (IdAtencion <= 0)
                    {
                        return BadRequest("No se pudo obtener la atención asociada al informe del ajustador.");
                    }

                    
                    
                    if (Enviar.Email_NotificacionAccidente(IdAtencion))
                    {
                        return Ok("Ajustador Audiencia Actualizado con exito.");
                    }
                    else
                    {
                        return BadRequest("Clave no se pudo enviar por correo. validar con soporte.");
                    }
                    


                }
                else {

                    return BadRequest(Mensaje.Value.ToString());

                }
                
            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }

        [HttpPost]
        [Route("InsertarReconocimientoDeDeuda")]
        public IHttpActionResult InsertarReconocimientoDeDeuda(
      ApiData.Atencion_Reconocimiento_Deuda deuda)
        {
            try
            {
                if (deuda == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                ObjectParameter respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter mensaje = new ObjectParameter("mensaje", typeof(string));
                byte[] firmaDeudor = Convert.FromBase64String(deuda.FirmaDeudor);
                this.db.help_P_Api_Insert_ReconocimientoDeDeuda(deuda.NombreDeudor, deuda.IdentidadDeudor, deuda.Celular, deuda.Domicilio, deuda.CorreoElectronico, deuda.LugarDeTrabajo, deuda.NombreContacto, deuda.CelularContacto, deuda.FechaRegistroDocumento, deuda.Marca, deuda.Modelo, deuda.Anio, deuda.Placa, deuda.NombreAsegurado, deuda.MarcaImplicado, deuda.ModeloImplicado, deuda.AnioImplicado, deuda.PlacaImplicado, deuda.MotorImplicado, deuda.ChasisImplicado, deuda.NombrePersonaPropiedadImplicada, deuda.DanioCausadoObservacion, deuda.RefAtencionId, new DateTime?(DateTime.Now), firmaDeudor, deuda.Ciudad, deuda.TelefonoCulpable, deuda.LicenciaCulpable, deuda.TipoLicencia, deuda.FechaVencimientoLicencia, deuda.NombreDireccionPropietarioVehiculoImplicado, deuda.CompromisoDePago, deuda.Observaciones, deuda.Edad, deuda.RefTipoFotografiaIdAdeudado, deuda.RefTipoFotografiaIdBeneficiario, respuesta, mensaje);
                return Convert.ToInt32(respuesta.Value) > 0 ? (IHttpActionResult)this.Ok<string>("Reconocimiento Creado con exito.") : (IHttpActionResult)this.BadRequest(mensaje.Value.ToString());
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? (IHttpActionResult)this.BadRequest(ex.InnerException.Message) : (IHttpActionResult)this.BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("InsertarLesionesHospitalizacion")] // /api/Proveedor/InsertarLesionesHospitalizacion/
        public IHttpActionResult InsertarLesionesHospitalizacion(int IdAjustadorAudiencia,string TipoLesionesTercerosAfectados,string NombreHospital)
        {
            try
            {
                if (IdAjustadorAudiencia == 0)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                ObjectParameter Respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter Mensaje = new ObjectParameter("mensaje", typeof(string));

                db.help_P_Api_Insert_LesionesHospitalizacion(IdAjustadorAudiencia, TipoLesionesTercerosAfectados, NombreHospital, Respuesta, Mensaje);

                if (Convert.ToInt32(Respuesta.Value) > 0)
                {
                    return Ok("Reconocimiento Creado con exito.");
                }
                else
                {

                    return BadRequest(Mensaje.Value.ToString());

                }

            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }


        [HttpPost]
        [Route("InsertarConvenioReparacionTaller")]
        public IHttpActionResult InsertarConvenioReparacionTaller(
      ApiData.ConvenioReparacionRequest ConvenioReparacion)
        {
            try
            {
                if (ConvenioReparacion == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                ObjectParameter respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter mensaje = new ObjectParameter("mensaje", typeof(string));
                ObjectParameter respuesta_v = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter mensaje_v = new ObjectParameter("mensaje", typeof(string));

                byte[] fotografia = Convert.FromBase64String(ConvenioReparacion.Fotografia);

                db.help_P_API_Valida_Danio_Por_Atencion(Convert.ToInt32(ConvenioReparacion.IdAtencion), ConvenioReparacion.CodigoDanioVehiculo.ToString(), respuesta_v, mensaje_v);

                if (Convert.ToInt32(respuesta_v.Value) > 0)
                {
                    return BadRequest(mensaje_v.Value.ToString());
                }
                else
                {
                    this.db.help_P_API_Insert_Convenio_Reparacion_Danio_Vehiculo_Cliente_Ajustador(
                   ConvenioReparacion.CodigoDanioVehiculo,
                   ConvenioReparacion.Descripcion,
                   new int?(ConvenioReparacion.IdAtencion),
                   fotografia,
                   new DateTime?(DateTime.Now),
                   new int?(ConvenioReparacion.RefTipoFotoId),
                   ConvenioReparacion.TipoEntidad,
                   new short?(ConvenioReparacion.TipoReparacion),
                   respuesta, mensaje);
                   return Convert.ToInt32(respuesta.Value) > 0 ? (IHttpActionResult)this.Ok<string>("Danio de convenio de reparacion ingresado con exito.") : (IHttpActionResult)this.BadRequest(mensaje.Value.ToString());
                    
                }
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? (IHttpActionResult)this.BadRequest(ex.InnerException.Message) : (IHttpActionResult)this.BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("ContarOtrosDanios")]
        public IHttpActionResult ContarOtrosDanios()
        {
            List<object> objectList = new List<object>();
            try
            {
                int num = ((IQueryable<help_OtrosDanio_Ficohsa>)this.db.help_OtrosDanio_Ficohsa).Count<help_OtrosDanio_Ficohsa>();
                objectList.Add((object)new
                {
                    Respuesta = "OK",
                    Mensaje = num
                });
                return (IHttpActionResult)this.Ok<List<object>>(objectList);
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? (IHttpActionResult)this.BadRequest(ex.InnerException.Message) : (IHttpActionResult)this.BadRequest(ex.Message);
            }
        }

        /*
        [HttpPost]
        [Route("InsertarFiniquitoManual")] // /api/Proveedor/InsertarFiniquitoManual/
        public IHttpActionResult InsertarFiniquitoManual(ApiData.help_FiniquitoManual Finiquito)
        {
            try
            {
                if (Finiquito == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                ObjectParameter Respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter Mensaje = new ObjectParameter("mensaje", typeof(string));

                byte[] FirmaData = System.Convert.FromBase64String(Finiquito.FirmaCliente);

                db.help_P_Api_InsertFiniquito_Ficohsa(Finiquito.NumeroReclamo
                                                    , Finiquito.FechaDesde
                                                    , Finiquito.FechaHasta
                                                    , Finiquito.NombreCliente
                                                    , Finiquito.TipoCoberturaFicohsa
                                                    , Finiquito.FechaFirma
                                                    , FirmaData
                                                    , Finiquito.NombreQuienRecibe
                                                    , Finiquito.IdentidadQuienRecibe
                                                    , Finiquito.NumeroCheque
                                                    , Finiquito.FechaDelCheque
                                                    , Finiquito.ValorDelCheque
                                                    , Finiquito.NombreAFavor
                                                    , Finiquito.Poliza
                                                    , Finiquito.RefAtencionId
                                                    , Finiquito.Marca
                                                    , Finiquito.Modelo
                                                    , Finiquito.Anio
                                                    , Finiquito.Placa
                                                    , Finiquito.Chasis
                                                    , Finiquito.Motor
                                                    , Respuesta
                                                    , Mensaje);

                if (Convert.ToInt32(Respuesta.Value) == 1)
                {
                    return Ok("Finiquito creado con exito.");
                }
                else
                {

                    return BadRequest(Mensaje.Value.ToString());

                }

            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }
        */

        [HttpPost]
        [Route("InsertarCoordenadasAtencionAgenteProveedor")] // /api/Proveedor/InsertarCoordenadasAtencionAgenteProveedor/
        public IHttpActionResult InsertarCoordenadasAtencionAgenteProveedor(ApiData.help_LogDatosGps LogGps)
        {
            try
            {
                if (LogGps == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                ObjectParameter Respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter Mensaje = new ObjectParameter("mensaje", typeof(string));

                //string Tipo = "APP_ATENCION";

                db.help_P_Api_Log_Insert_GPS(LogGps.Longitud
                                            , LogGps.Latitud
                                            , LogGps.RefAtencionId
                                            , DateTime.Now
                                            , LogGps.RefUsuarioId
                                            , LogGps.Tipo
                                            , LogGps.Contador
                                            , Respuesta
                                            , Mensaje);

                if (Convert.ToInt32(Respuesta.Value) == 1)
                {
                    return Ok("Coordenadas Ingresadas exitosamente.");
                }
                else
                {

                    return BadRequest(Mensaje.Value.ToString());

                }

            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }
            }
        }

        [HttpGet]
        [Route("VectorGpsAtencion")] // /api/Proveedor/VectorGpsAtencion/
        public IHttpActionResult VectorGpsAtencion(int IdAtencion, string Tipo)
        {
            try
            {
                var listaAjustadores = db.help_Log_GPS.Where(x => x.RefAtencionId == IdAtencion && x.Tipo == Tipo).ToList();
                listaAjustadores.ForEach(item => {
                    Data.Add(new
                    {
                        Latitud = item.Latitud,
                        Longitud = item.Longitud,
                        IdAtencion = item.RefAtencionId
                    });
                });

                return Ok(Data);

            }
            catch (Exception Ex)
            {

                if (Ex.InnerException != null)
                {
                    return BadRequest(Ex.InnerException.Message);
                }
                else
                {
                    return BadRequest(Ex.Message);
                }

            }

        }

    }

    public class Respuesta
    {
        public bool status { get; set; }
        public string message { get; set; }
    }
}
