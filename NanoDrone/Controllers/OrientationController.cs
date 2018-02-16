using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NanoDrone.Constants;
using NanoDrone.Devices;
using NanoDrone.Sensors;

namespace NanoDrone.Controllers
{
    class OrientationController
    {
        private Drone drone;
        private MotorController motorController;
        private double targetYaw;
        private double targetPitch;
        private double targetRoll;
        private double yaw;
        private double pitch;
        private double roll;

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

        public OrientationController(NanoDrone drone)
        {
            this.drone = drone;
            this.motorController = drone.MotorController;
            this.SetTargetOrientation(0, 0, 0);
            this.SetOwnOrientation(0, 0, 0);

            Test();
            this.motorController.ShutDown();
        }

        public void Test()
        {
            SetOwnOrientation(0, 0, 0);
            this.motorController.Throttle(0.1);

            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(5000));
            SetTargetOrientation(0, 0.1, 0);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(5000));
            SetTargetOrientation(0, -0.1, 0);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(5000));
            SetTargetOrientation(0, 0, -0.1);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(5000));
            SetTargetOrientation(0, 0, 0.1);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(5000));
            SetTargetOrientation(0.1, 0, 0);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(5000));
            SetTargetOrientation(-0.1, 0, 0);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(5000));
            SetTargetOrientation(0, 0, 0);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(5000));
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

        public void AdjustOrientationControls()
        {
            this.motorController.Yaw(0);
            this.motorController.Pitch(0);
            this.motorController.Roll(0);
        }

        public void AdjustOrientation()
        {
            Debug.WriteLine("Adjusting to Yaw: {0}, Pitch: {1}, Roll: {2}", targetYaw - yaw, targetPitch - pitch, targetRoll - roll);
            this.motorController.Yaw((targetYaw - yaw) * 0.35);
            this.motorController.Pitch((targetPitch - pitch) * 0.35);
            this.motorController.Roll((targetRoll - roll) * 0.35);
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
