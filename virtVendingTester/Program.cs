using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace virtVendingTester
{
    class Program
    {
        static bool exit = false;

        static void Main(string[] args)
        {
            CheckArgs(args);

            while (!exit)
            {
                RunOption(Console.ReadLine());
            }
        }

        static void CheckArgs(string[] args)
        {
            if (args.Length == 0)
                DisplayInfo();
            else
            {
                RunOption(args.ToString());
            }
        }

        static void DisplayInfo()
        {
            Console.WriteLine("VirtVendingTester.exe tests virtVendingMachine");
            Console.WriteLine("Available Arguments:");
            Console.WriteLine("-h =Help");
            Console.WriteLine("-p =Price list of items");
            Console.WriteLine("-s =All items in stock");
            Console.WriteLine("-q =Quantities of items");
            Console.WriteLine("-b A1 0.85 =Buy an item");
            Console.WriteLine("-f =Fill inventory (preset list)");
            Console.WriteLine("-e =Exit program");
            Console.WriteLine();
            Console.WriteLine();
        }

        static void RunOption(string option)
        {
            char bit = option[1];
            byte[] message = Encoding.ASCII.GetBytes("Test client");

            switch (bit)
            {
                case 'h':
                    DisplayInfo();
                    break;
                case 'b':
                case 'p':
                case 's':
                case 'q':
                case 'f':
                    message = Encoding.ASCII.GetBytes(option.ToString() + " <EOF>");
                    SendAndReceive(message);
                    break;
                case 'e':
                    message = Encoding.ASCII.GetBytes(option.ToString() + " <EOF>");
                    SendAndReceive(message);
                    exit = true;
                    break;
            }
        }

        static void SendAndReceive(byte[] message)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEP = new IPEndPoint(ipAddr, 3000);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                sender.Connect(localEP);

                Console.WriteLine("Socket connected to: {0}", sender.RemoteEndPoint.ToString());

                int byteSent = sender.Send(message);

                byte[] messageReceived = new byte[1024];

                int byteRec = sender.Receive(messageReceived);
                Console.WriteLine("**********");
                Console.WriteLine("Message from server: ");
                Console.WriteLine(Encoding.ASCII.GetString(messageReceived, 0, byteRec));
                Console.WriteLine();
            }

            catch (ArgumentNullException anx)
            {
                Console.WriteLine("Argument Null Exception: {0}", anx.ToString());
            }

            catch (SocketException sx)
            {
                Console.WriteLine("Socket Exception: {0}", sx.ToString());
            }

            catch (Exception x)
            {
                Console.WriteLine("Exception thrown: {0}", x.ToString());
            }

            finally
            {
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
        }
    }
}
