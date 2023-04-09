using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Exercise_4.Data;
using System.Security.Cryptography;
using Exercise_4.Models;
using System.Drawing;

namespace Exercise_4.Controllers
{
    public class MarkerController
    {
        private List<GMapMarker> markers;
        private VehicleController vehicleController; 

        public MarkerController() 
        {
            vehicleController = new VehicleController();
            ReadPositionVehicles(); //сомнительно
        }
        public GMapMarker CreateMarker(PointLatLng pointClick)
        {
            GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.blue_pushpin);
            marker.ToolTipText = pointClick.ToString();
            markers.Add(marker);

            vehicleController.AddVehicleToDb(marker);
            
            return marker;
        }
        public void RemoveMarker() 
        {

        }
        public void UpdateMarkerList(GMapMarker modifiedMarker)
        {
            var index = markers.FindIndex(m => m.ToolTipText == modifiedMarker.ToolTipText);
            if (index >= 0)
            {
                markers[index] = modifiedMarker;
                vehicleController.UpdateVehicleInDb(modifiedMarker);
            }   
        }

        public GMapMarker ChangeLatLngMarker(string nmea, GMapMarker marker)
        {
            PointLatLng pointLatLng = GPSReceiver.ParseGPGGA(nmea);
            marker.Position = pointLatLng;
            return marker;
        }
        public List<GMapMarker> GetListMarkers()
        {
            return new List<GMapMarker>(markers);
        }

        private void ReadPositionVehicles() // сомнительно
        {
            markers = new List<GMapMarker>();

            List<Vehicle> vehicles = vehicleController.GetListVehicle();

            foreach (Vehicle vehicle in vehicles)
            {
                markers.Add(new GMarkerGoogle(new PointLatLng(vehicle.Latitude, vehicle.Longitude) ,
                    GMarkerGoogleType.blue_pushpin)
                { ToolTipText = vehicle.Title});
            }
        }
    }
}
