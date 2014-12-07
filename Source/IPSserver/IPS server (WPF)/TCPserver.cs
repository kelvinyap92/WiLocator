////This program can receive TCP connection from client and then attempt to
//// compute the client's location based on the information given
////    Copyright (C) 2014, Davion Teh
////This file is part of "IPS server".

////    "IPS server" is free software: you can redistribute it and/or modify
////    it under the terms of the GNU General Public License as published by
////    the Free Software Foundation, either version 3 of the License, or
////    (at your option) any later version.

////    "IPS server" is distributed in the hope that it will be useful,
////    but WITHOUT ANY WARRANTY; without even the implied warranty of
////    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
////    GNU General Public License for more details.

////    You should have received a copy of the GNU General Public License
////    along with "IPS server".  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using tcpServer;

namespace IPS_server__WPF_
{
    /// <summary>
    /// Meh
    /// </summary>
    public class TCPserver
    {
        TcpServer tcpServer;

        /// <summary>
        /// Constructor that provides the information of the Server to TCPserver class
        /// </summary>
        public TCPserver()
        {
        tcpServer = new TcpServer();
        tcpServer.VerifyConnectionInterval = 1800000; //30min
        tcpServer.MaxSendAttempts = 4;

        tcpServer.OnConnect += tcpServer_OnConnect;
        tcpServer.OnDataAvailable += tcpServer_OnDataAvailable;
        }

        

        public int StartServer(int port)
        {             
            tcpServer.Port = (port < 1023) ? 80 : port;
            try
            {
                tcpServer.Open();
                Console.WriteLine(">>TCPserver - The server open at port {0}",port);
                return 0;
            }
            catch(Exception e)
            {
                Console.WriteLine("*ERROR* - TCPserver - Failed to start server on port {0}", port);
                Console.WriteLine(e);
                return -1;
            }
        }

        public int StopServer()
        {
            try
            {
                tcpServer.Close();
                Console.WriteLine(">>TCPserver - TcpListener stopped");
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("*ERROR* - TCPserver - Failed to stop server");
                Console.WriteLine(e);
                return -1;
            }
        }        

        private void tcpServer_OnConnect(TcpServerConnection connection)
        {
            Console.WriteLine(">>TCPserver - {0} connected",connection.Socket.Client.RemoteEndPoint);
            connection.sendData("Connected");
        }
        private void tcpServer_OnDataAvailable(tcpServer.TcpServerConnection connection)
        {
            string data = readStream(connection.Socket);
            string result;
            if (data != null)
            {
                    Console.WriteLine(">>Received Data, Message: {0}",data);
                    result = Parser.Parse(data);
                    if (result.Split(',')[0].CompareTo("Broadcast") != 0)
                        connection.sendData(result);
                    else
                        tcpServer.Send(result.Substring(10));

                data = null;
            }
        }

        protected string readStream(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] byteStream = new byte[1024];
            StringBuilder stringbuild = new StringBuilder();
            int bytereadCount = 0;

            if (stream.CanRead)
            {
                while (stream.DataAvailable)
                {
                    bytereadCount = stream.Read(byteStream, 0,byteStream.Length);
                    stringbuild.AppendFormat("{0}",Encoding.ASCII.GetString(byteStream,0,bytereadCount));
                }
                return stringbuild.ToString();
            }
            return null;
        }  
    }
}
