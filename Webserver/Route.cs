using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webserver
{
    public class Route
    {
        List<string> path;
        Func<Request, string> handler;

        public Route(string path, Func<Request, string> handler)
        {
            this.handler = handler;
            this.path = new List<string>(path.Split('/'));
        }

        public bool MatchesPath(Request request)
        {
            for (int i = 0; i < request.path.Count && i < this.path.Count; i++)
            {
                string split = request.path[i];
                if (! this.path[i].StartsWith(":") && this.path[i] != split)
                {
                    return false;
                }
            }
            return true;
        }

        public string Handle(Request request)
        {
            return this.handler.Invoke(request);
        }
    }
}
