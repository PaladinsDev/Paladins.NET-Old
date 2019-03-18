using System.Net;
using Newtonsoft.Json.Linq;

namespace PaladinsDev.PaladinsDotNET
{
    public class API
    {
        private static API instance;

        private string DevId;
        private string AuthKey;
        private int LanguageId;
        private Cache cache;
        private WebClient client;

        public API(string id, string key)
        {
            this.DevId = id;
            this.AuthKey = key;
            this.LanguageId = 1;
            this.client = new WebClient { Proxy = null };
            this.cache = new Cache();
        }

        public static API Instance(string id, string key)
        {
            if (instance == null)
            {
                instance = new API(id, key);
            }

            return instance;
        }

        private string GetTimestamp()
        {
            return (System.DateTime.UtcNow.ToString("yyyyMMddHHmm") + "00");
        }

        private string GetSignature(string input)
        {
            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            bytes = md5.ComputeHash(bytes);
            var output = new System.Text.StringBuilder();

            foreach(byte b in bytes)
            {
                output.Append(b.ToString("x2").ToLower());
            }

            return output.ToString();
        }
        
        // TODO: Get session before I finish this method.
        // TODO: Do Cache.cs before I do session method.

        private string BuildUrl(string method, 
                                string player = null, 
                                int language = -1, 
                                int matchId = -1, 
                                int champId = -1, 
                                int queue = -1, 
                                int tier = -1, 
                                int season = -1, 
                                int platform = -1)
        {
            // TODO: Finish this URL
            string baseUrl = $"{Variables.API_URL}/{method}Json/{this.DevId}/{this.GetSignature(this.DevId + method + this.AuthKey + this.GetTimestamp())}/{this.GetSession()}/{this.GetTimestamp()}";

            if (platform > -1)
            {
                baseUrl += $"/{platform}";
            }

            if (player != null)
            {
                baseUrl += $"/{player}";
            }

            if (champId > -1)
            {
                baseUrl += $"/{champId}";
            }

            if (language > -1)
            {
                baseUrl += $"/{language}";
            }

            if (matchId > -1)
            {
                baseUrl += $"/{matchId}";
            }

            if (queue > -1)
            {
                baseUrl += $"/{queue}";
            }

            if (tier > -1)
            {
                baseUrl += $"/{tier}";
            }


            if (season > -1)
            {
                baseUrl += $"/{season}";
            }
            return baseUrl;
        }

        private JToken MakeRequest(string url)
        {
            var response = this.client.DownloadString(url);

            return JValue.Parse(response);
        }
    }
}
