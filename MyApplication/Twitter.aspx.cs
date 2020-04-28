using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MyApplication
{
    public partial class Twitter : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        public IList<TwitterSearch> GetTweets(string searchString)
        {
            string consumerKey = "XbX1kiWS1f9NFzSibUMsZXIJd";
            string consumerSecret = "ruuVzqoIIAdU8jZhFI0QMm9LEUHMUpbQpaS4JBenhBJb6vxO7n";
            string accessToken = "";
            string accessTokenSecret = "";

            IList<TwitterSearch> searchResults = new List<TwitterSearch>();
            string URL = "https://api.twitter.com/1.1/search/tweets.json";
            Dictionary<string, string> parameters = new Dictionary<string, string>() {
                { "q", searchString },{ "result_type", "popular" } };

            WebRequest request = CreateRequest(
                consumerKey, consumerSecret, accessToken, accessTokenSecret,
                "GET", URL, parameters);

            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        //Label1.Text = reader.ReadToEnd();
                        string jsonString = reader.ReadToEnd();

                        JObject twitterSearch = JObject.Parse(jsonString);

                        // get JSON result objects into a list  
                        IList<JToken> results = twitterSearch["statuses"].Children().ToList();

                        foreach (JToken result in results)
                        {
                            var twittSearch = result.ToObject<TwitterSearch>();
                            twittSearch.user_name = result["user"]["name"].ToString() ;
                            searchResults.Add(twittSearch);
                            //Label1.Text   += result.ToString();   
                        }
                        

                    }
                }
            }
            return searchResults;
        }
        public class TwitterSearch
        {
            public string created_at { get; set; }
            public string id { get; set; }
            public string text { get; set; }
            public string favorite_count { get; set; }
            public string retweet_count { get; set; }
            public string user_name { get; set; }

        }
        static string EncodeParameters(Dictionary<string, string> parameters)
        {
            if (parameters.Count == 0)
                return string.Empty;
            Dictionary<string, string>.KeyCollection.Enumerator keys = parameters.Keys.GetEnumerator();
            keys.MoveNext();
            StringBuilder sb = new StringBuilder(
                string.Format("{0}={1}", keys.Current, Uri.EscapeDataString(parameters[keys.Current])));
            while (keys.MoveNext())
                sb.AppendFormat("&{0}={1}", keys.Current, Uri.EscapeDataString(parameters[keys.Current]));
            return sb.ToString();
        }
        static public WebRequest CreateRequest(
           string consumerKey, string consumerSecret, string accessToken, string accessKey,
           string method, string url, Dictionary<string, string> parameters)
        {
            string encodedParams = EncodeParameters(parameters);

            WebRequest request;
            if (method == "GET")
                request = WebRequest.Create(string.Format("{0}?{1}", url, encodedParams));
            else
                request = WebRequest.Create(url);

            request.Method = method;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add(
                "Authorization",
                MakeOAuthHeader(consumerKey, consumerSecret, accessToken, accessKey, method, url, parameters));

            if (method == "POST")
            {
                byte[] postBody = new ASCIIEncoding().GetBytes(encodedParams);
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(postBody, 0, postBody.Length);
                }
            }

            return request;
        }
        static string MakeOAuthHeader(string consumerKey, string consumerSecret, string accessToken, string accessKey,
                   string method, string url, Dictionary<string, string> parameters)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            string oauth_consumer_key = consumerKey;
            string oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            string oauth_signature_method = "HMAC-SHA1";
            string oauth_token = accessToken;
            string oauth_timestamp = Convert.ToInt64(ts.TotalSeconds).ToString();
            string oauth_version = "1.0";

            SortedDictionary<string, string> sd = new SortedDictionary<string, string>();
            if (parameters != null)
                foreach (string key in parameters.Keys)
                    sd.Add(key, Uri.EscapeDataString(parameters[key]));
            sd.Add("oauth_version", oauth_version);
            sd.Add("oauth_consumer_key", oauth_consumer_key);
            sd.Add("oauth_nonce", oauth_nonce);
            sd.Add("oauth_signature_method", oauth_signature_method);
            sd.Add("oauth_timestamp", oauth_timestamp);
            sd.Add("oauth_token", oauth_token);

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}&{1}&", method, Uri.EscapeDataString(url));
            foreach (KeyValuePair<string, string> entry in sd)
                sb.Append(Uri.EscapeDataString(string.Format("{0}={1}&", entry.Key, entry.Value)));
            string baseString = sb.ToString().Substring(0, sb.Length - 3);

            string oauth_token_secret = accessKey;
            string signingKey = string.Format(
                "{0}&{1}", Uri.EscapeDataString(consumerSecret), Uri.EscapeDataString(oauth_token_secret));
            HMACSHA1 hasher = new HMACSHA1(new ASCIIEncoding().GetBytes(signingKey));
            string oauth_signature = Convert.ToBase64String(hasher.ComputeHash(new ASCIIEncoding().GetBytes(baseString)));

            sb = new StringBuilder("OAuth ");
            sb.AppendFormat("oauth_consumer_key=\"{0}\",", Uri.EscapeDataString(oauth_consumer_key));
            sb.AppendFormat("oauth_nonce=\"{0}\",", Uri.EscapeDataString(oauth_nonce));
            sb.AppendFormat("oauth_signature=\"{0}\",", Uri.EscapeDataString(oauth_signature));
            sb.AppendFormat("oauth_signature_method=\"{0}\",", Uri.EscapeDataString(oauth_signature_method));
            sb.AppendFormat("oauth_timestamp=\"{0}\",", Uri.EscapeDataString(oauth_timestamp));
            sb.AppendFormat("oauth_token=\"{0}\",", Uri.EscapeDataString(oauth_token));
            sb.AppendFormat("oauth_version=\"{0}\"", Uri.EscapeDataString(oauth_version));

            return sb.ToString();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if  (!string.IsNullOrEmpty( txtSearch.Text))
            {

            
                var results=GetTweets(txtSearch.Text);
                //foreach (var item in results)
                //{

                //}
                grdResults.DataSource = results;
                grdResults.DataBind();
            }
        }
    }
}
        
           