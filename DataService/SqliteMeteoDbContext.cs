using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Final.DataService
{
    public class SqliteMeteoDbContext : MeteoDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string chemin =
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                + Path.DirectorySeparatorChar
                + ".meteo-final";

            Directory.CreateDirectory(chemin);

            string chaineConnexion =
                $"Data Source={chemin}{Path.DirectorySeparatorChar}" +
                $"{Properties.Settings.Default.nomBdSqlite}";

            options.UseSqlite(chaineConnexion);
        }
    }
}
