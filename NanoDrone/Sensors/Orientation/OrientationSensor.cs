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
using System.IO;
using System.Threading;
using NanoDrone.Utils;

namespace NanoDrone.Sensors.Orientation
{
    public class OrientationSensor
    {
        private I2cDevice sensor;
        private byte address;
        private bool initialized;

        public OrientationSensor(byte address = 0x28)
        {
            this.address = address;
            this.initialized = false;

            Initialize();
            Startup();
        }

        public void Initialize()
        {

            var settings = new I2cConnectionSettings(address) { BusSpeed = I2cBusSpeed.FastMode };
            DeviceInformationCollection devices = null;

            Task.Run(async () =>
            {

                // Get a selector string that will return all I2C controllers on the system 
                string aqs = I2cDevice.GetDeviceSelector();

                // Find the I2C bus controller device with our selector string
                devices = await DeviceInformation.FindAllAsync(aqs);

                //search for the controller
                if (!devices.Any())
                    throw new IOException("No I2C controllers were found on the system");

                //see if we can find the hat
                sensor = await I2cDevice.FromIdAsync(devices[0].Id, settings);

            }).Wait();

            if (sensor == null)
            {
                string message;
                if (devices != null && devices.Count > 0)
                {
                    message = string.Format(
                        "Slave address {0} on I2C Controller {1} is currently in use by another application. Please ensure that no other applications are using I2C.",
                        settings.SlaveAddress,
                        devices[0].Id);
                }
                else
                {
                    message = "Could not initialize the device!";
                }

                throw new IOException(message);
            }
            Debug.WriteLine("Sensor connected");
        }

        public void Startup()
        {
            SetMode(OperationModes.OPERATION_MODE_CONFIG);
            Debug.WriteLine("Config Mode");
            Task.Delay(1000).Wait();
            WriteByte(Registers.BNO055_SYS_TRIGGER_ADDR, 0x20);
            Debug.WriteLine("Resetting");
            Task.Delay(1000).Wait();
            WriteByte(Registers.BNO055_PWR_MODE_ADDR, (byte)PowerModes.POWER_MODE_NORMAL);
            Debug.WriteLine("Set normal power mode");
            Task.Delay(1000).Wait();
            WriteByte(Registers.BNO055_PAGE_ID_ADDR, 0);
            Debug.WriteLine("Set page address");
            Task.Delay(1000).Wait();
            WriteByte(Registers.BNO055_SYS_TRIGGER_ADDR, 0x0);
            Debug.WriteLine("Resetting");
            Task.Delay(1000).Wait();
            SetMode(OperationModes.OPERATION_MODE_NDOF);
            Debug.WriteLine("OPERATION_MODE_NDOF");
            Task.Delay(1000).Wait();
        }

        public Utils.Orientation GetOrientation()
        {
            try
            {
                byte yawMSB = ReadByte((byte)Registers.BNO055_EULER_H_MSB_ADDR);
                byte yawLSB = ReadByte((byte)Registers.BNO055_EULER_H_LSB_ADDR);
                byte pitchMSB = ReadByte((byte)Registers.BNO055_EULER_P_MSB_ADDR);
                byte pitchLSB = ReadByte((byte)Registers.BNO055_EULER_P_LSB_ADDR);
                byte rollMSB = ReadByte((byte)Registers.BNO055_EULER_R_MSB_ADDR);
                byte rollLSB = ReadByte((byte)Registers.BNO055_EULER_R_LSB_ADDR);

                var yawInt = BitConverter.ToInt16(new byte[] { yawLSB, yawMSB }, 0);
                var pitchInt = BitConverter.ToInt16(new byte[] { pitchLSB, pitchMSB }, 0);
                var rollInt = BitConverter.ToInt16(new byte[] { rollLSB, rollMSB }, 0);

                Utils.Orientation orientation = new Utils.Orientation((float) yawInt / 16, (float)pitchInt / 16, (float)rollInt / 16);

                Debug.WriteLine("{0},{1}:{2} {3},{4}:{5} {6},{7}:{8}", yawMSB, yawLSB, yawInt / 16, pitchMSB, pitchLSB, pitchInt / 16, rollMSB, rollLSB, rollInt / 16);
                //Debug.WriteLine("Yaw: {0}, Pitch: {1}, Roll: {2}", orientation.yaw, orientation.pitch, orientation.roll);

                return orientation;
            } catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }

        private byte ReadByte(byte address)
        {
            byte[] buffer = new byte[1];
            sensor.WriteRead(new byte[] { address }, buffer);
            return buffer[0];
        }

        private byte[] ReadBytes()
        {
            byte[] buffer = new byte[256];
            string stringbuffer = "";
            sensor.Read(buffer);
            for (var i = 0; i < buffer.Length; i++)
            {
                var value = buffer[i];
                string valueString = buffer[i].ToString();
                while (valueString.Length < 3)
                {
                    valueString = "0" + valueString;
                }
                stringbuffer += valueString + "|";
                if ((i + 1) % 16 == 0)
                {
                stringbuffer += "\n";
                }
            }

            Debug.WriteLine(DateTime.Now);
            Debug.WriteLine(stringbuffer);
            return buffer;
        }

        private void WriteByte(Registers address, byte data)
        {
            //Debug.WriteLine("Pre writing {0} to {1}", data, address);
            //ReadBytes();
            sensor.Write(new byte[] { (byte)address, data });
            //sensor.Write(new byte[] { (byte)address });
            //sensor.Write(new byte[] { data });
            //Debug.WriteLine("Post writing {0} to {1}", data, address);
            //ReadBytes();
        }

        private void SetMode(OperationModes mode)
        {
            WriteByte(Registers.BNO055_OPR_MODE_ADDR, (byte)mode);
        }
        
    }

}
