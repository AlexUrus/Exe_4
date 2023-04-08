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

namespace Exercise_4.Controllers
{
    public class MarkerController
    {
        private List<GMapMarker> markers;
        private VehicleController vehicleController;

        public MarkerController() 
        {
            //дописать чтение из базы
            vehicleController = new VehicleController();
            ReadPositionVehicles(); //сомнительно
        }
        public GMapMarker CreateMarker(PointLatLng pointClick, MouseEventArgs e)
        {
            GMapMarker marker = new GMarkerGoogle(pointClick, GMarkerGoogleType.blue_pushpin);

            markers.Add(marker);

            vehicleController.AddVehicleToDb(marker);
            
            return marker;
        }
        public void RemoveMarker() 
        {

        }
        public void UpdateMarker(GMapMarker marker)
        {
            var index = markers.FindIndex(m => m.ToolTipText == marker.ToolTipText);
            if (index >= 0)
            {
                markers[index] = marker;
                vehicleController.UpdateVehicleInDb(marker);
            }   
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
