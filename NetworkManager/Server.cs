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
    public class Server : IDisposable
    {
        Socket _listener;
        ManualResetEvent allDone;
        static Func<string, bool> _onMessageComplete;
        static Func<string> _getAckMessage;
        static Dictionary<IntPtr, Socket> _clients;

        public Server(Func<string, bool> onMessageComplete, Func<string> getAckMessage)
        {
            _clients = new Dictionary<IntPtr, Socket>();
            allDone = new ManualResetEvent(false);
            _onMessageComplete = onMessageComplete;
            _getAckMessage = getAckMessage;
        }

        private static string GetLocalIPAddress()
        {
#if DEBUG
            return "127.0.0.1";
# endif

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public void StartListening(int port)
        {
            var ipAddress = IPAddress.Parse(GetLocalIPAddress());
            IPEndPoint localEP = new IPEndPoint(ipAddress, port);

            Console.WriteLine($"Local address and port : {localEP.ToString()}");

            _listener = new Socket(localEP.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _listener.Bind(localEP);
                _listener.Listen(10);

                while (true)
                {
                    allDone.Reset();

                    Console.WriteLine("Waiting for a connection...");
                    _listener.BeginAccept(new AsyncCallback(AcceptCallback), _listener);

                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("Closing the listener...");
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket handler = _listener.EndAccept(ar);

            allDone.Set();

            StateObject state = new StateObject();
            state.WorkSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.WorkSocket;

            try {
                // Read data from the client socket.  
                int read = handler.EndReceive(ar);

                // Data was read from the client socket.  
                if (read > 0)
                {
                    if (state.buffer[read - 1] == Constants.EOF)
                    {
                        // Store all except EOF
                        state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, read - 1));

                        // End of our message; handle it and clear
                        string content = state.sb.ToString();

                        // Maintain this connected socket
                        if (!_clients.ContainsKey(handler.Handle))
                        {
                            _clients[handler.Handle] = handler;
                        }

                        if (_onMessageComplete(content))
                        {
                            state.sb.Clear();

                            var res = _getAckMessage();
                            byte[] bytes = Encoding.ASCII.GetBytes(res + (char)Constants.EOF);

                            // Add our null-terminating byte if not already present
                            if (bytes[bytes.Length - 1] != (char)Constants.EOF)
                            {
                                Array.Resize(ref bytes, bytes.Length + 1);
                                bytes[bytes.Length - 1] = Constants.EOF;
                            }

                            state.WorkSocket.Send(bytes);
                        }
                    }
                    else
                    {
                        // There might be more data, so store the data received so far.
                        state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, read));
                    }

                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
            }
            else
            {
                handler.Close();
            }
            }

            catch { }
        }

        public void PushToListeners(string message)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            SendToListeners(bytes);
        }

        public void SendToListeners(byte[] bytes)
        {
            // Add our null-terminating byte if not already present
            if (bytes[bytes.Length - 1] != (char)Constants.EOF)
            {
                Array.Resize(ref bytes, bytes.Length + 1);
                bytes[bytes.Length - 1] = Constants.EOF;
            }

            // Begin sending the data to the remote device.  
            foreach (var socket in _clients.Values)
            {
                if (socket.Connected)
                {
                    try
                    {
                        socket.Send(bytes);
                    }
                    catch
                    {

                    }
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
