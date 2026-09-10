using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WSRestApiHelp.Clases
{
    /// <summary>
    /// Rate limiting en memoria (ventana fija por minuto) para abuso de auth y endpoints sensibles ApiProtection.
    /// Defensa por nodo IIS; complementar con gateway/WAF para límite global.
    /// </summary>
    public class RateLimitHandler : DelegatingHandler
    {
        private static readonly ConcurrentDictionary<string, WindowCounter> Counters =
            new ConcurrentDictionary<string, WindowCounter>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Login y recuperación de clave (cualquier método).</summary>
        private static readonly string[] AuthAbusePaths =
        {
            "/api/login/autenticacion",
            "/api/login/recuperarpassword",
            "/api/login/validarclave",
            "/api/login/actualizarpassword",
            "/api/cliente/autenticacion",
            "/api/cliente/actualizarpasswordcliente"
        };

        /// <summary>Lectura / enumeración de PII o listados (GET o POST).</summary>
        private static readonly string[] SensitivePaths =
        {
            "/api/proveedor/obtenermisatenciones",
            "/api/proveedor/obtenermisatencionesactivas",
            "/api/proveedor/buscarmisatenciones",
            "/api/proveedor/obtenerdatosexpedientes",
            "/api/proveedor/obtenerdatosdeatencion",
            "/api/proveedor/obtenercachecliente",
            "/api/proveedor/vectorgpsatencion",
            "/api/proveedor/listadoajustadores",
            "/api/proveedor/listadoabogados",
            "/api/cliente/obtenermisatencionesactivas",
            "/api/cliente/obtenermisatencionesactivascliente",
            "/api/ficohsahn/consultar_asegurado_hn",
            "/api/ficohsahn/validardatosreclamobpm",
            "/api/ficohsahn/consultar_siniestro_requisitos_hn",
            "/api/ficohsahn/consultar_estado_sinietro",
            "/api/ficohsahn/consulta_causas_hn",
            "/api/ficohsagt/consultar_estado_sinietro"
        };

        /// <summary>Escrituras costosas / carga BPM (límite aparte, más bajo).</summary>
        private static readonly string[] HeavyWritePaths =
        {
            "/api/ficohsahn/carga_reclamo_sinau_bpm_fico",
            "/api/ficohsahn/carga_reclamo_sinau_bpm_ficoqa",
            "/api/ficohsahn/subir_archivos_wsfh",
            "/api/login/enviarnotificacionaccidente",
            "/api/login/subirvideo"
        };

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!IsEnabled())
            {
                return base.SendAsync(request, cancellationToken);
            }

            if (request.Method == HttpMethod.Options)
            {
                return base.SendAsync(request, cancellationToken);
            }

            string path = GetRelativePath(request);
            int limit;
            string bucket;

            if (MatchesAny(path, AuthAbusePaths))
            {
                limit = ReadInt("RateLimit_Login_PerMinute", 20);
                bucket = "auth";
            }
            else if (MatchesAny(path, HeavyWritePaths))
            {
                limit = ReadInt("RateLimit_HeavyWrite_PerMinute", 15);
                bucket = "heavy-write";
            }
            else if (MatchesAny(path, SensitivePaths))
            {
                limit = ReadInt("RateLimit_SensitiveGet_PerMinute", 60);
                bucket = "sensitive";
            }
            else
            {
                return base.SendAsync(request, cancellationToken);
            }

            string clientKey = GetClientKey(request);
            string counterKey = bucket + "|" + clientKey + "|" + DateTime.UtcNow.ToString("yyyyMMddHHmm");

            WindowCounter counter = Counters.GetOrAdd(counterKey, _ => new WindowCounter());
            int current = Interlocked.Increment(ref counter.Count);

            CleanupOldKeys();

            if (current > limit)
            {
                var response = request.CreateResponse((HttpStatusCode)429);
                response.ReasonPhrase = "Too Many Requests";
                response.Headers.TryAddWithoutValidation("Retry-After", "60");
                response.Content = new StringContent(
                    "{\"Message\":\"Demasiadas solicitudes. Intente nuevamente en un minuto.\"}",
                    System.Text.Encoding.UTF8,
                    "application/json");
                return Task.FromResult(response);
            }

            return base.SendAsync(request, cancellationToken);
        }

        private static bool IsEnabled()
        {
            string raw = ConfigurationManager.AppSettings["RateLimit_Enabled"];
            if (string.IsNullOrWhiteSpace(raw))
            {
                return true;
            }

            bool enabled;
            return !bool.TryParse(raw, out enabled) || enabled;
        }

        private static int ReadInt(string key, int fallback)
        {
            int value;
            return int.TryParse(ConfigurationManager.AppSettings[key], out value) && value > 0
                ? value
                : fallback;
        }

        private static string GetRelativePath(HttpRequestMessage request)
        {
            string path = request.RequestUri != null ? request.RequestUri.AbsolutePath : string.Empty;
            int apiIndex = path.IndexOf("/api/", StringComparison.OrdinalIgnoreCase);
            if (apiIndex >= 0)
            {
                path = path.Substring(apiIndex);
            }

            return path.TrimEnd('/').ToLowerInvariant();
        }

        private static bool MatchesAny(string path, IEnumerable<string> candidates)
        {
            foreach (string candidate in candidates)
            {
                if (path.Equals(candidate, StringComparison.OrdinalIgnoreCase)
                    || path.StartsWith(candidate + "/", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetClientKey(HttpRequestMessage request)
        {
            IEnumerable<string> values;
            if (request.Headers.TryGetValues("X-Forwarded-For", out values))
            {
                string forwarded = values.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(forwarded))
                {
                    return forwarded.Split(',')[0].Trim();
                }
            }

            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                var ctx = request.Properties["MS_HttpContext"] as System.Web.HttpContextWrapper;
                if (ctx != null && ctx.Request != null && !string.IsNullOrWhiteSpace(ctx.Request.UserHostAddress))
                {
                    return ctx.Request.UserHostAddress;
                }
            }

            return "unknown";
        }

        private static void CleanupOldKeys()
        {
            if (Counters.Count < 500)
            {
                return;
            }

            string currentMinute = DateTime.UtcNow.ToString("yyyyMMddHHmm");
            foreach (var key in Counters.Keys)
            {
                if (!key.EndsWith(currentMinute, StringComparison.Ordinal))
                {
                    WindowCounter removed;
                    Counters.TryRemove(key, out removed);
                }
            }
        }

        private sealed class WindowCounter
        {
            public int Count;
        }
    }
}
