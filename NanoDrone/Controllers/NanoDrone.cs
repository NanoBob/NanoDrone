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
                return this.motorController;
            }
        }

        public NanoDrone()
        {

            motorController = new MotorController(this);
            orientationController = new OrientationController(this);

        }

        ~NanoDrone()
        {
            this.motorController.ShutDown();
        }

        public void Stop()
        {
            this.motorController.ShutDown();
        }


        public void SensorLoop()
        {
            var keyValuePair = motorController.ultrasonicSensorsBySide.First();
            var sensor = keyValuePair.Value;
            var motors = motorController.motorsBySide;
            var firstMotor = motors.First().Value;
            while (true)
            {
                Debug.WriteLine("Triggering");
                sensor.Trigger();
                Task.Delay(500).Wait();
                if (sensor.LastDistance > 140)
                {
                    firstMotor.Stop();
                } else
                {
                    firstMotor.Run((200 - sensor.LastDistance) / 200);
                }
            }
        }

        public void RunMotor()
        {
            var motors = motorController.motorsBySide;

            while (true)
            {
                Debug.WriteLine("Loop");
                foreach(var kvPair in motors)
                {
                    Debug.WriteLine("Motor");
                    var motor = kvPair.Value;
                    motor.Run(0.8);
                    Task.Delay(250).Wait();

                    motor.Stop();
                    Task.Delay(500).Wait();
                }
            }
        }
    }
}
