using NetworkManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client(HandleMessage);
            if (client.Connect("127.0.0.1", 2104))
            {
                while (client.IsConnected())
                {
                    client.Send("Test" + (char)Constants.EOF);
                    Console.WriteLine("Sent");

                    Thread.Sleep(5000);
                }
            }
            else
            {
                throw new Exception("failed to connect");
            }
        }

        static bool HandleMessage(string message)
        {
            Console.WriteLine(message);
            return true;
        }
    }
}
