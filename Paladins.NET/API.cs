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

        public JToken GetServerStatus()
        {
            return this.MakeRequest(this.BuildUrl("gethirezserverstatus"));
        }

        public JToken GetPatchInfo()
        {
            return this.MakeRequest(this.BuildUrl("getpatchinfo"));
        }

        public JToken GetTopMatches()
        {
            return this.MakeRequest(this.BuildUrl("gettopmatches"));
        }

        public JToken GetRankedLeaderboard(int tier, int season, int queue)
        {
            return this.MakeRequest(this.BuildUrl("getleagueleaderboard", null, -1, -1, -1, queue, tier, season));
        }

        public JToken GetRankedSeasons(int queue)
        {
            return this.MakeRequest(this.BuildUrl("getleagueseasons", null, -1, -1, -1, queue));
        }

        public JToken GetMatchIdsByQueue<T>(string hour, T date, int queue = 424)
        {
            string url = $"{Variables.API_URL}/getmatchiidsbyqueueJson/{this.DevId}/" + this.GetSignature(this.DevId + "getmatchidsbyqueue" + this.AuthKey + this.GetTimestamp()) + $"/{this.GetSession()}/{this.GetTimestamp()}/{queue}/{date}/{hour}";

            return this.MakeRequest(url);
        }

        public JToken GetChampions()
        {
            return this.MakeRequest(this.BuildUrl("getchampions", null, this.LanguageId));
        }

        public JToken GetChampionCards(int championId)
        {
            return this.MakeRequest(this.BuildUrl("getchampioncards", null, this.LanguageId, -1, championId));
        }

        public JToken GetChampionSkins(int championId)
        {
            return this.MakeRequest(this.BuildUrl("getchampionskins", null, this.LanguageId, -1, championId));
        }

        public JToken GetItems()
        {
            return this.MakeRequest(this.BuildUrl("getitems", null, this.LanguageId));
        }

        public JToken GetPlayer(string name)
        {
            return this.MakeRequest(this.BuildUrl("getplayer", name));
        }

        public JToken GeyPlayerIdByName(string name)
        {
            return this.MakeRequest(this.BuildUrl("getplayeridbyname", name));
        }

        public JToken GetPlayerIdByPortalUserId(string name, int platform)
        {
            return this.MakeRequest(this.BuildUrl("getplayeridbyportaluserid", name, -1, -1, -1, -1, -1, -1, platform));
        }

        public JToken GetPlayerIdsByGamertag(string name, int platform)
        {
            return this.MakeRequest(this.BuildUrl("getplayeridsbygamertag", name, -1, -1, -1, -1, -1, -1, platform));
        }

        public JToken GetPlayerIdInfoForXboxAndSwitch(string name)
        {
            return this.MakeRequest(this.BuildUrl("getplayeridinfoforxboxandswitch", name));
        }

        public JToken GetPlayerFriends(string playerId)
        {
            return this.MakeRequest(this.BuildUrl("getfriends", playerId));
        }

        public JToken GetPlayerChampionRanks(string playerId)
        {
            return this.MakeRequest(this.BuildUrl("getchampionranks", playerId));
        }

        public JToken GetPlayerLoadouts(string playerId)
        {
            return this.MakeRequest(this.BuildUrl("getplayerloadouts", playerId, this.LanguageId));
        }

        public JToken GetPlayerStatus(string playerId)
        {
            return this.MakeRequest(this.BuildUrl("getplayerstatus", playerId));
        }

        public JToken GetPlayerMatchHistory(string playerId)
        {
            return this.MakeRequest(this.BuildUrl("getmatchhistory", playerId));
        }

        public JToken GetPlayerQueueStats(string playerId, int queue)
        {
            return this.MakeRequest(this.BuildUrl("getqueuestats", playerId, -1, -1, -1, queue));
        }

        public JToken GetMatchModeDetails(int matchId)
        {
            return this.MakeRequest(this.BuildUrl("getmodedetails", null, -1, matchId));
        }

        public JToken GetMatchDetails(int matchId)
        {
            return this.MakeRequest(this.BuildUrl("getmatchdetails", null, -1, matchId));
        }

        public JToken GetActiveMatchDetails(int matchId)
        {
            return this.MakeRequest(this.BuildUrl("getmatchplayerdetails", null, -1, matchId));
        }

        public JToken GetDataUsage()
        {
            return this.MakeRequest(this.BuildUrl("getdataused"));
        }

        private string GetSession()
        {
            return this.cache.Remember(Variables.CACHE_SESSION_ID, 12, GetSessionFromAPI).ToString();
        }

        private string GetSessionFromAPI()
        {
            string url = $"{Variables.API_URL}/createsessionjson/{this.DevId}/{this.GetSignature(this.DevId + "createsession" + this.AuthKey + this.GetTimestamp())}/{this.GetTimestamp()}";

            JToken resp = this.MakeRequest(url);

            if (resp.SelectToken("ret_msg").ToString() != "Approved" || resp.SelectToken("session_id") == null)
            {
                throw new System.Exception(resp.SelectToken("ret_msg").ToString());
            } else
            {
                return resp.SelectToken("session_id").ToString();
            }
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
