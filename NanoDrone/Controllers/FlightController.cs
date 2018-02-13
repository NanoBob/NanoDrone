using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NanoDrone.Sensors;
using NanoDrone.Devices;
using NanoDrone.Constants;
using Adafruit.Pwm;
using System.Diagnostics;
using System.Threading;
using System.Runtime;

namespace NanoDrone.Controllers
{
    class FlightController
    {
        private Drone drone;
        public Dictionary<string, Motor> motorsBySide;
        public Dictionary<string, UltrasonicSensor> ultrasonicSensorsBySide;
        private PwmController hat;


        private double throttle;
        private double pitchPower;
        private double yawPower;
        private double rollPower;

        public FlightController(Drone drone)
        {
            this.drone = drone;
            this.throttle = 0;
            this.pitchPower = 0;
            this.yawPower = 0;
            this.rollPower = 0;

            InitSensors();
            InitMotors();


        }

        public void InitSensors()
        {
            ultrasonicSensorsBySide = new Dictionary<string, UltrasonicSensor>();
            ultrasonicSensorsBySide.Add("bottom",new UltrasonicSensor(23, 24));
        }

        public async void InitMotors()
        {
            Debug.WriteLine("Initialising motors");
            motorsBySide = new Dictionary<string, Motor>();

            Debug.WriteLine("Initialising driver");
            hat = new PwmController();
            hat.SetDesiredFrequency(90);

            var motor = new Motor(hat, 0, MotorDirection.counterClockwise);
            motorsBySide.Add("frontLeft", motor);

            motor = new Motor(hat, 4, MotorDirection.clockwise);
            motorsBySide.Add("frontRight", motor);

            motor = new Motor(hat, 8, MotorDirection.counterClockwise);
            motorsBySide.Add("rearLeft", motor);

            motor = new Motor(hat, 12, MotorDirection.clockwise);
            motorsBySide.Add("rearRight", motor);
            Debug.WriteLine("Initialized motors");

            ArmMotors();
        }

        public void ArmMotors()
        {
            Debug.WriteLine("Arming motors");
            foreach (KeyValuePair<string, Motor> kv in motorsBySide)
            {
                Motor motor = kv.Value;
                //motor.Arm();
                new Task(motor.Arm).Start();
            }
            Debug.WriteLine("Armed motors");
        }

        public void ControlMotors()
        {
            Motor motor = null;
            motorsBySide.TryGetValue("frontLeft", out motor);
            if (motor != null)
            {
                motor.Run(this.throttle + this.pitchPower + this.rollPower + this.yawPower);
            }

            motor = null;
            motorsBySide.TryGetValue("frontRight", out motor);
            if (motor != null)
            {
                motor.Run(this.throttle + this.pitchPower - this.rollPower - this.yawPower);
            }

            motor = null;
            motorsBySide.TryGetValue("rearLeft", out motor);
            if (motor != null)
            {
                motor.Run(this.throttle - this.pitchPower + this.rollPower + this.yawPower);
            }

            motor = null;
            motorsBySide.TryGetValue("rearRight", out motor);
            if (motor != null)
            {
                motor.Run(this.throttle - this.pitchPower - this.rollPower - this.yawPower);
            }

        }

        public void Pitch(double power)
        {
            this.pitchPower = power;
            ControlMotors();
        }

        public void Roll(double power)
        {
            this.rollPower = power;
            ControlMotors();
        }

        public void Yaw(double power)
        {
            this.yawPower = power;
            ControlMotors();
        }

        public void Throttle(double power)
        {
            this.throttle = power;
            ControlMotors();
        }

        public void shutDown()
        {
            Throttle(0);
            Pitch(0);
            Roll(0);
            Yaw(0);
        }

    }
}
