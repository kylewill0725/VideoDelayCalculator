using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VideoDelayWPF.MainApp.Services;

namespace VideoDelayWPF.WebServer
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.Use(async (context, next) =>
            {
                using var websocket = await context.WebSockets.AcceptWebSocketAsync();
                var taskCompletionSource = new TaskCompletionSource();
                context.RequestServices.GetService<CameraLatencyService>()?.ProcessWebsocket(websocket, taskCompletionSource);
                await taskCompletionSource.Task;
            });
        }
    }
}