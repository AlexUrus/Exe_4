using Exercise_4.Models;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exercise_4.Controllers
{
    public class VehicleController
    {
        public async void AddVehicleToDb(GMapMarker marker)
        {
            await Task.Run(() =>
            {
                using (var db = new Data.ApplicationContext())
                {
                    var vehicle = new Vehicle
                    {
                        Latitude = marker.Position.Lat,
                        Longitude = marker.Position.Lng,
                        Title = marker.ToolTipText
                    };

                    db.Vehicles.Add(vehicle);
                    db.SaveChangesAsync();
                }
            });
        }

        public async void UpdateVehicleInDb(GMapMarker marker)
        {
            await Task.Run(() =>
            {
                using (var db = new Data.ApplicationContext())
                {
                    var vehicle = db.Vehicles.FirstOrDefault(h => h.Title == marker.ToolTipText);
                    if(vehicle != null)
                    {
                        vehicle.Longitude = marker.Position.Lng;
                        vehicle.Latitude = marker.Position.Lat;
                        db.SaveChangesAsync();
                    }
                }
            });
        }

        public List<Vehicle> GetListVehicle()
        {
            using (var db = new Data.ApplicationContext())
            {
                var vehicleList = db.Vehicles.ToList();
                return vehicleList;
            }
        }



    }
}
