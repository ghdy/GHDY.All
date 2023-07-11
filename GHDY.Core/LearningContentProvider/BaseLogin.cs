using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace GHDY.Core.LearningContentProviderCore
{
    public abstract class BaseLogin
    {
        public CookieContainer CookieContainer { get; set; }
        public Uri LoginUrl { get; set; }
        public bool Logined { get; set; }

        public abstract string GetLoginID();
        public abstract string Login();
        public abstract string GetFileNameByUrl(string url);
        public abstract void DownloadFile(string targetUrl, string fileName);

        public string CurrentCookieHeader
        {
            get
            {
                if (this.CookieContainer.Count > 0)
                    return this.CookieContainer.GetCookieHeader(LoginUrl);
                else
                    return string.Empty;
            }
        }

        private static readonly List<BaseLogin> _Logins = new List<BaseLogin>();
        public static BaseLogin GetLogin(string id)
        {
            //if (_Logins.Count < 1)
            //{
            //    var login = new LoginUNSV();
            //    login.Login();
            //    _Logins.Add(login);
            //}

            foreach (var login in _Logins)
            {
                if (string.Equals(login.GetLoginID(), id) == true)
                    return login;
            }

            return null;
        }

        public static void AddLogin(BaseLogin loginModel)
        {
            foreach (var login in _Logins)
            {
                if (string.Equals(login.GetLoginID(), loginModel.GetLoginID()) == true)
                    return;
            }

            if (loginModel.Logined == false)
                loginModel.Login();

            _Logins.Add(loginModel);
        }

        protected string LoginProcess(string postData, bool audioRedirect)
        {
            this.CookieContainer = new CookieContainer();

            var responseHtml = "response null!";
            //postData += string.Format("&{0}={1}", _userAutoLoginKey, "0");

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] postDataByte = encoding.GetBytes(postData);
            //var postDataEncodedString = encoding.GetString(postDataByte);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.LoginUrl);
            request.Method = "Post";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postDataByte.Length;
            request.KeepAlive = true;
            request.CookieContainer = this.CookieContainer;
            request.AllowAutoRedirect = audioRedirect;
            

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(postDataByte, 0, postDataByte.Length);
                //requestStream.Flush();
                requestStream.Close();
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                //response.Cookies = cookieContainer.GetCookies(request.RequestUri);
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                responseHtml = reader.ReadToEnd();

                response.Close();
            }
            return responseHtml;
        }

        protected void DownloadFileProcess(string targetUrl, string fileName, CookieContainer container)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(targetUrl);
            request.AllowAutoRedirect = true;
            request.CookieContainer = container;
            using (WebResponse response = request.GetResponse())
            {
                using (Stream reader = response.GetResponseStream())
                {
                    int blocksize = 1024 * 100;
                    byte[] buffer = new byte[blocksize];

                    using (FileStream writer = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        int c = 0;
                        while ((c = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            writer.Write(buffer, 0, c);
                        }
                    }
                }
                response.Close();
            }
        }
    }
}
