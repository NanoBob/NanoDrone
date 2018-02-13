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
        private FlightController flightController;
        private MissionController missionController;

        public NanoDrone()
        {
            //Task.Run(() => this.Test()).Wait();

            flightController = new FlightController(this);
            missionController = new MissionController(this);



            //Task.Run(SensorLoop);
            //RunMotor();


            /*
            for (var i = 0; i < 100; i++)
            {
                flightController.Throttle(i * 0.1);
                Task.Delay(250);
            }
            flightController.shutDown();
            */

        }

        private async Task Test()
        {
            using (var hat = new Adafruit.Pwm.PwmController())
            {
                DateTime timeout = DateTime.Now.AddSeconds(60);
                hat.SetDesiredFrequency(60);
                while (timeout >= DateTime.Now)
                {
                    hat.SetPulseParameters(0, 300, false);
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    hat.SetPulseParameters(0, 480, false);
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                }
            }
        }

        public void Stop()
        {
            foreach(var keyValuePair in this.flightController.motorsBySide)
            {
                var motor = keyValuePair.Value;
                motor.Stop();
            }
        }


        public async Task SensorLoop()
        {
            var keyValuePair = flightController.ultrasonicSensorsBySide.First();
            var sensor = keyValuePair.Value;
            var motors = flightController.motorsBySide;
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

        public async Task RunMotor()
        {
            var motors = flightController.motorsBySide;

            while (true)
            {
                Debug.WriteLine("Loop");
                foreach(var kvPair in motors)
                {
                    Debug.WriteLine("Motor");
                    var motor = kvPair.Value;
                    motor.Run(0.8);
                    await Task.Delay(250);

                    motor.Stop();
                    await Task.Delay(500);
                }
            }
        }
    }
}
