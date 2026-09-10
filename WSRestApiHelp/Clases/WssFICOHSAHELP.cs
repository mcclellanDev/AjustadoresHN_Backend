using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSRestApiHelp.Clases
{
    public class WssFICOHSAHELP_HN
    {
        public string Msj { get; set; }

        private WSFICOHSAHELP_HN_PRO.WSPorSaludSoapClient WSFH = new WSFICOHSAHELP_HN_PRO.WSPorSaludSoapClient();
        private WSFICOHSAHELP_HN.WSPorSaludSoapClient WSFH_QA = new WSFICOHSAHELP_HN.WSPorSaludSoapClient();
        //public int Public = 4;//Pruebas
        public int Public = 7;//Produccion

        public List<WSFICOHSAHELP_HN_PRO.valida_lista_coberturasRequest> listaCoberturas = new List<WSFICOHSAHELP_HN_PRO.valida_lista_coberturasRequest>();

        public List<object> validaCoberturas(ApiData.CoberturaData cobertura)
        {
            List<object> respuesta = new List<object>();
            WSFICOHSAHELP_HN_PRO.valida_lista_coberturasRequest Consulta = new WSFICOHSAHELP_HN_PRO.valida_lista_coberturasRequest()
            {
                credenciales = new WSFICOHSAHELP_HN_PRO.credenciales()
                {
                    usuario = "userBanfarma",
                    clave = "2014BANf@rm@8@nc@",
                    publicacion = Public
                },
                nIdepol = cobertura.pNumPoliza,
                nNumOper = cobertura.pNumSiniestro,
                nNumcer = Convert.ToInt32(cobertura.pNumCertificado),
                nIdeBien = cobertura.pNumEndoso,
                nIdeAseg = cobertura.pNumAsegurado,
                nNumpol = Convert.ToInt32(Public),
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
                Msj = listaCoberturas.FirstOrDefault().mensaje.ToString();
            }
            else
            {
                foreach (var item in listaCoberturas.FirstOrDefault().clDetListaCobert)
                {
                    respuesta.Add(new
                    {
                        codigo = item.CODIGO,
                        descripcion = item.DESCRIPCION,
                        cobertura = item.COBERTURA,
                        suma = item.SUMA,
                    });
                }
                
            }
            return respuesta;
        }
    }
}