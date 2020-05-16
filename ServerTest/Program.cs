using NetworkManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server(HandleMessage, null);
            server.StartListening(2104);
        }

        static bool HandleMessage(string message)
        {
            Console.WriteLine(message);
            return true;
        }
    }
}
