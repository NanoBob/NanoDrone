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
    class FlightRoutes: RouteSet
    {
        public FlightRoutes(Webserver.Webserver server, Controllers.NanoDrone drone) : base(server)
        {
            AddRoute(new Route("flight/test/:speed", (Request request) =>
            {
                Task.Run(() =>
                {
                    string speed = request.path[2];
                    double numericSpeed = double.Parse(speed);

                    for (double i = 0; i < numericSpeed / 100; i += 0.005)
                    {
                        drone.MotorController.Throttle(i);
                        Task.Delay(500).Wait();
                    }
                    for (double i = numericSpeed / 100; i >= 0; i -= 0.005)
                    {
                        drone.MotorController.Throttle(i);
                        Task.Delay(500).Wait();
                    }
                });

                return "";
            }));

            AddRoute(new Route("flight/shutdown", (Request request) =>
            {
                drone.Stop();

                return "";
            }));

            AddRoute(new Route("flight/start", (Request request) =>
            {
                drone.Start();

                return "";
            }));
        }
    }
}
