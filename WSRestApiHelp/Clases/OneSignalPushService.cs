using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;

namespace WSRestApiHelp.Clases
{
    public class OneSignalPushService
    {
        private readonly string appId;
        private readonly string restApiKey;

        public OneSignalPushService()
        {
            appId = ConfigurationManager.AppSettings["OneSignalAppId"];
            restApiKey = ConfigurationManager.AppSettings["OneSignalRestApiKey"];
        }

        public bool IsConfigured
        {
            get
            {
                return !String.IsNullOrWhiteSpace(appId) &&
                       !String.IsNullOrWhiteSpace(restApiKey);
            }
        }

        public OneSignalSendResult SendToPlayer(string playerId, string title, string message, Dictionary<string, object> data)
        {
            if (String.IsNullOrWhiteSpace(playerId))
            {
                throw new ArgumentException("No se recibio TokenPush de OneSignal.", "playerId");
            }

            if (!IsConfigured)
            {
                throw new InvalidOperationException("OneSignal no esta configurado. Debe definir OneSignalAppId y OneSignalRestApiKey en Web.config.");
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var payload = new
            {
                app_id = appId,
                include_player_ids = new[] { playerId },
                headings = new { en = title ?? "HELP Ajustadores", es = title ?? "HELP Ajustadores" },
                contents = new { en = message ?? "", es = message ?? "" },
                data = data ?? new Dictionary<string, object>()
            };

            var request = (HttpWebRequest)WebRequest.Create("https://onesignal.com/api/v1/notifications");
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("Authorization", "Key " + restApiKey);

            var json = JsonConvert.SerializeObject(payload);
            var bytes = Encoding.UTF8.GetBytes(json);
            request.ContentLength = bytes.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return new OneSignalSendResult
                    {
                        Success = true,
                        StatusCode = (int)response.StatusCode,
                        Body = reader.ReadToEnd()
                    };
                }
            }
            catch (WebException ex)
            {
                var body = "";
                if (ex.Response != null)
                {
                    using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        body = reader.ReadToEnd();
                    }
                }

                var httpResponse = ex.Response as HttpWebResponse;

                return new OneSignalSendResult
                {
                    Success = false,
                    StatusCode = httpResponse != null ? (int)httpResponse.StatusCode : 0,
                    Body = String.IsNullOrWhiteSpace(body) ? ex.Message : body
                };
            }
        }
    }

    public class OneSignalSendResult
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Body { get; set; }
    }
}
