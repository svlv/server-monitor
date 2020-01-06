using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System;

namespace Bitcoin
{
    public class RpcClient
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Uri { get; set; }
        private int _id = 1;

        public string SendRequest(string method, JArray _params)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Uri);
            webRequest.Credentials = new NetworkCredential(UserName, Password);
            webRequest.ContentType = "application/json-rpc";
            webRequest.Method = "POST";

            JObject joe = new JObject();
            joe.Add(new JProperty("jsonrpc", "1.0"));
            joe.Add(new JProperty("id", _id.ToString()));
            joe.Add(new JProperty("method", method));
            joe.Add(new JProperty("params", _params));

            string s = JsonConvert.SerializeObject(joe);
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            webRequest.ContentLength = byteArray.Length;
            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse webResponse = webRequest.GetResponse();
            Stream responseStream = webResponse.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            ++_id;

            return reader.ReadToEnd();
        }

        public int GetBlockCount()
        {
            try
            {
                var response = SendRequest("getblockcount", new JArray());
                JObject resp = JObject.Parse(response);
                return System.Convert.ToInt32(resp.SelectToken("result"));
            }
            catch(Exception)
            {
            }

            return 0;
        }
    }
}
