using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace ChatClient
{
    class Program
    {
        private const int SIO_UDP_CONNRESET = -1744830452;

        static void Main(string[] args)
        {
            string remoteName = "localhost", textMessage = "Client: This is a test";
            int remotePort = 5150;
            bool running = true;

            PackageData test = new PackageData();


            Print("Enter your name");
            string input = Console.ReadLine();
            test.Add(input);
            test.Add("Kokalolasosmomatot");

            try
            {
                Client client = new Client(input);


                if (!client.Connect(remoteName, remotePort))
                    throw new InvalidOperationException("Error occurred creating the connection");

                client.Send(test);


                client.StartReceiving((data) =>
                {
                    try
                    {
                        while (!data.EOF)
                        {
                            string name = data.GetString();
                            string message = data.GetString();

                            StatusMessage(message, name);
                        }
                    }
                    catch (Exception e)
                    {
                    }
                });

                while (running)
                {
                    Console.WriteLine("Skriv ett meddelande: ");
                    string message = Console.ReadLine();

                    if (message.ToLower() == "quit")
                    {
                        running = false;
                        continue;
                    }

                    client.SendMessage(message);
                }

                client.Close();
            }
            catch (Exception e)
            {
                ErrorMessage(e.Message);
            }

            PressAnyKey();

        }

        static void PressAnyKey()
        {
            Console.WriteLine("Press any key to continue. . . ");
            Console.ReadKey();
        }

        public static void StatusMessage(string message)
        {
            StatusMessage(message, "Client");
        }
        public static void StatusMessage(string message, string user)
        {
            Print($"{user}: {message}", ConsoleColor.Green);
        }
        public static void ErrorMessage(string message)
        {
            Print($"Client: {message}", ConsoleColor.Red);
        }

        static void Print(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;

            Print(message);

            Console.ResetColor();
        }

        public static void Print(string message)
        {
            Console.WriteLine(message);
        }

    }
}
