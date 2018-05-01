﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server.Helpers
{
    public static class SocketHelper
    {
        const int port = 32768;
        private static TcpListener tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);

        private static bool status = false;
        public static bool Status
        {
            set
            {
                status = value;
                if (status)
                {
                    StartListener();
                }
                else
                {
                    StopListener();
                }
            }
            get { return status; }
        }

        public static void ChangeStatus()
        {
            
        }

        private static Thread processingOfListening = new Thread(() =>
            {
                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    new Thread(new ThreadStart(() => 
                    {
                        RequestHandler requestHandler = new RequestHandler(tcpClient);
                        requestHandler.Process();
                    })).Start();
                }
            });


        public static void StartListener()
        {
            tcpListener.Start();
            if(processingOfListening.ThreadState != ThreadState.Suspended)
            {
                processingOfListening.Start();
            }
            else
            {
                processingOfListening.Resume();
            }
        }

        public static void StopListener()
        {
            processingOfListening.Suspend();
            tcpListener.Stop();
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return null;
        }

    }
}
