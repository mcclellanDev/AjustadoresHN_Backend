using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static WSRestApiHelp.Controllers.ProveedorController;
using WSRestApiHelp.Models;
using System.Data.Entity.Core.Objects;
using WSRestApiHelp.Clases;
using System.Web.Http.Cors;
using System.Web;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.DotNet.PlatformAbstractions;
using static WSRestApiHelp.Clases.ApiData;

namespace WSRestApiHelp.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [AllowAnonymous]
    [RoutePrefix("api/Login")]
    
    public class LoginController : ApiController
    {
        
        private helpEntities db = new helpEntities();

        public List<object> Data = new List<object>();

        [HttpPost]
        [Route("Autenticacion")] // /api/Login/Autenticacion/
        public IHttpActionResult Autenticacion(ApiData.LoginDataRequest loginData)
        {
            try
            {

                ObjectParameter Respuesta = new ObjectParameter("Respuesta", typeof(int));
                ObjectParameter Mensaje = new ObjectParameter("mensaje", typeof(string));

                if (loginData == null)
                    throw new HttpResponseException(HttpStatusCode.BadRequest);

                if (String.IsNullOrEmpty(loginData.User) && String.IsNullOrEmpty(loginData.Password))
                {
                    return BadRequest("Correo o contraseña estan vacios. por favor ingrese un valor valido.");
                }
                else
                {
                    var VaildarCorreo = db.help_P_Api_ValidaCorreo(loginData.User, Respuesta, Mensaje);
                    if (Convert.ToInt32(Respuesta.Value) == 1)
                    {

                        var PassEnc = db.help_F_Api_ObtenerPassword(loginData.User).ToList();
                        var user = PassEnc.First();
                        //var statusUser = dbm.help_F_Api_UserCheckStatus(user.userId);
                        if (user.Contrasena != null)
                        {
                            //SophiaSecurityCrypt NewCripta = new SophiaSecurityCrypt(SophiaSecurityCrypt.SophiaTypeCrypt.Simmetric);
                            //string key = SophiaBase64Encoder.Encoder(Password + user.userId);
                            //var Desencriptada = NewCripta.Desencriptar(user.password2, (new PasswordDeriveBytes(key, null)).GetBytes(32));

                            if (user.Contrasena == loginData.Password)
                            {
                                if (user.UsuarioBPM != null)
                                {
                                    var token = TokenGenerator.GenerateTokenJwt(loginData.User, user.Id);

                                    db.help_P_Api_Actualizar_Token_Agente_Proveedor(user.Id, token.ToString(), null, 1);

                                    Data.Add(new
                                    {
                                        ProveedorAgenteId = user.Id,
                                        ProveedorName = db.help_Proveedor.Find(user.RefProveedorId).NombreProveedor,
                                        TipoAgente = user.TypoAgenteString,
                                        Correo = user.Correo,
                                        ProveedorId = user.RefProveedorId,
                                        Usuario = user.Usuario,
                                        NombreAgente = user.NombreAgente,
                                        Token = token.ToString(),
                                        Foto = user.Fotografia,
                                        Lat = user.LatitudActual,
                                        Long = user.LongitudActual,
                                        Movil = user.MovilServicio,
                                        IdPaid = user.RefIdPais,
                                        Pais = user.Pais,
                                        Firma = user.FirmaAjustador,
                                        UsuarioBPM = user.UsuarioBPM
                                        //status = status
                                    });

                                    return Ok(Data);
                                }
                                else
                                {
                                    return BadRequest("El Ajustador no tiene codigo BPM asignado contacte a soporte.");
                                }

                            }
                            else
                            {
                                return BadRequest("Contraseña invalida");
                            }
                        }
                        else
                        {

                            return BadRequest("Por favor verifica tu contraseña o valida con el administrador de HELP.");
                        }
                    }
                    else
                    {
                        return BadRequest("Email invalido");

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

        [HttpPost]
        [Route("GetAppVersion")] // /api/Login/RecuperarPassword/
        public IHttpActionResult ObtenerVersionApp(string plataforma)
        {
            try {

                if (String.IsNullOrEmpty(plataforma))
                {
                    return BadRequest("El campo se encuentra vacio. por favor ingrese un campo");
                }
                else
                {
                    List<object> Data = new List<object>();
                    List<object> appversion = new List<object>();

                    ObjectParameter Respuesta = new ObjectParameter("Respuesta", typeof(int));
                    ObjectParameter Mensaje = new ObjectParameter("mensaje", typeof(string));

                    var versionApp = db.help_F_GetAppVersion(plataforma).ToList();
                   
                    versionApp.ForEach(item =>
                    {
                        Mensaje.Value = item.versionCodigo;
                    });


                    return Ok(Mensaje.Value.ToString());
                }
            }
            catch (Exception Ex) {
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
        [Route("RecuperarPassword")] // /api/Login/RecuperarPassword/
        public IHttpActionResult RecuperarPassword(string user) {

            try
            {
                SendToMail Enviar = new SendToMail();

                if (String.IsNullOrEmpty(user))
                {
                    return BadRequest("El campo se encuentra vacio. por favor ingrese un campo");
                }
                else
                {

                    List<object> Data = new List<object>();
                    ObjectParameter Respuesta = new ObjectParameter("Respuesta", typeof(int));
                    ObjectParameter Mensaje = new ObjectParameter("mensaje", typeof(string));

                    db.help_P_Api_Insert_Clave_Recuperar_Password(user, Respuesta, Mensaje);
                    
                    if (Convert.ToInt32(Respuesta.Value) == 1) {

                        if (Enviar.RecuperarPassword(user, (Mensaje.Value.ToString())))
                        {
                            return Ok(Mensaje.Value.ToString());
                        }
                        else {
                            return BadRequest("Clave no se pudo enviar por correo. validar con soporte.");
                        }           
                    }
                    else {

                        return BadRequest(Mensaje.Value.ToString());
                    
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

        [HttpPost]
        [Route("ValidarClave")]
        public IHttpActionResult ValidarClave(string clave,string user) {
            try
            {
                if (String.IsNullOrEmpty(clave))
                {

                    return BadRequest("El campo se encuentra vacio. por favor ingrese un campo");

                }
                else {

                    var validar =  db.help_ProveedorAgentes.Where(x => x.ClaveRecuperarPassword == clave && x.Usuario == user).ToList();
                    if (validar.Count() > 0) {
                        return Ok("Clave y usurio coinciden proceda autenticarse.");
                    }
                    else {
                        return BadRequest("Clave no coinciden. Intente de nuevo");
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

        [HttpPost]
        [Route("ActualizarPassword")]
        public IHttpActionResult ActualizarPassword(string user,string password) {
            try
            {
                if (String.IsNullOrEmpty(user) && String.IsNullOrEmpty(password))
                {
                    return BadRequest("Correo o contraseña estan vacios. por favor ingrese un valor valido.");
                }
                else { 
                
                    db.help_P_Api_Actualizar_Password_Proveedor_Agente(user,password);
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
        [Route("EnviarNotificacionAccidente")]
        public IHttpActionResult EnviarNotificacionAccidente(int IdAtencion)
        {
            try
            {
                if (IdAtencion == 0)
                {
                    return BadRequest("Correo o contraseña estan vacios. por favor ingrese un valor valido.");
                }
                else
                {

                    SendToMail Enviar = new SendToMail();

                    if (Enviar.Email_NotificacionAccidente(IdAtencion))
                    {
                        return Ok("Correo de notificacion enviado con exito.");
                    }
                    else
                    {
                        return BadRequest("Clave no se pudo enviar por correo. validar con soporte.");
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

        [HttpPost]
        [Route("SubirVideo")] //   /api/Login/SubirVideo/
        public IHttpActionResult SubirVideo(int IdAtencion, IFormFile File)
        {
            try
            {

                if (IdAtencion == 0)
                {
                    return BadRequest("Ingrese un número de atención válido.");
                }

                // Validar el tamaño del archivo
                if (File == null || File.Length == 0)
                {
                    return BadRequest("Ingrese un archivo válido.");
                }

                // Establecer el límite de tamaño permitido (por ejemplo, 10 MB)
                long sizeLimit = 16 * 1024 * 1024; // 10 MB

                if (File.Length > sizeLimit)
                {
                    return BadRequest($"El tamaño del archivo supera el límite permitido de {sizeLimit / (1024 * 1024)} MB.");
                }

                var atencion = db.help_Atenciones.Find(IdAtencion);
                if (atencion == null)
                {
                    return BadRequest("No existe la atencion indicada: " + IdAtencion);
                }

                help_Atenciones_Fotografias AtencionFoto = new help_Atenciones_Fotografias();

                db.Database.CommandTimeout = 3000;

                AtencionFoto.Fotografia = LeerBytesDesdeStream(File.OpenReadStream());
                AtencionFoto.NombreFoto = "Video" + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + IdAtencion + " - " + Guid.NewGuid();
                AtencionFoto.TipoFoto = File.ContentType;
                AtencionFoto.RefUsuarioId = 1;
                AtencionFoto.FechaRegistro = DateTime.Now;
                AtencionFoto.help_Atenciones = db.help_Atenciones.Find(IdAtencion);
                AtencionFoto.RefTipoId = 73;
                AtencionFoto.help_Atenciones = atencion;
                db.help_Atenciones_Fotografias.Add(AtencionFoto);
                db.SaveChanges();


                return Ok("Video guardado con exito.");

                

            }
            catch (Exception ex)
            {

               

                if (ex.InnerException != null)
                {
                    return BadRequest($"Error: {ex.InnerException.Message}");
                }
                else
                {
                    return BadRequest($"Error: {ex.Message}");
                }
            }
        }

        private byte[] LeerBytesDesdeStream(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }


    }
}
