using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.I2c;
using Windows.Devices.Enumeration;
using Windows.Devices.Pwm;
using Windows.Devices.Pwm.Provider;

namespace NanoDrone.Sensors.Orientation
{
    class OrientationSensor
    {
        private I2cDevice sensor;
        private byte address;
        private bool initialized;

        public OrientationSensor(byte address = 0x28)
        {
            this.address = address;
            this.initialized = false;

            Initialize().Wait();

            Debug.WriteLine(sensor);
            byte[] buffer = new byte[1];
            sensor.Read(buffer);
            Debug.WriteLine(buffer);
        }

        public async Task Initialize()
        {
            Debug.WriteLine("START");
            string selector = I2cDevice.GetDeviceSelector();
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(selector);

            if (devices.Count < 1)
            {
                Debug.WriteLine("Unable to locate I2C controller");
                throw new Exception("Unable to locate I2C controller");
            }
            DeviceInformation controllerInfo = devices[0];
            I2cConnectionSettings connectionSettings = new I2cConnectionSettings(address);
            Debug.WriteLine("FINDING ASYNC");
            sensor = await I2cDevice.FromIdAsync(controllerInfo.Id, connectionSettings);
            Debug.WriteLine("FOUND DEVICE");

            if (sensor == null)
            {
                Debug.WriteLine("Unable to locate orientation sensor");
                throw new Exception("Unable to locate orientation sensor");
            }
            
            this.initialized = true;
            Debug.WriteLine("STOP");
        }

        public void GetOrientation()
        {
           
        }
        
    }
}
