using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NanoDrone.Constants;
using Adafruit.Pwm;


namespace NanoDrone.Devices
{
    public class Motor
    {
        public MotorDirection direction;
        private PwmController hat;
        private byte number;
        private int servoMax = (int)(4000 * 0.4);
        private int servoMin = (int)(4000 * 0.4 * 0.35);
        private double frequency;

        private double speed;
        public double Speed
        {
            get
            {
                return this.speed;
            }
            set
            {
                this.Run(value);
            }
        }

        private bool running;
        public bool Running { get; }

        public Motor(PwmController hat, byte motorNumber, MotorDirection direction)
        {
            this.hat = hat;
            this.speed = 0;
            this.direction = direction;
            this.running = false;
            this.number = motorNumber;
            this.frequency = 180;
        }

        public void Run(double speed)
        {
            if (speed < 0)
            {
                hat.SetPulseParameters(this.number, (servoMin));
                return;
            }
            this.running = true;
            Debug.WriteLine("Running {0} at {1}", number, servoMin + speed * (servoMax - servoMin));
            hat.SetPulseParameters(this.number, servoMin + speed * ( servoMax - servoMin ) );
        }

        public void Stop()
        {
            hat.SetPulseParameters(this.number, 0);
            running = false;
        }

        public void Calibrate()
        {
            this.hat.SetDesiredFrequency(frequency);
            Debug.WriteLine("Calibrating {0} at {1}", number, frequency);

            this.Stop();
            Debug.WriteLine("Please disconnect the power");
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(4000));
            this.Run(1);
            Debug.WriteLine("Please reconnect the power");
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(4000));
            this.Run(0);
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(4000));
            this.Run(0.05);
            Debug.WriteLine("Arming finished");
            new ManualResetEvent(false).WaitOne(TimeSpan.FromMilliseconds(5000));
            this.Run(0);
        }

        public void Arm()
        {
            this.hat.SetDesiredFrequency(frequency);
            Debug.WriteLine("Arming {0} at {1}", number, frequency);
            this.Calibrate();
        }

    }
}
