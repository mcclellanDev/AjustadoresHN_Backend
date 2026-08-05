using Sophia;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Cors;
using WSRestApiHelp.Clases;
using WSRestApiHelp.Models;

namespace WSRestApiHelp.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    [RoutePrefix("api/Cliente")]
    public class ClienteController : ApiController
    {
        private helpEntities db = new helpEntities();

        public List<object> Data = new List<object>();

        [HttpPost]
        [Route("Autenticacion")] // /api/Cliente/Autenticacion/
        public IHttpActionResult Autenticacion(ApiData.LoginClienteDataRequest loginData)
        {
            try
            {

                ObjectParameter Respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter IdCliente = new ObjectParameter("IdCliente", typeof(int));
                ObjectParameter IdSesion = new ObjectParameter("IdSesion", typeof(int));
                ObjectParameter Token = new ObjectParameter("Token", typeof(string));
                ObjectParameter Key = new ObjectParameter("Key", typeof(string));

                if (loginData == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                if (String.IsNullOrEmpty(loginData.User) && String.IsNullOrEmpty(loginData.Password))
                {
                    return BadRequest("Correo o contraseña estan vacios. por favor ingrese un valor valido.");
                }
                else
                {

                    var VaildarCorreo = db.help_SP_Valida_Login_Cliente(loginData.User, loginData.Password, Respuesta, IdCliente);
                    if (Convert.ToInt32(IdCliente.Value) != 0)
                    {
                        //int IdCliente =  Convert.ToInt32(Mensaje.Value);
                        var token = TokenGenerator.GenerateTokenJwt(loginData.User);
                        db.help_SP_Crear_Sesion_Cliente(Convert.ToInt32(IdCliente.Value), loginData.Host, loginData.Dispositivo, IdSesion, Token, Key);

                        var clienteInfo = db.help_GetInformacion_Cliente(Convert.ToInt32(IdCliente.Value)).FirstOrDefault();
                        Data.Add(new
                        {
                            Id = clienteInfo.Id,
                            RefPaisId = clienteInfo.RefPaisId,
                            RefPersonaId = clienteInfo.RefPersonaId,
                            TipoIdentificacion = clienteInfo.TipoIdentificacion,
                            NumeroIdentificacion = clienteInfo.NumeroIdentificacion,
                            Nombre = clienteInfo.Nombre,
                            Sexo = clienteInfo.Sexo,
                            FechaNacimiento = clienteInfo.FechaNacimiento,
                            NumeroMovil = clienteInfo.NumeroMovil,
                            Fotografia = clienteInfo.Fotografia,
                            Usuario = clienteInfo.Usuario,
                            Contrasena = clienteInfo.Contrasena,
                            FechaRegistro = clienteInfo.FechaRegistro,
                            Estado = clienteInfo.Estado,
                            Poliza = clienteInfo.Poliza,
                            Certificado = clienteInfo.Certificado,
                            Token = token,
                            Firma = clienteInfo.Firma,
                            Marca = clienteInfo.Marca,
                            Modelo = clienteInfo.Modelo,
                            Anio = clienteInfo.Anio,
                            Motor = clienteInfo.Motor,
                            Placa = clienteInfo.Placa,
                            Chasis = clienteInfo.Chasis,
                            Tipo = clienteInfo.Tipo_Vehiculo,
                            Color = clienteInfo.Color,
                            FotografiaVehiculo = clienteInfo.FotografiaVehiculo
                        });
                        return Ok(Data);
                    }
                    else
                    {
                        return BadRequest("La person aún no ha sido afiliada. Si cree que esto es un error contacte a soporte.");
                    }

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
        [Route("ObtenerMisAtencionesActivas")] // /api/Cliente/ObtenerMisAtencionesActivas/
        public IHttpActionResult ObtenerMisAtencionesActivas(string IdCliente)
        {

            try
            {
                
                List<object> Data = new List<object>();

                var misAtenciones = db.help_F_Api_Listar_Mis_Atenciones_Activas_Cliente(IdCliente).OrderBy(x => x.FechaInicio).ToList();

                if (misAtenciones.Count() > 0)
                {

                    misAtenciones.ForEach(item => {
                        Data.Add(new
                        {
                            IdAtencion = item.Id,
                            Servicio = item.LbTipoServicio,
                            ServicioId = item.ServicioId,
                            ServicioIdCategoria = item.IdCategoriaServicio,
                            Categoria = item.LbCategoriaServicio,
                            CategoriaId = item.IdCategoria,
                            Proveedor = item.NombreProveedor,
                            Zona = item.Zona,
                            Direccion = item.Direccion,
                            AgenteId = item.RefAgenteId,
                            ProveedorId = item.RefProveedorId,
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


        [HttpGet]
        [Route("ObtenerRazonesAsignacion")] // /api/Proveedor/ObtenerRazonesAsignacion/
        public IHttpActionResult ObtenerRazonesAsignacion()
        {
            try
            {
                var listaAjustadores = db.help_F_ObtenerRazonesAsignacion(1).ToList(); //help_Log_GPS.Where(x => x.RefAtencionId == IdAtencion && x.Tipo == Tipo).ToList();
                listaAjustadores.ForEach(item => {
                    Data.Add(new
                    {
                        razon = item.razon,
                        razonId = item.razonId
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


        /*
        [HttpPost]
        [Route("FirmaFotoCliente")] // /api/Proveedor/ListadoAjustadores/
        public IHttpActionResult FirmaFotoCliente(string Identificacion)
        {
            try
            {
                var imagenesCliente = db.help_F_ObtenerFirmaFoto_Cliente(Identificacion).ToList();
                //                var listaAjustadores = db.help_F_Api_ListadoAbogados(IdPais).ToList();
                imagenesCliente.ForEach(item => {
                    Data.Add(new
                    {
                        Fotografia  = item.Fotografia,
                        Firma = item.Firma
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

        
        [HttpPost]
        [Route("FirmaCliente_Save")] // /api/Proveedor/ListadoAjustadores/
        public IHttpActionResult FirmaCliente_Save(ApiData.datosFirmaCliente datosFirma)
        {
            try
            {
                db.help_P_GuardarFirmaCliente(datosFirma.IdAfiliado, datosFirma.Firma);
                this.Data.Add((object)new
                {
                    Respuesta = 1,
                    Mensaje = "Firma guardada exitosamente!"
                });
                return (IHttpActionResult)this.Ok<List<object>>(this.Data);

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
        [Route("FotografiaCliente_Save")] // /api/Proveedor/ListadoAjustadores/
        public IHttpActionResult FotografiaCliente_Save(ApiData.datosFotografiaCliente datosFoto)
        {
            try
            {
                db.help_P_GuardarFotoCliente(datosFoto.IdAfiliado, datosFoto.Fotografia);
                this.Data.Add((object)new
                {
                    Respuesta = 1,
                    Mensaje = "Foto guardada exitosamente!"
                });
                return (IHttpActionResult)this.Ok<List<object>>(this.Data);

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
        [Route("FotografiaClienteVehiculo_Save")] // /api/Proveedor/ListadoAjustadores/
        public IHttpActionResult FotografiaClienteVehiculo_Save(ApiData.datosFotografiaClienteVehiculo datosFoto)
        {
            try
            {
                db.help_P_GuardarFotoVehiculoAfiliado(datosFoto.Identificacion, datosFoto.Fotografia);
                this.Data.Add((object)new
                {
                    Respuesta = 1,
                    Mensaje = "Foto guardada exitosamente!"
                });
                return (IHttpActionResult)this.Ok<List<object>>(this.Data);

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
        [Route("ActualizarPasswordCliente")]
        public IHttpActionResult ActualizarPasswordCliente(string user, string password)
        {
            try
            {
                if (String.IsNullOrEmpty(user) && String.IsNullOrEmpty(password))
                {
                    return BadRequest("Correo o contraseña estan vacios. por favor ingrese un valor valido.");
                }
                else
                {

                    db.help_P_Api_Actualizar_Password_Cliente(user, password);
                    return Ok("Contraseña actualizada con exito.");

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
        [Route("InsertarNotificacionCliente")] // /api/Proveedor/InsertarCoordenadasAtencionAgenteProveedor/
        public IHttpActionResult InsertarNotificacionCliente(ApiData.help_DatosNotificacionCLiente notificacion)
        {
            try
            {
                if (notificacion == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                ObjectParameter Respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter Mensaje = new ObjectParameter("mensaje", typeof(string));

                db.help_P_Insertar_Notificacion_Cliente(
                    notificacion.titulo
                    ,notificacion.mensaje
                    ,notificacion.esAtencion
                    ,notificacion.IdAtencion
                    ,notificacion.esServicio
                    ,notificacion.Servicio
                    ,notificacion.Direccion
                    ,notificacion.Ciudad
                    ,notificacion.Colonia
                    ,notificacion.Cliente
                    ,notificacion.Plan
                    ,notificacion.Estado
                    ,notificacion.ColorEstado
                    ,notificacion.esProveedor
                    ,notificacion.ProveedorAgenteId);

                if (Convert.ToInt32(Respuesta.Value) == 1)
                {
                    return Ok("Notificacion Ingresada exitosamente.");
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

        [HttpGet]
        [Route("ObtenerMisAtencionesActivasCliente")] // /api/Cliente/ObtenerMisAtencionesActivasCliente/
        public IHttpActionResult ObtenerMisAtencionesActivasCliente(string Identificacion)
        {

            try
            {
                if (Identificacion == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                List<object> Data = new List<object>();

                var misAtenciones = db.help_F_Api_Listar_Mis_Atenciones_Activas_Cliente(Identificacion).OrderBy(x => x.FechaInicio).ToList();

                if (misAtenciones.Count() > 0)
                {

                    misAtenciones.ForEach(item => {
                        Data.Add(new
                        {
                            IdAtencion = item.Id,
                            Servicio = item.LbTipoServicio,
                            ServicioId = item.ServicioId,
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
    }


}
