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
            /* Routes for getting the current orientation */
            AddRoute(new Route("orientation/yaw", (Request request) => {
                return drone.OrientationController.Yaw.ToString();
            }));
            AddRoute(new Route("orientation/pitch", (Request request) => {
                return drone.OrientationController.Pitch.ToString();
            }));
            AddRoute(new Route("orientation/roll", (Request request) => {
                return drone.OrientationController.Roll.ToString();
            }));

            AddRoute(new Route("orientation/all", (Request request) =>
            {
                return string.Format("{0}|{1}|{2}", drone.OrientationController.Yaw, drone.OrientationController.Pitch, drone.OrientationController.Roll);
            }));

            /* routes for enabling, disabling and getting auto mode */

            AddRoute(new Route("orientation/auto/enable", (Request request) =>
            {
                bool result = drone.OrientationController.Enable();
                return string.Format("Enabling orientation assist, result: {0}", result);
            }));

            AddRoute(new Route("orientation/auto/disable", (Request request) =>
            {
                bool result = drone.OrientationController.Disable();
                return string.Format("Disabling orientation assit, result: {0}", result);
            }));

            AddRoute(new Route("orientation/auto", (Request request) =>
            {
                return drone.OrientationController.Running.ToString();
            }));
        }
    }
}
