using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NanoDrone.Constants;
using NanoDrone.Devices;
using NanoDrone.Sensors;

namespace NanoDrone.Controllers
{
    class MissionController
    {
        private Drone drone;

        public MissionController(Drone drone)
        {
            this.drone = drone;
        }
    }
}
