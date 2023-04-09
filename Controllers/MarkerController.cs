using Exercise_4.Models;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Exercise_4.Controllers
{
    public class MarkerController
    {
        private List<GMapMarker> markers;
        private readonly VehicleController vehicleController;
        public GMapMarker CurrentMarker;
        public bool MarkerIsDragging;
        public MarkerController() 
        {
            vehicleController = new VehicleController();
            ReadPositionVehicles();
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

        public void ChangeLatLngMarker(string nmea)
        {
            PointLatLng pointLatLng = GPSReceiver.ParseGPGGA(nmea);
            CurrentMarker.Position = pointLatLng;
        }
        public List<GMapMarker> GetListMarkers()
        {
            return new List<GMapMarker>(markers);
        }
        public void ShowDialogMarker()
        {
            MessageBox.Show(
                "Информация о маркере" +
                $"\r\n Ш: {CurrentMarker.Position.Lat}" +
                $"\r\n Д: {CurrentMarker.Position.Lng}" +
                $"\r\n    {CurrentMarker.ToolTipText}");
            CurrentMarker = null;
        }

        public GMapMarker ChangeColorMarker()
        {
            Random random = new Random();
            int numberColor = random.Next( 0, 3 );
            GMapMarker marker;
            switch (numberColor)
            {
                case 0:
                    {
                        marker = new GMarkerGoogle(CurrentMarker.Position, GMarkerGoogleType.red_pushpin)
                        {
                            ToolTipText = CurrentMarker.ToolTipText
                        };
                        CurrentMarker = marker;
                        break;
                    }
                case 1: 
                    {
                        marker = new GMarkerGoogle(CurrentMarker.Position, GMarkerGoogleType.pink_pushpin)
                        {
                            ToolTipText = CurrentMarker.ToolTipText
                        };
                        CurrentMarker = marker;
                        break;
                    }
                case 2:
                    {
                        marker = new GMarkerGoogle(CurrentMarker.Position, GMarkerGoogleType.yellow_pushpin) 
                        { 
                            ToolTipText = CurrentMarker.ToolTipText 
                        };
                        CurrentMarker = marker;
                        break;
                    }
                case 3:
                    {
                        marker = new GMarkerGoogle(CurrentMarker.Position, GMarkerGoogleType.purple_pushpin) 
                        { 
                            ToolTipText = CurrentMarker.ToolTipText 
                        };
                        CurrentMarker = marker;
                        break;
                    }
            }
            //UpdateMarkerList(CurrentMarker);
            return CurrentMarker;
        }

        public GMapMarker CreateRandomMarker()
        {
            Random rnd = new Random();
            Random rnd2 = new Random();
            PointLatLng pointLatLng = new PointLatLng(rnd.NextDouble() * 90.0, rnd2.NextDouble() * 90.0);
            GMapMarker marker = new GMarkerGoogle(pointLatLng, GMarkerGoogleType.blue_pushpin)
            {
                ToolTipText = pointLatLng.ToString()
            };

            markers.Add(marker);

            vehicleController.AddVehicleToDb(marker);
            return marker;
        }
        private void ReadPositionVehicles() 
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
