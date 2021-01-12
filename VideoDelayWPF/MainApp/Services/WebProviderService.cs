using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using dotenv.net.Interfaces;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using VideoDelayWPF.MainApp.Shared;
using VideoDelayWPF.WebServer;

namespace VideoDelayWPF.MainApp.Services
{
    public class WebProviderService
    {
        private IWebHost? _server;

        public WebProviderService()
        {
            
        }

        public void RestartServer(IEnvReader settings)
        {
            _server = WebHost
                .CreateDefaultBuilder()
                .UseKestrel(x =>
                {
                    // Todo: Log missing env variables
                    if (!settings.TryGetIntValue("WebServerPort", out var port))
                    {
                        port = 61680;
                    }
                    
                    if (!settings.TryGetBooleanValue("AllowExternalIps", out var externalIps))
                    {
                        externalIps = true;
                    }

                    if (externalIps)
                    {
                        x.Listen(IPAddress.Any, port);
                    }
                    
                    x.Listen(IPAddress.Loopback, port);
                })
                .UseWebRoot("WebServer/www")
                .UseStartup<Startup>()
                .Build();
            
            Task.Run(() =>
            {
                _server.Start();
                var serverAddress = 
                    _server.ServerFeatures.Get<IServerAddressesFeature>().Addresses
                    .First(address => !address.Contains("127.0.0.1"));

                var port = new Uri(serverAddress).Port;
                
                Events.WebServerStarted(port);
                _server.WaitForShutdown();
            });
        }

        public void StopServer()
        {
            _server?.StopAsync().Wait();
        }
    }
}