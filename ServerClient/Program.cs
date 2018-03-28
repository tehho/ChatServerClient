using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace ChatClient
{
    class Program
    {
        private const int SIO_UDP_CONNRESET = -1744830452;
        private static  bool running;
        static void Main(string[] args)
        {
            Server server = new Server();

            server.StartListning();

            running = true;

            while (running)
            {
                var command = Console.ReadLine();

                if (command.ToLower().IndexOf("\\command") == 0)
                    decodeCommando(command);

                server.SendMessage(command);

            }

            server.Close();

            PressAnyKey();
        }

        static void decodeCommando(string command)
        {
            if (command.Contains("quit"))
                running = false;
            
        }

        static void PressAnyKey()
        {
            Console.WriteLine("Press any key to continue. . . ");
            Console.ReadKey();
        }

        public static void StatusMessage(string message)
        {
            Print($"Server: {message}", ConsoleColor.Green);
        }

        public static void ErrorMessage(string message)
        {
            Print($"Server: {message}", ConsoleColor.Red);
        }

        static void Print(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;

            Print(message);

            Console.ResetColor();
        }

        static void Print(string message)
        {
            Console.WriteLine(message);
        }
    }
}