using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webserver;

namespace NanoDrone.RouteSets
{
    class RouteSet
    {
        protected Webserver.Webserver server;

        private List<Route> routes;
        public RouteSet(Webserver.Webserver server)
        {
            this.server = server;
            this.routes = new List<Route>();
        }

        public void AddRoute(Route route)
        {
            this.routes.Add(route);
            this.server.AddRoute(route);
        }

    }
}
