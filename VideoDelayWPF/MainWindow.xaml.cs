using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QRCoder;

namespace VideoDelayWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Task _websocketServer;
        private CancellationTokenSource _websocketCancel;
        
        // ReSharper disable once InconsistentNaming
        public event EventHandler<Bitmap> IPGenerated; 
        public event EventHandler<string> ClientConnected; 

        public MainWindow()
        {
            InitializeComponent();
            IPGenerated += (_, b) =>
            {
                Dispatcher.Invoke(() =>
                {
                    QRCode.Source = b.ToImageSource();
                });
            };

            ClientConnected += (_, clientIP) =>
            {
                Dispatcher.Invoke(() =>
                {
                    QRCode.Visibility = Visibility.Collapsed;
                    TextBlock.Visibility = Visibility.Visible;
                    TextBlock.Text = clientIP;
                });
            };
        }
        
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            
            if (_websocketServer != null) return;
            
            _websocketCancel = new CancellationTokenSource();
            _websocketServer = Task.Run(RunWebSocketServer, _websocketCancel.Token);
        }

        private  void RunWebSocketServer()
        {
            var server = new TcpListener(IPAddress.Any, 0);
            server.Start();
            var endpoint = (IPEndPoint) server.LocalEndpoint;
            var qrGenerator = new QRCodeGenerator();
            var qrData = qrGenerator.CreateQrCode($"ws://{GetLocalIp()}:{endpoint.Port}", QRCodeGenerator.ECCLevel.M);
            
            IPGenerated?.Invoke(this, new QRCode(qrData).GetGraphic(5));

            var client = server.AcceptSocket();
            ClientConnected?.Invoke(this, $"{client.LocalEndPoint}");
        }

        /// <summary>
        /// From: https://stackoverflow.com/a/27376368
        /// </summary>
        /// <returns></returns>
        private string GetLocalIp()
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("8.8.8.8", 65530);
            var endPoint = socket.LocalEndPoint as IPEndPoint ?? throw new Exception($"LocalEndpoint {socket.LocalEndPoint} is not an IPEndpoint");
            return endPoint.Address.ToString();
        }
        
    }
    
    static class BitmapExtensions 
    {
        /// <summary>
        /// From https://stackoverflow.com/a/22501616
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapImage ToImageSource(this Bitmap bitmap)
        {
            using MemoryStream memory = new MemoryStream();
            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Position = 0;
            var bitmapimage = new BitmapImage();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = memory;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();

            return bitmapimage;
        }    
    }
}