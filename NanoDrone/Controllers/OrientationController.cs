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
        }

        public void Test()
        {
            SetOwnOrientation(0, 0, 0);

            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(2000));
            SetTargetOrientation(0, 0.1, 0);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(2000));
            SetTargetOrientation(0, -0.1, 0);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(2000));
            SetTargetOrientation(0, 0, -0.1);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(2000));
            SetTargetOrientation(0, 0, 0.1);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(2000));
            SetTargetOrientation(0.1, 0, 0);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(2000));
            SetTargetOrientation(-0.1, 0, 0);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(2000));
            SetTargetOrientation(0, 0, 0);
        }

        public void SetTargetOrientation(double targetYaw = -100, double targetPitch = -100, double targetRoll = -100)
        {
            this.targetYaw = targetYaw > -100 ? targetYaw : this.targetYaw;
            this.targetPitch = targetPitch > -100 ? targetPitch : this.targetPitch;
            this.targetRoll = targetRoll > -100 ? targetRoll : this.targetRoll;
            AdjustOrientation();
        }

        private void SetOwnOrientation(double yaw, double pitch, double roll)
        {
            this.yaw = yaw;
            this.pitch = pitch;
            this.roll = roll;
            AdjustOrientation();
        }

        public void AdjustOrientation()
        {
            if (targetYaw != yaw)
            {
                this.motorController.Yaw(targetYaw - yaw);
            }
            if (targetPitch != pitch)
            {
                this.motorController.Pitch(targetPitch - pitch);
            }
            if (targetRoll != roll)
            {
                this.motorController.Roll(targetRoll - roll);
            }
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
