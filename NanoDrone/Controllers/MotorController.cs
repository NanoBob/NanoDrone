using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NanoDrone.Sensors;
using NanoDrone.Devices;
using Adafruit.Pwm;
using System.Diagnostics;
using System.Threading;
using System.Runtime;

namespace NanoDrone.Controllers
{
    public class MotorController
    {
        private Drone drone;
        public Dictionary<string, Motor> motorsBySide;
        public Dictionary<string, UltrasonicSensor> ultrasonicSensorsBySide;
        private PwmController hat;


        private double throttle;
        private double pitchPower;
        private double yawPower;
        private double rollPower;

        public MotorController(NanoDrone drone)
        {
            this.drone = drone;
            this.throttle = 0;
            this.pitchPower = 0;
            this.yawPower = 0;
            this.rollPower = 0;

            InitSensors();
            InitMotors();
            Throttle(0);
        }

        public void InitSensors()
        {
            ultrasonicSensorsBySide = new Dictionary<string, UltrasonicSensor>();
            ultrasonicSensorsBySide.Add("bottom",new UltrasonicSensor(23, 24));
        }

        public void InitMotors()
        {
            Debug.WriteLine("Initialising motors");
            motorsBySide = new Dictionary<string, Motor>();

            Debug.WriteLine("Initialising driver");
            hat = new PwmController();
            hat.SetDesiredFrequency(90);

            var motor = new Motor(hat, 0);
            motorsBySide.Add("frontLeft", motor);

            motor = new Motor(hat, 4);
            motorsBySide.Add("frontRight", motor);

            motor = new Motor(hat, 8);
            motorsBySide.Add("rearRight", motor);

            motor = new Motor(hat, 12);
            motorsBySide.Add("rearLeft", motor);
            Debug.WriteLine("Initialized motors");

            ArmMotors();
        }

        public void ArmMotors()
        {
            Debug.WriteLine("Arming motors");
            List<Task> tasks = new List<Task>();
            foreach (KeyValuePair<string, Motor> kv in motorsBySide)
            {
                Motor motor = kv.Value;
                Task armTask = new Task(motor.Arm);
                armTask.Start();
                tasks.Add(armTask);
            }
            foreach(Task armTask in tasks)
            {
                while (! armTask.IsCompleted)
                {

                }
            }
            Debug.WriteLine("Armed motors");
        }

        public void ControlMotors()
        {
            Debug.WriteLine("Yaw: {0}, Pitch: {1}, Roll: {2}", this.yawPower, this.pitchPower, this.rollPower);
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
                motor.Run(this.throttle - this.pitchPower + this.rollPower - this.yawPower);
            }

            motor = null;
            motorsBySide.TryGetValue("rearRight", out motor);
            if (motor != null)
            {
                motor.Run(this.throttle - this.pitchPower - this.rollPower + this.yawPower);
            }

        }

        public void Pitch(double power)
        {
            this.pitchPower = LimitThrust(power, 0.3);
            ControlMotors();
        }

        public void Roll(double power)
        {
            this.rollPower = LimitThrust(power, 0.3);
            ControlMotors();
        }

        public void Yaw(double power)
        {
            this.yawPower = LimitThrust(power, 0);// 0.1);
            ControlMotors();
        }

        public double LimitThrust(double thrust, double limit)
        {
            if (thrust > limit)
            {
                return limit;
            } else if (thrust < - limit)
            {
                return - limit;
            }
            return thrust;
        }

        public void Throttle(double power)
        {
            this.throttle = power;
            ControlMotors();
        }

        public void Shutdown()
        {
            foreach(KeyValuePair<string, Motor> kvPair in this.motorsBySide)
            {
                kvPair.Value.Shutdown();
            }
        }

        public void Start()
        {
            foreach (KeyValuePair<string, Motor> kvPair in this.motorsBySide)
            {
                kvPair.Value.Start();
            }
        }

    }
}
