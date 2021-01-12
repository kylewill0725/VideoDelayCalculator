using System;

namespace VideoDelayWPF.MainApp.Shared
{
    public static class Events
    {
        public static event EventHandler<int>? OnWebServerStarted;

        public static void WebServerStarted(int port)
        {
            OnWebServerStarted?.Invoke(null, port);
        }
    }
}