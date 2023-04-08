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
    public class GPSReceiver
    {
        private readonly SerialPort _serialPort;
        private readonly GMapMarker _marker;
        private readonly GMapControl _map;

        public GPSReceiver(SerialPort serialPort, GMapMarker marker, GMapControl map)
        {
            _serialPort = serialPort;
            _marker = marker;
            _map = map;

            // подписываемся на событие приема данных
            _serialPort.DataReceived += SerialPortOnDataReceived;
        }

        private void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // считываем данные из порта
            var data = _serialPort.ReadLine();

            // парсим координаты из сообщения GPGGA
            var coord = ParseGPGGA(data);

            // если координаты были успешно распарсены
            if (coord != null)
            {
                // обновляем координаты маркера на карте
                _map.Invoke(new Action(() =>
                {
                    _marker.Position = new PointLatLng(coord.Latitude, coord.Longitude);
                    _map.Refresh();
                }));
            }
        }

        private GPSInfo ParseGPGGA(string nmea) // извлечение координат из NMEA сообщения
        {
            // разделяем строку на части
            var parts = nmea.Split(',');

            // проверяем, что это сообщение GPGGA
            if (parts[0] != "$GPGGA" || parts.Length < 10)
                return null;

            // извлекаем координаты
            double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double latitude);
            double.TryParse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture, out double longitude);

            return new GPSInfo(latitude, longitude);
        }

    }

}
