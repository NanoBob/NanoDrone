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
    class MotorRoutes: RouteSet
    {
        public MotorRoutes(Webserver.Webserver server, Controllers.NanoDrone drone) : base(server)
        {
            AddRoute(new Route("motors/:side/speed", (Request request) => 
            {
                string side = request.path[1];
                var motors = drone.MotorController.motorsBySide;
                if (motors.TryGetValue(side, out Motor motor))
                {
                    return motor.Speed.ToString();
                }
                return "Motor not found";
            }));

            AddRoute(new Route("motors/test", (Request request) =>
            {
                Task.Run(() =>
                {
                    foreach (KeyValuePair<string, Motor> kvPair in drone.MotorController.motorsBySide)
                    {
                        kvPair.Value.Test();
                    }
                });
                return "";
            }));

            AddRoute(new Route("motors/:side/test", (Request request) =>
            {
                string side = request.path[1];
                var motors = drone.MotorController.motorsBySide;
                if (motors.TryGetValue(side, out Motor motor))
                {
                    Task.Run(() =>
                    {
                        motor.Test();
                    });
                    return "Test ran";
                }
                return "Motor not found";
            }));

            AddRoute(new Route("motors/:side/arm", (Request request) =>
            {
                string side = request.path[1];
                var motors = drone.MotorController.motorsBySide;
                if (motors.TryGetValue(side, out Motor motor))
                {
                    Task.Run(() =>
                    {
                        motor.Arm();
                    });
                    return "Armed";
                }
                return "Motor not found";
            }));
            
        }
    }
}
