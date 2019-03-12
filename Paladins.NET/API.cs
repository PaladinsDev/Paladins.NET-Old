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

        private JToken MakeRequest(string url)
        {
            var response = this.client.DownloadString(url);

            return JValue.Parse(response);
        }
    }
}
