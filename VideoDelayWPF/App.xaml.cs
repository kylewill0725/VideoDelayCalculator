using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using dotenv.net.DependencyInjection.Microsoft;
using dotenv.net.Interfaces;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using VideoDelayWPF.MainApp.Services;
using VideoDelayWPF.WebServer;

namespace VideoDelayWPF
{
    /// <summary>
    /// Interaction logic for AppAoppMainprivate void Test() {}TTeApp.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;
        private ServiceCollection _services;

        public App()
        {
            ConfigureServices();

            var envSettings = _serviceProvider!.GetService<IEnvReader>()!;
            _serviceProvider.GetService<MainWindow>()!.Show();
            _serviceProvider.GetService<WebProviderService>()!.RestartServer(envSettings);
        }

        private void ConfigureServices()
        {
            _services = new ServiceCollection();
            _services.AddEnv(builder =>
            {
                builder
                    .AddEnvFile("config.env")
                    .AddThrowOnError(true)
                    .AddEncoding(Encoding.UTF8);
            });
            _services.AddEnvReader();
            _services.AddSingleton<WebProviderService>();
            _services.AddSingleton<MainWindow>();
            
            _serviceProvider = _services.BuildServiceProvider();
            
        }
    }
}