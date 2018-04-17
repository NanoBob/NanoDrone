using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NanoDrone.Devices;
using NanoDrone.Sensors;
using NanoDrone.Sensors.Orientation;

namespace NanoDrone.Controllers
{
    public class OrientationController
    {
        private NanoDrone drone;
        private OrientationSensor sensor;
        private double targetYaw;
        private double targetPitch;
        private double targetRoll;
        private double yaw;
        private double pitch;
        private double roll;
        private Task sensorTask;
        private CancellationTokenSource cancellationToken;
        private bool running;

        public double Yaw
        {
            get
            {
                return yaw;
            }
        }
        public double Pitch
        {
            get
            {
                return pitch;
            }
        }
        public double Roll
        {
            get
            {
                return roll;
            }
        }
        public MotorController motorController
        {
            get
            {
                return drone.MotorController;
            }
        }

        public bool Running
        {
            get
            {
                return this.running;
            }
        }

        public OrientationController(NanoDrone drone)
        {
            this.drone = drone;
            this.running = false;

            sensor = new OrientationSensor();

            this.SetOwnOrientation(0, 0, 0);
            this.SetTargetOrientation(0, 0, 0);
            Start();
        }

        public void Start()
        {
            Enable();
            sensorTask = Task.Run(() =>
            {
                while (motorController == null)
                {
                }
                while (true)
                {
                    while (!running)
                    {
                        Task.Delay(150).Wait();
                    }
                    Utils.Orientation orientation = sensor.GetOrientation();
                    if (orientation != null)
                    {
                        SetOwnOrientation(orientation.yaw, orientation.pitch, orientation.roll);
                    }
                }
            });
        }

        public bool Enable()
        {
            if (! running)
            {
                running = true;
                return true;
            }
            return false;
        }

        public bool Disable()
        {
            if (running)
            {
                running = false;
                return true;
            }
            return false;
        }

        public void Test()
        {
            SetOwnOrientation(0, 0, 0);
            this.motorController.Throttle(0);

            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(5000));
            SetTargetOrientation(0, 10, 0);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(5000));
            SetTargetOrientation(0, -10, 0);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(5000));
            SetTargetOrientation(0, 0, -10);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(5000));
            SetTargetOrientation(0, 0, 10);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(5000));
            SetTargetOrientation(10, 0, 0);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(5000));
            SetTargetOrientation(-10, 0, 0);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(5000));
            SetTargetOrientation(0, 0, 0);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(5000));
            this.motorController.Shutdown();
        }

        public void SetTargetOrientation(double targetYaw, double targetPitch, double targetRoll)
        {
            this.targetYaw = targetYaw;
            this.targetPitch = targetPitch;
            this.targetRoll = targetRoll;
            AdjustOrientation();
        }

        private void SetOwnOrientation(double yaw, double pitch, double roll)
        {
            this.yaw = yaw;
            this.pitch = pitch;
            this.roll = roll;
            AdjustOrientation();
        }

        public void ResetOrientationControls()
        {
            if (this.motorController != null)
            {
                this.motorController.Yaw(0);
                this.motorController.Pitch(0);
                this.motorController.Roll(0);
            }
        }

        public void AdjustOrientation()
        {
            if (this.motorController != null)
            {
                double yawDelta = CalculateDelta(targetYaw, yaw);
                double pitchDelta = CalculateDelta(targetPitch, pitch);
                double rollDelta = CalculateDelta(targetRoll, roll);

                this.motorController.Yaw(ConvertDeltaToThrustOffset(yawDelta));
                this.motorController.Pitch(ConvertDeltaToThrustOffset(pitchDelta));
                this.motorController.Roll(ConvertDeltaToThrustOffset(rollDelta));

                Debug.WriteLine("Yaw:       {0} : {1}   {2}", yaw, yawDelta, ConvertDeltaToThrustOffset(yawDelta));
                Debug.WriteLine("Pitch:     {0} : {1}   {2}", pitch, pitchDelta, ConvertDeltaToThrustOffset(pitchDelta));
                Debug.WriteLine("Roll:      {0} : {1}   {2}", roll, rollDelta, ConvertDeltaToThrustOffset(rollDelta));
            } else
            {
                Debug.WriteLine("Waiting for motor controller to boot");
            }
        }

        private double ConvertDeltaToThrustOffset(double offset)
        {
            return (double)1 / (double)180 * offset;
        }            

        private double CalculateDelta(double target, double source)
        {
            double delta = target - source;
            if (delta > 180)
            {
                delta -= 360;
            }
            else if (delta < -180)
            {
                delta += 360;
            }
            return delta;
        }

        public void ThrottleTest()
        {
            for (double i = 0; i < 0.48; i += 0.005)
            {
                motorController.Throttle(i);
                Debug.WriteLine("Throttle at {0}", i);
                new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(500));
            }
            motorController.Throttle(0);
        }
    }
}
