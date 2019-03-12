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
        }

        public static API Instance(string id, string key)
        {
            if (instance == null)
            {
                instance = new API(id, key);
            }

            return instance;
        }

        private string MakeSignature(string input)
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

        private JToken MakeRequest(string url)
        {
            var response = this.client.DownloadString(url);

            return JValue.Parse(response);
        }
    }
}
