using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Windows.Networking.Sockets;
using Windows.Networking;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;
using System.IO;

namespace Webserver
{

    public class Webserver
    {
        private const uint BufferSize = 8192;
        private StreamSocketListener listener;
        private List<Route> routes;

        public Webserver(int port)
        {
            this.routes = new List<Route>();
            this.listener = new StreamSocketListener();
            Debug.WriteLine("Creating webserver on port {0}", port);
            
            listener.BindServiceNameAsync(port.ToString());
            listener.ConnectionReceived += OnConnectionReceive;
            
        }

        public void AddRoute(Route route)
        {
            this.routes.Add(route);
        }

        private async Task<Request> ParseRequest(StreamSocketListenerConnectionReceivedEventArgs args)
        {
            var request = new StringBuilder();

            using (var input = args.Socket.InputStream)
            {
                var data = new byte[BufferSize];
                IBuffer buffer = data.AsBuffer();
                var dataRead = BufferSize;

                while (dataRead == BufferSize)
                {
                    await input.ReadAsync(
                         buffer, BufferSize, InputStreamOptions.Partial);
                    request.Append(Encoding.UTF8.GetString(
                                                  data, 0, data.Length));
                    dataRead = buffer.Length;
                }
            }
            return new Request(request.ToString());
        }

        private async void OnConnectionReceive(StreamSocketListener listener, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            try
            {
                Request request = await ParseRequest(args);

                Response response = new Response(args.Socket.OutputStream);

                string responseText = HandleRequest(request, response);

                response.Send(responseText);
            } catch (Exception)
            {

                Response response = new Response(args.Socket.OutputStream);

                response.Send("Internal Server Error", 500, "Internal Server Error");
            }
        }

        private string HandleRequest(Request request, Response response)
        {
            for (int i = 0; i < this.routes.Count; i++)
            {
                Route route = this.routes[i];
                if (route.MatchesPath(request))
                {
                    string responseText = route.Handle(request);
                    response.Send(responseText);
                }
            }
            return "";
        }

    }
}
