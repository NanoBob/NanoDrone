using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.I2c;
using Windows.Devices.Enumeration;
using System.IO;

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
                string aqs = I2cDevice.GetDeviceSelector();                
                devices = await DeviceInformation.FindAllAsync(aqs);                
                if (!devices.Any())
                {
                    throw new IOException("No I2C controllers were found on the system");
                }                
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

            this.initialized = true;
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

                // the below values depend on the actual orientation of your sensor
                Utils.Orientation orientation = new Utils.Orientation(- (float) yawInt / 16 - 90, - (float)rollInt / 16, - (float)pitchInt / 16);

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
            sensor.Write(new byte[] { (byte)address, data });
        }

        private void SetMode(OperationModes mode)
        {
            WriteByte(Registers.BNO055_OPR_MODE_ADDR, (byte)mode);
        }
        
    }

}
