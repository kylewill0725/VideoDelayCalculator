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

        public App()
        {
            var appServices = new ServiceCollection();
            ConfigureServices(appServices);
            _serviceProvider = appServices.BuildServiceProvider();

            var envSettings = _serviceProvider.GetService<IEnvReader>();
            _serviceProvider.GetService<MainWindow>()!.Show();
            _serviceProvider.GetService<WebProviderService>()!.RestartServer(envSettings);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddEnv(builder =>
            {
                builder
                    .AddEnvFile(".env")
                    .AddThrowOnError(false)
                    .AddEncoding(Encoding.UTF8);
            });
            services.AddSingleton<WebProviderService>();
            services.AddSingleton<MainWindow>();
        }
    }
}