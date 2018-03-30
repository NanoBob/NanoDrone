using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NanoDrone.Sensors;
using System.Diagnostics;
using Windows.Devices.Gpio;
using System.Threading;
using Adafruit.Pwm;

namespace NanoDrone.Controllers
{
    public class NanoDrone : Drone
    {
        private MotorController motorController;
        private OrientationController orientationController;
        public MotorController MotorController
        {
            get
            {
                return motorController;
            }
        }
        public OrientationController OrientationController
        {
            get
            {
                return orientationController;
            }
        }

        public NanoDrone()
        {

            orientationController = new OrientationController(this);
            motorController = new MotorController(this);

        }

        ~NanoDrone()
        {
            this.motorController.ShutDown();
        }

        public void Stop()
        {
            this.motorController.ShutDown();
        }
    }
}
