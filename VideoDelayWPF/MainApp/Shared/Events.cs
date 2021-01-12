using System;

namespace VideoDelayWPF.MainApp.Shared
{
    public static class Events
    {
        public static event EventHandler<string>? OnWebServerStarted;

        public static void WebServerStarted(string address)
        {
            OnWebServerStarted?.Invoke(null, address);
        }
    }
}