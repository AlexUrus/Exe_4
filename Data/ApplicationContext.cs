using Exercise_4.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;

namespace Exercise_4.Data
{
    internal class ApplicationContext : DbContext
    {
        public DbSet<Vehicle> Vehicles { get; set; }
        public ApplicationContext(): 
            base(ConfigurationManager.
                ConnectionStrings["DBConnectString"].
                ConnectionString) {
            SeedData();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Vehicle>().ToTable(nameof(Vehicles));
            modelBuilder.Entity<Vehicle>().HasKey(x => x.Id);
            modelBuilder.Entity<Vehicle>().Property(h => h.Title).IsRequired();
            modelBuilder.Entity<Vehicle>().Property(h => h.Latitude).IsRequired();
            modelBuilder.Entity<Vehicle>().Property(h => h.Longitude).IsRequired();
        }

        public void SeedData()
        {
            if(Vehicles.Count() == 0)
            {
                var vehicles = new List<Vehicle>
                {
                new Vehicle { Title = "Drilling rig" , Latitude = 57.98175, Longitude = 83.94652 },
                new Vehicle { Title = "Bulldozer" , Latitude = 52.98146, Longitude = 81.94695 },
                new Vehicle { Title = "Tractor" , Latitude = 54.99175, Longitude = 85.98652 },
                new Vehicle { Title = "Kamaz" , Latitude = 56.08146, Longitude = 82.94995 },
                };

                Vehicles.AddRange(vehicles);
                SaveChanges();
            }
            
        }


    }
}
