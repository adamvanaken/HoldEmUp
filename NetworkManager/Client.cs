using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkManager
{
    public class Client : IDisposable
    {
        Socket _sender;
        static ManualResetEvent connectDone;
        static ManualResetEvent sendDone;

        static Func<string,bool> _onMessageComplete;

        public Client(Func<string, bool> onMessageComplete)
        {
            connectDone = new ManualResetEvent(false);
            sendDone = new ManualResetEvent(false);

            _onMessageComplete = onMessageComplete;
        }

        public bool IsConnected()
        {
            return _sender.Connected;
        }

        public bool Connect(string host, int port)
        {
            IPHostEntry ipHostInfo = Dns.Resolve(host);
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            IPEndPoint ipe = new IPEndPoint(ipAddress, port);

            _sender = new Socket(ipe.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _sender.BeginConnect(ipe, new AsyncCallback(ConnectCallback), _sender);

                connectDone.WaitOne((int)TimeSpan.FromSeconds(30).TotalMilliseconds);

                return _sender.Connected;
            }
            catch (ArgumentNullException ae)
            {
                Console.WriteLine("ArgumentNullException : {0}", ae.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }

            return false;
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Receive(client);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        public void Send(string message)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            Send(bytes);
        }

        public void Send(byte[] bytes)
        {
            // Begin sending the data to the remote device.  
            _sender.BeginSend(bytes, 0, bytes.Length, SocketFlags.None,
                new AsyncCallback(SendCallback), _sender);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive(Socket client)
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.WorkSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.WorkSocket;
                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);
                if (bytesRead > 0)
                {
                    if (state.buffer[bytesRead - 1] == Constants.EOF)
                    {
                        // Store all except EOF
                        state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead - 1));

                        // End of our message; handle it and clear
                        string content = state.sb.ToString();
                        _onMessageComplete(content);
                        state.sb.Clear();
                    }
                    else
                    {
                        // There might be more data, so store the data received so far.
                        state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    }

                    //  Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    client.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
