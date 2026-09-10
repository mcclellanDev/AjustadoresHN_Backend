using System;
using System.Configuration;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace WSRestApiHelp.Clases
{
    public static class AudienciaCalendarioHelper
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static string BuildGoogleCalendarUrl(
            string titulo,
            string descripcion,
            string lugar,
            DateTime inicioLocal,
            DateTime finLocal)
        {
            DateTime inicioUtc = inicioLocal.AddHours(6);
            DateTime finUtc = finLocal.AddHours(6);
            string dates =
                inicioUtc.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture) + "/" +
                finUtc.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture);

            return "https://calendar.google.com/calendar/render?action=TEMPLATE" +
                   "&text=" + Uri.EscapeDataString(titulo ?? "") +
                   "&dates=" + dates +
                   "&details=" + Uri.EscapeDataString(descripcion ?? "") +
                   "&location=" + Uri.EscapeDataString(lugar ?? "") +
                   "&ctz=America/Tegucigalpa";
        }

        public static string BuildOutlookWebCalendarUrl(
            string titulo,
            string descripcion,
            string lugar,
            DateTime inicioLocal,
            DateTime finLocal)
        {
            // Deeplink de Outlook en la web (abre el evento sin descargar .ics).
            string start = inicioLocal.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);
            string end = finLocal.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);

            return "https://outlook.office.com/calendar/0/deeplink/compose?path=/calendar/action/compose&rru=addevent" +
                   "&subject=" + Uri.EscapeDataString(titulo ?? "") +
                   "&body=" + Uri.EscapeDataString(descripcion ?? "") +
                   "&location=" + Uri.EscapeDataString(lugar ?? "") +
                   "&startdt=" + Uri.EscapeDataString(start) +
                   "&enddt=" + Uri.EscapeDataString(end);
        }

        public static string BuildIcsContent(
            string titulo,
            string descripcion,
            string lugar,
            DateTime inicioLocal,
            DateTime finLocal,
            int idExpediente,
            int idAtencion)
        {
            DateTime inicioUtc = inicioLocal.AddHours(6);
            DateTime finUtc = finLocal.AddHours(6);
            string stamp = DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture);
            string uid = string.Format(
                CultureInfo.InvariantCulture,
                "audiencia-{0}-{1}-{2}@porsalud.net",
                idExpediente,
                idAtencion,
                inicioUtc.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture));

            var sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//PORSALUD//HELP Ajustadores//ES");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine("METHOD:PUBLISH");
            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine("UID:" + uid);
            sb.AppendLine("DTSTAMP:" + stamp);
            sb.AppendLine("DTSTART:" + inicioUtc.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture));
            sb.AppendLine("DTEND:" + finUtc.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture));
            sb.AppendLine("SUMMARY:" + EscapeIcsText(titulo));
            sb.AppendLine("DESCRIPTION:" + EscapeIcsText(descripcion));
            sb.AppendLine("LOCATION:" + EscapeIcsText(lugar));
            sb.AppendLine("STATUS:CONFIRMED");
            sb.AppendLine("END:VEVENT");
            sb.AppendLine("END:VCALENDAR");
            return sb.ToString();
        }

        public static string BuildDownloadUrl(
            string titulo,
            string descripcion,
            string lugar,
            DateTime inicioLocal,
            DateTime finLocal,
            int idExpediente,
            int idAtencion,
            int validezDias = 90)
        {
            var payload = new AudienciaCalendarioPayload
            {
                Titulo = titulo,
                Descripcion = descripcion,
                Lugar = lugar,
                Inicio = inicioLocal,
                Fin = finLocal,
                IdExpediente = idExpediente,
                IdAtencion = idAtencion,
                ExpiraUtc = DateTime.UtcNow.AddDays(validezDias)
            };

            string data = ToBase64Url(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload, JsonOptions)));
            string sig = ComputeSignature(data);
            string baseUrl = ObtenerApiBaseUrl().TrimEnd('/');

            return baseUrl + "/api/Proveedor/DescargarAudienciaIcs?data=" +
                   Uri.EscapeDataString(data) +
                   "&sig=" + Uri.EscapeDataString(sig);
        }

        public static bool TryResolvePayload(string data, string sig, out AudienciaCalendarioPayload payload, out string error)
        {
            payload = null;
            error = null;

            if (string.IsNullOrWhiteSpace(data) || string.IsNullOrWhiteSpace(sig))
            {
                error = "Token de calendario incompleto.";
                return false;
            }

            string expected = ComputeSignature(data);
            if (!FixedTimeEquals(expected, sig))
            {
                error = "Firma de calendario invalida.";
                return false;
            }

            try
            {
                byte[] jsonBytes = FromBase64Url(data);
                payload = JsonSerializer.Deserialize<AudienciaCalendarioPayload>(
                    Encoding.UTF8.GetString(jsonBytes),
                    JsonOptions);
            }
            catch
            {
                error = "Token de calendario corrupto.";
                return false;
            }

            if (payload == null)
            {
                error = "Token de calendario vacio.";
                return false;
            }

            if (payload.ExpiraUtc < DateTime.UtcNow)
            {
                error = "El enlace del calendario ha expirado.";
                return false;
            }

            return true;
        }

        private static string ObtenerApiBaseUrl()
        {
            string configured = ConfigurationManager.AppSettings["API_PUBLIC_BASE_URL"];
            if (!string.IsNullOrWhiteSpace(configured))
            {
                return configured.Trim();
            }

            string issuer = ConfigurationManager.AppSettings["JWT_ISSUER_TOKEN"];
            if (!string.IsNullOrWhiteSpace(issuer))
            {
                return issuer.Trim();
            }

            return "https://portal.porsalud.net/Outer/WSRestApiHelp/";
        }

        private static string ComputeSignature(string data)
        {
            string secret = ConfigurationManager.AppSettings["JWT_SECRET_KEY"] ?? "clave-secreta-api";
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return ToBase64Url(hash);
            }
        }

        private static string EscapeIcsText(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value
                .Replace("\\", "\\\\")
                .Replace(";", "\\;")
                .Replace(",", "\\,")
                .Replace("\r\n", "\\n")
                .Replace("\n", "\\n")
                .Replace("\r", "\\n");
        }

        private static string ToBase64Url(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static byte[] FromBase64Url(string value)
        {
            string padded = value.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }

            return Convert.FromBase64String(padded);
        }

        private static bool FixedTimeEquals(string a, string b)
        {
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            int result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }

            return result == 0;
        }
    }

    public class AudienciaCalendarioPayload
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Lugar { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
        public int IdExpediente { get; set; }
        public int IdAtencion { get; set; }
        public DateTime ExpiraUtc { get; set; }
    }
}
