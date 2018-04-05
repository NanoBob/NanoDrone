using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NanoDrone.Controllers;
using NanoDrone.Devices;
using Webserver;

namespace NanoDrone.RouteSets
{
    class OrientationRoutes: RouteSet
    {
        public OrientationRoutes(Webserver.Webserver server, Controllers.NanoDrone drone) : base(server)
        {
            server.AddRoute(new Route("orientation/yaw", (Request request) => {
                return drone.OrientationController.Yaw.ToString();
            }));
            server.AddRoute(new Route("orientation/pitch", (Request request) => {
                return drone.OrientationController.Pitch.ToString();
            }));
            server.AddRoute(new Route("orientation/roll", (Request request) => {
                return drone.OrientationController.Roll.ToString();
            }));

            server.AddRoute(new Route("orientation/all", (Request request) =>
            {
                return string.Format("{0}|{1}|{2}", drone.OrientationController.Yaw, drone.OrientationController.Pitch, drone.OrientationController.Roll);
            }));
        }
    }
}
