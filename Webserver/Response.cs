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
        private string headers;
        private bool sent;
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
            this.headers = "";
        }

        public void AddHeader(string header, string value)
        {
            this.headers += @"\r\n" + header + ": " + value;
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
                            this.headers += $"HTTP/1.1 200 OK\r\nContent-Length: {bodyStream.Length}\r\nConnection: close\r\n\r\n";
                            var headerArray = Encoding.UTF8.GetBytes(this.headers);

                            await response.WriteAsync(headerArray, 0, headerArray.Length);
                            await bodyStream.CopyToAsync(response);
                            await response.FlushAsync();
                        }
                    }
                }
            });
        }

        public void Send()
        {
            this.Send("200 OK");
        }
    }
}
