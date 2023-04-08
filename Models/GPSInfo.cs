using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exercise_4
{
    public class GPSInfo
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public GPSInfo(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }

}
