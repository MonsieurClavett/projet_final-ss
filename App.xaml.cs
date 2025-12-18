using Autofac;
using Autofac.Configuration;
using Final.DataService;
using Final.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Final
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var db = new SqliteMeteoDbContext())
            {
                db.Database.Migrate(); 
            }

            var config = new ConfigurationBuilder();
            config.AddJsonFile("di.json");

            var module = new ConfigurationModule(config.Build());
            var builder = new ContainerBuilder();
            builder.RegisterModule(module);

            FournisseurDI.Container = builder.Build();

            LanguageService.ApplyCulture(global::Final.Properties.Settings.Default["langue"]?.ToString());



        }


    }
}
