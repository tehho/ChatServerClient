using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace ChatClient
{
    class Program
    {
        private const int SIO_UDP_CONNRESET = -174830452;
        private static Client client;
        private static bool running;
        private static string remoteName;
        private static int remotePort;

        private static ConsoleWindowFrame window;


        static void Main(string[] args)
        {
            window = new ConsoleWindowFrame
            {
                Width = 70,
                Height = 20
            };

            window.StartRender();

            try
            { 
                remoteName = "localhost";
                remotePort = 5150;
                running = true;
                client = null;

                string input = window.GetInputWithQuestion("Enter your name: ");

                client = new Client(input);

                while (running)
                {
                    try
                    {
                        string message = window.GetInputWithQuestion("Skriv ett meddelande: ");
                        
                        DecodeMessage(message);

                    }
                    catch (Exception e)
                    {
                    }
                }

                client.Close();
            }
            catch (Exception e)
            {
                ErrorMessage(e.Message);
            }

            window.Abort();

            PressAnyKey();

        }

        static void PressAnyKey()
        {
            window.GetInputWithQuestion("Press <ENTER> to continue. . . ");
        }

        public static void DecodeMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            if (message.ToLower() == "help" || message.ToLower() == "?")
            {
                ShowHelp();
                return;
            }

            if (message.ToLower().IndexOf("command:") == 0)
            {
                DecodeCommand(message);
            }
            else
            {
                client.SendMessage(message);
            }
        }

        private static void ShowHelp()
        {
            StatusMessage("Here is the help meny, most commands need <command:> before.");
            StatusMessage("Quit -> Quits the program");
            StatusMessage("Connect -> Tries to connect to the target host");
            StatusMessage("sethost:NAME|IP -> Sets the target server to Name by dns or IP direkt");
        }

        private static void DecodeCommand(string message)
        {
            var list = message.Split(':');
            var commando = list[1].ToLower();

            if (commando.Contains("quit"))
            {
                StatusMessage("Exiting program...");
                running = false;
                return;
            }

            if (commando == "sethost")
            {
                remoteName = list[2];
                StatusMessage($"Setting target to {list[2]}");
            }
            else if (commando == "setname")
            {
                client.Name = list[2];
                StatusMessage($"Setting name to {list[2]}");

            }
            else if (commando.Contains("connect"))
            {
                if (client.Connect(remoteName, remotePort))
                {
                    StatusMessage("Starting receiving from server...");
                    client.StartReceiving((data) =>
                    {
                        try
                        {
                            while (!data.EOF)
                            {
                                string name = data.GetString();
                                string messageData = data.GetString();

                                window.Add(new WebMessage(name, messageData));
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    });
                }
                else
                {
                    StatusMessage("Connection failed...");
                }
            }
            else if (list[1] == "help")
            {
                ShowHelp();
            }
            else
            {
                StatusMessage($"Unknown command {list[1]}");
            }
        }
        

        public static void StatusMessage(string message)
        {
            window.Add(new WebMessage("System", message));
        }
        public static void StatusMessage(string message, string user)
        {

            Print($"{user}: {message}", ConsoleColor.Green);
        }
        public static void ErrorMessage(string message)
        {
            window.Add(new WebMessage("Error", message));
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
            Console.CursorLeft = 1;
        }

    }
}
