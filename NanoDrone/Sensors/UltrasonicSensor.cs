using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using System.Diagnostics;

namespace NanoDrone.Sensors
{
    class UltrasonicSensor
    {
        private GpioController controller;
        private GpioPin echoPin;
        private GpioPin triggerPin;
        private Stopwatch stopwatch;
        private bool measuring;

        private double lastDistance;
        public double LastDistance
        {
            get
            {
                return lastDistance;
            }
        }

        public UltrasonicSensor(int triggerPin, int echoPin)
        {
            this.controller = GpioController.GetDefault();

            this.triggerPin = controller.OpenPin(triggerPin);
            this.triggerPin.SetDriveMode(GpioPinDriveMode.Output);

            this.echoPin = controller.OpenPin(echoPin);
            this.echoPin.SetDriveMode(GpioPinDriveMode.Input);
        }

        public double Trigger()
        {
            if (measuring)
            {
                return -1;
            }
            return TriggerAndReceive().Result;
        }

        public void TriggerAsync()
        {
            if (measuring)
            {
                return;
            }
            Task.Run(TriggerAndReceive);
        }

        public void Trigger(Action<double> callback)
        {
            if (measuring)
            {
                return;
            }
            Task.Run(() => TriggerAndCallback(callback) );
        }

        public async Task TriggerAndCallback(Action<double> callback)
        {
            var result = TriggerAndReceive().Result;
            callback(result);
        }

        public async Task<double> TriggerAndReceive()
        {
            ManualResetEvent resetEvent = new ManualResetEvent(false);
            
            this.triggerPin.Write(GpioPinValue.High);
            resetEvent.WaitOne(TimeSpan.FromMilliseconds(0.01));
            this.triggerPin.Write(GpioPinValue.Low);

            Stopwatch stopwatch = new Stopwatch();

            while (this.echoPin.Read() == GpioPinValue.Low)
            {
            }
            stopwatch.Start();


            while (this.echoPin.Read() == GpioPinValue.High)
            {
            }
            stopwatch.Stop();
            
            TimeSpan timeElapsed = stopwatch.Elapsed;
            double distance = timeElapsed.TotalSeconds * 17000;

            Debug.WriteLine("Distance: {0}cm", distance);
            lastDistance = distance;
            return distance;
        }

    }
}
