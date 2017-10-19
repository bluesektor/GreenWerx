using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace TreeMon.Client.Utilities
{
    public class NetworkEx
    {
        protected NetworkEx() { }

        //One of many answers from https://stackoverflow.com/questions/6803073/get-local-ip-address
        //
        public static string GetIp() {
            string localIP = "";
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    localIP = endPoint.Address.ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
            }
            return localIP;
        }
    }
}
