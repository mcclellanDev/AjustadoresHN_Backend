using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using WSRestApiHelp.Models;

namespace WSRestApiHelp.Clases
{
    /// <summary>
    /// Validación inicial expediente vs Consulta_Consolidada_de_Asegurados (QA).
    /// Mapeo: MapeoExpedienteWebService.xlsx
    /// </summary>
    public static class BpmValidacionReclamoService
    {
        public static readonly string[] CamposNoValidadosAlInicio =
        {
            "puntoServicio",
            "Producto",
            "Cobertura",
            "Ramo",
            "FechaOcurrencia",
            "Causa",
            "ValorReserva",
            "UsuarioBPM",
            "Latitud",
            "Longitud",
            "NombreConductor",
            "Genero",
            "Parentesco",
            "Observacion"
        };

        public const int MaxDigitosReservaDolares = 5;
        public const decimal MaxReservaLempiras = 28000m;

        public static string ResolverChasisExpediente(help_F_Api_Datos_Expediente_Siniestro_Result expediente)
        {
            if (expediente == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(expediente.ChasisVehiculo))
            {
                return expediente.ChasisVehiculo.Trim();
            }

            if (!string.IsNullOrWhiteSpace(expediente.Chasis))
            {
                return expediente.Chasis.Trim();
            }

            return null;
        }

        public static BpmValidacionCampo CompararCampo(
            string campoBpm,
            string campoWebService,
            string valorExpediente,
            string valorWebService,
            bool noIncluidoEnJsonBpm = false)
        {
            string exp = Normalizar(valorExpediente);
            string ws = Normalizar(valorWebService);

            bool coincide;
            string observacion = null;

            if (string.IsNullOrEmpty(ws))
            {
                coincide = false;
                observacion = "Sin valor en Web Service.";
            }
            else if (string.IsNullOrEmpty(exp))
            {
                coincide = true;
                observacion = "Expediente vacío; se toma valor del Web Service.";
            }
            else if (string.Equals(exp, ws, StringComparison.OrdinalIgnoreCase))
            {
                coincide = true;
            }
            else
            {
                coincide = false;
                observacion = "Diferencia entre expediente y Web Service.";
            }

            var campo = new BpmValidacionCampo
            {
                CampoBpm = campoBpm,
                CampoWebService = campoWebService,
                ValorExpediente = valorExpediente,
                ValorWebService = valorWebService,
                Coincide = coincide,
                Requerido = true,
                NoIncluidoEnJsonBpm = noIncluidoEnJsonBpm,
                Observacion = observacion
            };

            if (noIncluidoEnJsonBpm && string.Equals(campoBpm, "Moneda", StringComparison.OrdinalIgnoreCase))
            {
                campo.ReglaValorReserva = BuildReglaValorReserva(ws);
            }

            return campo;
        }

        public static BpmReglaValorReserva BuildReglaValorReserva(string monedaWs)
        {
            string moneda = Normalizar(monedaWs);
            bool esDolares = moneda.IndexOf("DOLAR", StringComparison.OrdinalIgnoreCase) >= 0
                || string.Equals(moneda, "USD", StringComparison.OrdinalIgnoreCase);

            bool esLempiras = moneda.IndexOf("LEMPIRA", StringComparison.OrdinalIgnoreCase) >= 0
                || string.Equals(moneda, "HNL", StringComparison.OrdinalIgnoreCase);

            return new BpmReglaValorReserva
            {
                Moneda = monedaWs,
                EsDolares = esDolares,
                EsLempiras = esLempiras,
                MaxDigitosDolares = MaxDigitosReservaDolares,
                MaxLempiras = MaxReservaLempiras,
                Descripcion = esDolares
                    ? "Si Moneda es DOLARES, ValorReserva no debe exceder 5 dígitos."
                    : esLempiras
                        ? "Si Moneda es LEMPIRAS, ValorReserva no debe exceder 28000."
                        : "Moneda no reconocida; validar ValorReserva manualmente."
            };
        }

        public static List<BpmValidacionCampo> CompararExpedienteConAsegurado(
            help_F_Api_Datos_Expediente_Siniestro_Result expediente,
            WSFICOHSAHELP_HN.clConsoltAsegurado asegurado)
        {
            string chasisExp = ResolverChasisExpediente(expediente);
            string nombreExp = !string.IsNullOrWhiteSpace(expediente?.NombreAsegurado)
                ? expediente.NombreAsegurado
                : expediente?.Cliente;

            return new List<BpmValidacionCampo>
            {
                CompararCampo("Chasis", "CHASIS", chasisExp, asegurado?.CHASIS),
                CompararCampo("Poliza", "POLIZA", ExtraerNumeroPolizaExterna(expediente?.PolizaExterna), asegurado?.POLIZA),
                CompararCampo("Certificado", "CERTIFICADO", expediente?.Certificado, asegurado?.CERTIFICADO),
                CompararCampo("NombreAsegurado", "CONTRATANTE_NOMBRE", nombreExp, asegurado?.CONTRATANTE_NOMBRE),
                // Sucursal no vive en el expediente HELP; se obtiene del WS (CAMPO_EXTRA_GM_3).
                CompararCampo("Sucursal", "CAMPO_EXTRA_GM_3", null, asegurado?.CAMPO_EXTRA_GM_3),
                CompararCampo("Moneda", "CAMPO_EXTRA_AU_2", expediente?.Moneda, asegurado?.CAMPO_EXTRA_AU_2, noIncluidoEnJsonBpm: true)
            };
        }

        /// <summary>
        /// PolizaExterna suele venir como prefijo-numero (ej. XXX-2001080207).
        /// Para comparar con POLIZA del WS se usa solo la parte después del '-'.
        /// </summary>
        public static string ExtraerNumeroPolizaExterna(string polizaExterna)
        {
            if (string.IsNullOrWhiteSpace(polizaExterna))
            {
                return null;
            }

            string[] partes = polizaExterna.Split(new[] { '-' }, 2);
            if (partes.Length >= 2 && !string.IsNullOrWhiteSpace(partes[1]))
            {
                return partes[1].Trim();
            }

            return polizaExterna.Trim();
        }

        public static object BuildSugeridoParaBpm(
            WSFICOHSAHELP_HN.clConsoltAsegurado asegurado,
            help_F_Api_Datos_Expediente_Siniestro_Result expediente)
        {
            return new
            {
                Chasis = asegurado?.CHASIS,
                Poliza = asegurado?.POLIZA,
                Certificado = asegurado?.CERTIFICADO,
                NombreAsegurado = asegurado?.CONTRATANTE_NOMBRE,
                Sucursal = asegurado?.CAMPO_EXTRA_GM_3,
                Moneda = asegurado?.CAMPO_EXTRA_AU_2,
                Observacion = expediente?.IdAjusteAudiencia > 0
                    ? expediente.IdAjusteAudiencia.ToString()
                    : null
            };
        }

        /// <summary>
        /// Comparación silenciosa: recorta extremos, trata NBSP/tabs/saltos/cero-ancho
        /// como espacio y compacta espacios internos. No cambia los valores que se muestran al usuario.
        /// </summary>
        private static string Normalizar(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var sb = new StringBuilder(value.Length);
            bool lastWasSpace = false;

            foreach (char c in value)
            {
                if (EsSeparadorInsignificante(c))
                {
                    if (!lastWasSpace)
                    {
                        sb.Append(' ');
                        lastWasSpace = true;
                    }

                    continue;
                }

                sb.Append(c);
                lastWasSpace = false;
            }

            return sb.ToString().Trim();
        }

        private static bool EsSeparadorInsignificante(char c)
        {
            if (char.IsWhiteSpace(c))
            {
                return true;
            }

            switch (c)
            {
                case '\u00A0': // NBSP
                case '\u2007': // figure space
                case '\u202F': // narrow NBSP
                case '\uFEFF': // BOM / ZWNBSP
                case '\u200B': // zero-width space
                case '\u200C': // ZWNJ
                case '\u200D': // ZWJ
                case '\u2060': // word joiner
                    return true;
                default:
                    return CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.SpaceSeparator;
            }
        }
    }

    public class BpmValidacionCampo
    {
        public string CampoBpm { get; set; }
        public string CampoWebService { get; set; }
        public string ValorExpediente { get; set; }
        public string ValorWebService { get; set; }
        public bool Coincide { get; set; }
        public bool Requerido { get; set; }
        public bool NoIncluidoEnJsonBpm { get; set; }
        public string Observacion { get; set; }
        public BpmReglaValorReserva ReglaValorReserva { get; set; }
    }

    public class BpmReglaValorReserva
    {
        public string Moneda { get; set; }
        public bool EsDolares { get; set; }
        public bool EsLempiras { get; set; }
        public int MaxDigitosDolares { get; set; }
        public decimal MaxLempiras { get; set; }
        public string Descripcion { get; set; }
    }
}
