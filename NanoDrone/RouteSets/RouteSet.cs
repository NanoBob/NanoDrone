using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDrone.RouteSets
{
    class RouteSet
    {
        protected Webserver.Webserver server;

        public RouteSet(Webserver.Webserver server)
        {
            this.server = server;
        }

    }
}
