using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDrone.Utils
{
    public class Orientation
    {
        public float yaw;
        public float pitch;
        public float roll;

        public Orientation(float yaw, float pitch, float roll)
        {
            this.yaw = yaw;
            this.pitch = pitch;
            this.roll = roll;
        }
    }
}
