using NanoDrone.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webserver;

namespace NanoDrone.Controllers
{
    public class CommunicationController
    {
        Webserver.Webserver webserver;
        NanoDrone drone;

        public CommunicationController(NanoDrone drone)
        {
            this.drone = drone;
            webserver = new Webserver.Webserver(666);

            webserver.AddRoute(new Route("motors/:side/speed", (Request request) => {
                string side = request.path[1];
                var motors = drone.MotorController.motorsBySide;
                if (motors.TryGetValue(side, out Motor motor))
                {
                    return motor.Speed.ToString();
                }
                return "Motor not found";
            }));
        }
    }
}
