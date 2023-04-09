using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exercise_4
{
    public static class GPSReceiver
    {
        public static PointLatLng ParseGPGGA(string nmea) 
        {

            string[] parts = nmea.Split(',');

            if (parts[0] != "$GPGGA" || parts.Length < 10)
                throw new Exception("Сообщение NMEA не корректно");

            string latitudeDM = parts[2];
            string longitudeDM = parts[4];

            double.TryParse(latitudeDM.Substring(0, 2), out double latDegrees);
            double.TryParse(latitudeDM.Substring(2), NumberStyles.Float, CultureInfo.InvariantCulture, out double latMinutes);
            double.TryParse(longitudeDM.Substring(0, 3), out double longDegrees);
            double.TryParse(longitudeDM.Substring(3), NumberStyles.Float, CultureInfo.InvariantCulture, out double longMinutes);

            double latitude = latDegrees + (latMinutes / 60);
            double longitude = longDegrees + (longMinutes / 60);

            return new PointLatLng(latitude,longitude);
        }

    }

}
