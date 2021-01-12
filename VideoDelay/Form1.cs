using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoDelay
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource _backgroundCancellationToken;
        private Task _backgroundThread;

        public Form1()
        {
            InitializeComponent();
            
            // _backgroundCancellationToken = new CancellationTokenSource();
            // _backgroundThread = Task.Run(BackgroundTask, _backgroundCancellationToken.Token);
        }


        private void BackgroundTask()
        {
            var server = new TcpListener(IPAddress.Any, 0);
            BeginInvoke(new Action(() =>
            {
                
            }));
        }
    }
}