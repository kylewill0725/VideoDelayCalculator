using System;
using System.Linq;
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

        public void RestartServer(IEnvReader settings)
        {
            _server = WebHost
                .CreateDefaultBuilder()
                .UseKestrel(x =>
                {
                    // Todo: Log missing env variables
                    if (!settings.TryGetIntValue("WebServerPort", out var port))
                    {
                        port = 0;
                    }
                    
                    if (!settings.TryGetBooleanValue("AllowExternalIps", out var externalIps))
                    {
                        externalIps = true;
                    }

                    if (externalIps)
                    {
                        x.ListenAnyIP(port);
                    }
                    
                    x.ListenLocalhost(port);
                })
                .UseStartup<Startup>()
                .Build();
            
            Task.Run(() =>
            {
                _server.Start();
                var serverAddresses = _server.ServerFeatures.Get<IServerAddressesFeature>();
                
                Events.WebServerStarted($"{serverAddresses.Addresses.First()}");
                _server.WaitForShutdown();
            });
        }

        public void StopServer()
        {
            _server?.StopAsync().Wait();
        }
    }
}