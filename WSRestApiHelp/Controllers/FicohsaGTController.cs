using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WSRestApiHelp.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    [RoutePrefix("api/FicohsaGT")]
    public class FicohsaGTController : ApiController
    {
        public string Msj { get; set; }
        //CONSulta WS GT
        private WSFICOHSAHELP.WSPorSaludSoapClient WSFH = new WSFICOHSAHELP.WSPorSaludSoapClient();
        //public int Public = 4;//Pruebas-
        public int Public = 7;//Produccion
        public List<WSFICOHSAHELP.clDatosPolizaAuto> DatosPolizaAutHelp = new List<WSFICOHSAHELP.clDatosPolizaAuto>();

        public List<object> Data = new List<object>();

        [HttpPost]
        [Route("Consultar_Estado_Sinietro")] // /api/FicohsaHN/Consultar_Estado_Sinietro/
        //CONSULTAR METODO GT
        public IHttpActionResult ConsultarAseguradoGT(string Identidad, string NombreAsegurado, string Chasis, string Placa)
        {

            WSFICOHSAHELP.consultaDatosPolizaRequest Consulta = new WSFICOHSAHELP.consultaDatosPolizaRequest()
            {
                credenciales = new WSFICOHSAHELP.credenciales() { usuario = "userBanfarma", clave = "2014BANf@rm@8@nc@", publicacion = Public },
                pIdentidad = Identidad,
                pNombreTitular = NombreAsegurado,
                pChasis = Chasis,
                pPlaca = Placa
            };

            var ListaAseguradoHelp = WSFH.consultaDatosPoliza(Consulta.credenciales,
                                                              Consulta.pIdentidad,
                                                              Consulta.pNombreTitular,
                                                              Consulta.pChasis,
                                                              Consulta.pPlaca).ToList();

            if (ListaAseguradoHelp.FirstOrDefault().cod_error == "-1")
            {
                Data.Add(new { 
                    Mensaje = ListaAseguradoHelp.FirstOrDefault().mensaje.ToString()
                });
            }
            else
            {

                foreach (var i in ListaAseguradoHelp.FirstOrDefault().DatosPolizaAuto.ToList())
                {
                    Data.Add(new WSFICOHSAHELP.clDatosPolizaAuto()
                    {
                        Archivo = i.Archivo,
                        Fecha_Datos = i.Fecha_Datos,
                        Pais = i.Pais,
                        Nombre = i.Nombre,
                        Sexo = i.Sexo,
                        Fecha_Nacimiento = i.Fecha_Nacimiento,
                        Tipo_Persona = i.Tipo_Persona,
                        Tipo_Identificacion = i.Tipo_Identificacion,
                        Identificacion = i.Identificacion,
                        Rtn = i.Rtn,
                        Id_Cliente = i.Id_Cliente,
                        Id_Persona = i.Id_Persona,
                        Telefono1 = i.Telefono1,
                        Direccion = i.Direccion,
                        Correo = i.Correo,
                        Tipo_Beneficiario = i.Tipo_Beneficiario,
                        Clase = i.Clase,
                        Ramo = i.Ramo,
                        Poliza = i.Poliza,
                        Certificado = i.Certificado,
                        Id_Contratante = i.Id_Contratante,
                        Contratante_Nombre = i.Contratante_Nombre,
                        Id_Aseguradora = i.Id_Aseguradora,
                        Aseguradora_Nombre = i.Aseguradora_Nombre,
                        Fecha_Ingreso = i.Fecha_Ingreso,
                        Fecha_Ini = i.Fecha_Ini,
                        Fecha_Fin = i.Fecha_Fin,
                        Req_Administrativo = i.Req_Administrativo,
                        Pre_Existencia = i.Pre_Existencia,
                        Estado = i.Estado,
                        Id_Agente = i.Id_Agente,
                        Agente_Nombre = i.Agente_Nombre,
                        Agente_Telefono = i.Agente_Telefono,
                        Agente_Email = i.Agente_Email,
                        Asistencia = i.Asistencia,
                        Ano = i.Ano,
                        Tipo_Vehiculo = i.Tipo_Vehiculo,
                        Marca = i.Marca,
                        Modelo = i.Modelo,
                        Color = i.Color,
                        Placa = i.Placa,
                        Chasis = i.Chasis,
                        Motor = i.Motor,
                        Colision = i.Colision,
                        Danos_Tasa = i.Danos_Tasa,
                        Robo = i.Robo,
                        Robo_Tasa = i.Robo_Tasa,
                        Seccion1 = i.Seccion1,
                        Seccion2 = i.Seccion2,
                        Seccion3 = i.Seccion3,
                    });
                }

                //ListaAseguradoHelp.ForEach(item => {
                //    item.DatosPolizaAuto.ToList().ForEach(i => {
                //        respuesta.Add(new
                //        {
                //            Archivo = i.Archivo,
                //            Fecha_Datos = i.Fecha_Datos,
                //            Pais = i.Pais,
                //            Nombre = i.Nombre,
                //            Sexo = i.Sexo,
                //            Fecha_Nacimiento = i.Fecha_Nacimiento,
                //            Tipo_Persona = i.Tipo_Persona,
                //            Tipo_Identificacion = i.Tipo_Identificacion,
                //            Identificacion = i.Identificacion,
                //            Rtn = i.Rtn,
                //            Id_Cliente = i.Id_Cliente,
                //            Id_Persona = i.Id_Persona,
                //            Telefono1 = i.Telefono1,
                //            Direccion = i.Direccion,
                //            Correo = i.Correo,
                //            Tipo_Beneficiario = i.Tipo_Beneficiario,
                //            Clase = i.Clase,
                //            Ramo = i.Ramo,
                //            Poliza = i.Poliza,
                //            Certificado = i.Certificado,
                //            Id_Contratante = i.Id_Contratante,
                //            Contratante_Nombre = i.Contratante_Nombre,
                //            Id_Aseguradora = i.Id_Aseguradora,
                //            Aseguradora_Nombre = i.Aseguradora_Nombre,
                //            Fecha_Ingreso = i.Fecha_Ingreso,
                //            Fecha_Ini = i.Fecha_Ini,
                //            Fecha_Fin = i.Fecha_Fin,
                //            Req_Administrativo = i.Req_Administrativo,
                //            Pre_Existencia = i.Pre_Existencia,
                //            Estado = i.Estado,
                //            Id_Agente = i.Id_Agente,
                //            Agente_Nombre = i.Agente_Nombre,
                //            Agente_Telefono = i.Agente_Telefono,
                //            Agente_Email = i.Agente_Email,
                //            Asistencia = i.Asistencia,
                //            Ano = i.Ano,
                //            Tipo_Vehiculo = i.Tipo_Vehiculo,
                //            Marca = i.Marca,
                //            Modelo = i.Modelo,
                //            Color = i.Color,
                //            Placa = i.Placa,
                //            Chasis = i.Chasis,
                //            Motor = i.Motor,
                //            Colision = i.Colision,
                //            Danos_Tasa = i.Danos_Tasa,
                //            Robo = i.Robo,
                //            Robo_Tasa = i.Robo_Tasa,
                //            Seccion1 = i.Seccion1,
                //            Seccion2 = i.Seccion2,
                //            Seccion3 = i.Seccion3,
                //        });
                //    });
                //});
            }

            return Ok(Data);

        }

    }
}
