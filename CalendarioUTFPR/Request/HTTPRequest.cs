using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace CalendarioUTFPR.Request
{
    public class HTTPRequest
    {
        private const string URL = "https://moodle.utfpr.edu.br/login/index.php";
        private const string METHOD = "POST";
        private bool REMEMBER = true;
        private string user, pass;

        public HTTPRequest(string user, string pass)
        {
            this.user = user;
            this.pass = pass;
        }

        public CookieCollection requestToken()
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("username", user);
            nvc.Add("password", pass);
            nvc.Add("rememberusername", REMEMBER ? "1" : "0");
            nvc.Add("anchor", "");
            var parameters = new StringBuilder();
            foreach(string key in nvc.Keys)
            {
                parameters.AppendFormat("{0}={1}&", key, nvc[key]);
            }

            byte[] buffer = Encoding.ASCII.GetBytes(parameters.ToString());

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = METHOD;
            request.AllowAutoRedirect = true;
            //request.Headers["origin"] = "200.19.73.23";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Host = "moodle.utfpr.edu.br";
            request.Headers["Cache-Control"] = "no-cache";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36";
            request.ContentLength = buffer.Length;

            CookieContainer ck = new CookieContainer();
            request.CookieContainer = ck;

            Stream data = request.GetRequestStream();
            data.Write(buffer, 0, buffer.Length);
            data.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream respStream = response.GetResponseStream();
            StreamReader sr = new StreamReader(respStream);
            File.WriteAllText("response.html", sr.ReadToEnd());
            response.Cookies = request.CookieContainer.GetCookies(request.RequestUri);
            CookieCollection cook = response.Cookies;
            response.Close();
            return cook;
        }
    }
}
