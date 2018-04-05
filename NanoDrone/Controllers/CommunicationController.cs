using NanoDrone.Devices;
using NanoDrone.RouteSets;
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

            new MotorRoutes(webserver, this.drone);
            new OrientationRoutes(webserver, this.drone);
        }
    }
}
