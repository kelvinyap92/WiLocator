/* THIS IS FOR WEBSOCKET CONNECTION ONLY
 * IF YOU'RE NOT USING WEBSOCKET CLIENT, DO NOT USE THIS CLASS
 * 
 * 
 * 
 * 
 */
 
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using Alchemy;
using Alchemy.Classes;

namespace IPS_server__WPF_
{
    class ClientHandler
    {
        IPAddress serverIP;
        int initCapacity = 101;
        int concurrencyLevel;
        WebSocketServer aServer;
        protected static ConcurrentDictionary<string,Connection> OnlineConnections;
        
        public ClientHandler()
        {
            concurrencyLevel = Environment.ProcessorCount * 2;
            OnlineConnections = new ConcurrentDictionary<string,Connection>(concurrencyLevel, initCapacity);
        }

        public void startServer(int port,out int exitCode)
        {
            
            serverIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Last(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            port = (port < 1023) ? 80 : port; //Use any port available from 1024-5000 if receive less than 1024

            try
            {
                aServer = new WebSocketServer(port, serverIP)
                {
                    OnReceive = OnReceive,
                    OnSend = OnSend,
                    OnConnected = OnConnect,
                    OnDisconnect = OnDisconnect,
                    TimeOut = new TimeSpan(0,30,0)

                };


                aServer.Start();
                Console.WriteLine(">>CHandler - aServer {0} started to listen at port \"{1}\"", serverIP, port);
                exitCode = 0;

            }
            catch(ArgumentNullException)
            {
                Console.WriteLine("*ERROR* | CHandler - aServer({0},{1}) - Localaddr is null!", serverIP, port);
                exitCode = -1;
            }
            catch(ArgumentOutOfRangeException)
            {
                Console.WriteLine("*ERROR* | CHandler - aServer({0},{1}) - Provided port is not in valid range!", serverIP, port);
                exitCode = -2;
            }           
            
        }

        public int stopServer()
        {
            try
            {
                aServer.Stop();
                Console.WriteLine(">>CHandler - TcpListener stopped");
                return 0;
            }
            catch(SocketException e)
            {
                Console.WriteLine("*ERROR* | CHandler - stopServer() - Error Code: {0} ", e.ErrorCode);
                return -1;
            }
        }

       public static void OnConnect(UserContext aContext)
        {            
            Console.WriteLine(">>CHandler - Client Connected From : " + aContext.ClientAddress.ToString());
            
            // Create a new Connection Object to save client context information
            Connection conn = new Connection { Context = aContext };
            //conn.IsAlive(aContext);
            // Add a connection Object to thread-safe collection
            OnlineConnections.TryAdd(aContext.ClientAddress.ToString(), conn);
             
        }
 
        
 
        public static void OnReceive(UserContext aContext)
        {
            Console.WriteLine(">>CHandler - Received message: " +aContext.DataFrame.ToString());
            try
            {
                string input = aContext.DataFrame.ToString();
                string result = "";
                if (string.Compare(input, "ACK") == 0)
                {
                    Console.WriteLine(">>CHandler - [" + aContext.ClientAddress.ToString() + "] replied ACK packet");
                }
                else
                {
                    result = Parser.Parse(input);
                    aContext.Send(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(">>CH ERROR - {0}",ex.Message.ToString());
            }
             
        }
        public static void OnSend(UserContext aContext)
        {            
            Console.WriteLine(">>CHandler - Data Sent To : " + aContext.ClientAddress.ToString());
        }
        public static void OnDisconnect(UserContext aContext)
        {
            Console.WriteLine(">>CHandler - Client Disconnected : " + aContext.ClientAddress.ToString());
 
            // Remove the connection Object from the thread-safe collection
            DisconnectClient((object)aContext);
        }

        private static void DisconnectClient(object fella)
        {
            UserContext aContext = (UserContext)fella;
            Connection conn;
            OnlineConnections.TryRemove(aContext.ClientAddress.ToString(), out conn);            
        }

        public class Connection
        {
            public UserContext Context { get; set; }
            public Connection() { }
                                          
            

            public void IsAlive(object state)
            {
                try
                {
                    // Sending Data to the Client
                    Context.Send("ALIVE");
                    Console.WriteLine(">>CHandler - Sent IsAlive() packet");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            
        }
    }
    
}
