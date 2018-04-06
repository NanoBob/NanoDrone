using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Webserver
{
    public class Response
    {
        private IOutputStream stream;
        private Dictionary<string, string> headers;
        private bool sent;
        private int statusCode;
        private string reasonPhrase;
        public bool Sent
        {
            get
            {
                return sent;
            }
        }

        public Response(IOutputStream stream)
        {
            this.stream = stream;
            this.sent = false;
            this.headers = new Dictionary<string, string>();
            this.headers.Add("Connection", "close");
            this.statusCode = 200;
            this.reasonPhrase = "OK";

        }

        public void AddHeader(string header, string value)
        {
            this.headers.Add(header, value);
        }

        public void SetResponseStatus(int code, string reason = "")
        {
            this.statusCode = code;
            this.reasonPhrase = reason;
        }

        public void Send(string body)
        {
            if (this.sent)
            {
                return;
            }
            this.sent = true;
            Task.Run(async () =>
            {
                using (var output = stream)
                {
                    using (var response = output.AsStreamForWrite())
                    {
                        var html = Encoding.UTF8.GetBytes(body);
                        using (var bodyStream = new MemoryStream(html))
                        {
                            this.headers.Add("Content-length", bodyStream.Length.ToString());
                            string headersString = string.Format("HTTP/1.1 {0} {1}\r\n", this.statusCode, this.reasonPhrase);
                            foreach(KeyValuePair<string, string> kvPair in this.headers)
                            {
                                headersString += string.Format("{0}: {1}\r\n", kvPair.Key, kvPair.Value);
                            }
                            // $"HTTP/1.1 200 OK\r\nContent-Length: {bodyStream.Length}\r\nConnection: close\r\n\r\n";
                            var headerArray = Encoding.UTF8.GetBytes(headersString + "\r\n");

                            await response.WriteAsync(headerArray, 0, headerArray.Length);
                            await bodyStream.CopyToAsync(response);
                            await response.FlushAsync();
                        }
                    }
                }
            });
        }

        public void Send(string body, int code, string reason = "")
        {
            this.SetResponseStatus(code, reason);
            this.Send();
        }

        public void Send()
        {
            this.Send("");
        }

    }
}
