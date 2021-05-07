using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Syncfusion.Licensing;
using Syncfusion.SfSkinManager;
using VaccineTracker.Domain;

namespace VaccineTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AddEnvironmentVariables();
            InitializeSyncfusion();
            Current.DispatcherUnhandledException += (sender, args) =>
            {
                MessageBox.Show(args.Exception.Message);
            };
            base.OnStartup(e);
        }

        private static void InitializeSyncfusion()
        {
            var license = Environment.GetEnvironmentVariable("SyncfusionLicense");
            if (string.IsNullOrEmpty(license)) return;
            SyncfusionLicenseProvider.RegisterLicense(license);
        }

        private static void AddEnvironmentVariables()
        {
            var configs = new ConfigurationBuilder()
                .AddUserSecrets<App>().Build().AsEnumerable();
            foreach (var kvp in configs)
            {
                Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
            }
        }

        public static IConfigurationProvider SecretsProvider { get; private set; }
    }
}
