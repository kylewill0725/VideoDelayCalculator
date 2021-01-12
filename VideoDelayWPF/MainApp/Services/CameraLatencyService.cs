using System;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace VideoDelayWPF.MainApp.Services
{
    public class CameraLatencyService
    {
        private readonly CameraFeedService _feed;

        public CameraLatencyService(CameraFeedService feed)
        {
            _feed = feed;
        }

        public void ProcessWebsocket(WebSocket websocket, TaskCompletionSource taskCompletionSource)
        {

            Task.Run(() => ComputeLatency(_feed, websocket, taskCompletionSource).Wait());
        }

        private async Task ComputeLatency(
            CameraFeedService cameraFeedService, 
            WebSocket websocket,
            TaskCompletionSource taskCompletionSource)
        {
            var rtt = await GetRoundTripTime(websocket);
            var ping = rtt / 2;
            
            await SendCommandWithConfirm(websocket, Commands.Blink(100));
            if (!await cameraFeedService.FindClientLocation(10))
            {
                await websocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Failed to find the client",
                    CancellationToken.None);
            }

            await SendCommandWithConfirm(websocket, Commands.Blank());

            await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Success", CancellationToken.None);
            taskCompletionSource.SetResult();
        }

        private async Task<long> GetRoundTripTime(WebSocket websocket)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await SendCommandWithConfirm(websocket, Commands.Ping());
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        
        private async Task SendCommandWithConfirm(WebSocket webSocket, byte[] command)
        {
            byte[] responseData;
            do
            {
                await webSocket.SendAsync(command, WebSocketMessageType.Binary, true, CancellationToken.None);
                var responseBuffer = new byte[1000];
                var responseInfo = await webSocket.ReceiveAsync(responseBuffer, CancellationToken.None);
                responseData = ((Span<byte>) responseBuffer).Slice(0, responseInfo.Count).ToArray();
            } while (command != responseData);
        }

        private static class Commands
        {
            public static byte[] Ping()
            {
                byte commandId = 1;
                return BitConverter.GetBytes(commandId);
            }
            
            public static byte[] Blink(ushort milliseconds)
            {
                byte commandId = 2;
                byte dutyCycle = 50;
                return 
                    BitConverter.GetBytes(commandId)
                        .Concat(BitConverter.GetBytes(dutyCycle))
                        .Concat(BitConverter.GetBytes(milliseconds))
                        .ToArray();
            }

            public static byte[] Blank()
            {
                byte commandId = 3;
                return
                    BitConverter.GetBytes(commandId);
            }
         }
    }
    
}