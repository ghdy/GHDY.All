using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Core
{
    public class DownloadClient
    {
        public const string UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.146 Safari/537.36";

        public TimeSpan Timeout { get; private set; }

        public long TotalBytesToReceive { get; private set; }

        public event EventHandler<AsyncCompletedEventArgs> DownloadFileCompleted;
        public event EventHandler<DownloadedChangedEventArgs> DownloadProgressChanged;

        public DownloadClient()
            : this(TimeSpan.FromMinutes(2))
        {
            this.Timeout = TimeSpan.FromMinutes(2);
        }

        public DownloadClient(TimeSpan timeout)
        {
            this.Timeout = timeout;
        }

        #region DownloadFileAsync
        public async Task<bool> DownloadFileAsync(string downloadingFilePath, Uri url)
        {
            this.TotalBytesToReceive = 0;

            if (url == null)
            {
                this.DownloadFileCompleted?.Invoke(this, new AsyncCompletedEventArgs(null, true, downloadingFilePath));
            }

            try
            {
                byte[] array = await ReadFileAsync(url);

                using (FileStream fstr = new FileStream(downloadingFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fstr.Write(array, 0, array.Length);
                }
                return true;
            }
            catch (WebException webEx)
            {
                File.Delete(downloadingFilePath);
                this.DownloadFileCompleted?.Invoke(this, new AsyncCompletedEventArgs(webEx, true, downloadingFilePath));
            }
            catch (IOException ioEx)
            {
                File.Delete(downloadingFilePath);
                this.DownloadFileCompleted?.Invoke(this, new AsyncCompletedEventArgs(ioEx, true, downloadingFilePath));
            }
            catch (Exception ex)
            {
                File.Delete(downloadingFilePath);
                throw new Exception("Error in DownloadClient.cs, " + ex.Message, ex);
            }

            return false;
        }

        private async Task<byte[]> ReadFileAsync(Uri url)
        {
            byte[] result = null;

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Timeout = (int)this.Timeout.TotalMilliseconds;
            request.UserAgent = UserAgent;
            request.AllowAutoRedirect = true;

            //var result = new MemoryStream();
            using (var response = await request.GetResponseAsync())
            {

                var redirectUrl = response.ResponseUri;
                var contentType = response.ContentType;

                using (Stream receiveStream = response.GetResponseStream())
                {
                    //await receiveStream.CopyToAsync(result);

                    //receiveStream.CopyTo(result);

                    this.TotalBytesToReceive = response.ContentLength;
                    result = new byte[response.ContentLength];
                    int bytesToRead = (int)result.Length;
                    int _maxLength = bytesToRead;

                    int bytesRead = 0;
                    while (bytesToRead > 0)
                    {
                        int n = receiveStream.Read(result, bytesRead, bytesToRead);
                        if (n == 0)
                            break;

                        bytesRead += n;
                        bytesToRead -= n;
                        //Console.WriteLine("Readed:" + bytesRead);

                        if (this.DownloadProgressChanged != null)
                        {

                            var args = new DownloadedChangedEventArgs(bytesRead, (int)this.TotalBytesToReceive);

                            this.DownloadProgressChanged(this, args);
                        }
                    }
                    //using (FileStream fstr = new FileStream(downloadingFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                    //{
                    //    fstr.Write(inBuf, 0, bytesRead);
                    //}
                }
            }

            this.DownloadFileCompleted?.Invoke(this, new AsyncCompletedEventArgs(null, false, null));


            return result;// result.ToArray();
        }
        #endregion

        public static bool UrlIsExist(Uri url, out Uri redirectUrl)
        {
            redirectUrl = null;

            bool result = false;
            HttpWebRequest httpRequest = WebRequest.Create(url) as HttpWebRequest;
            httpRequest.UserAgent = UserAgent;
            httpRequest.Timeout = (int)TimeSpan.FromSeconds(60).TotalMilliseconds;
            //httpRequest.Method = "HEAD";
            try
            {
                HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
                redirectUrl = httpResponse.ResponseUri;

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                    return true;
            }
            catch
            {
                result = false;
                //try
                //{
                //    var code = (exception.Response as HttpWebResponse).StatusCode;
                //    result = code != HttpStatusCode.NotFound;
                //}
                //catch
                //{
                //    var code = (exception.Response as HttpWebResponse).StatusCode;

                //    result = exception.Status == WebExceptionStatus.Success;
                //}
            }

            return result;
        }
    }

    public class DownloadedChangedEventArgs
    {
        public DownloadedChangedEventArgs(int received, int total)
        {
            this.BytesReceived = received;
            this.TotalBytesToReceive = total;
        }

        public int BytesReceived { get; private set; }
        public int TotalBytesToReceive { get; private set; }

        public int ProgressPercentage
        {
            get
            {
                var temp = (double)this.BytesReceived;
                var percentage = temp * 100 / this.TotalBytesToReceive;
                return (int)percentage;
            }
        }
    }
}
