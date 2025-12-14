using Final.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Final.DataService
{
    public abstract class MeteoDbContext : DbContext
    {
        public MeteoDbContext() { }

        public DbSet<Region> Regions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seeding: 1 région par défaut
            modelBuilder.Entity<Region>().HasData(new Region
            {
                Id = 1,
                Nom = "Shawinigan",
                Latitude = 46.56984172477484,
                Longitude = -72.73811285651442
            });
        }
    }
}
