using System.Collections.Generic;
using System.Data;

namespace System
{
    public class ConsoleWindowFrame
    {
        public int Width { get; set; }
        public int Height { get; set; }

        private string _question;

        private string input;

        public Queue<WebMessage> _messages;

        private ThreadWraper _renderThread;
        private bool needToReRender;

        public ConsoleWindowFrame()
        {
            Width = 40;
            Height = 10;
            _messages = new Queue<WebMessage>();
            input = "";
            _question = "";
            _renderThread = null;
            needToReRender = false;
            Console.CursorVisible = false;
        }
        
        public void Add(WebMessage message)
        {
            needToReRender = true;
            _messages.Enqueue(message);
            if (_messages.Count > Height)
            {
                _messages.Dequeue();
            }
        }

        public void StartRender()
        {
            _renderThread = new ThreadWraper(Render);
            needToReRender = true;
            _renderThread.Start();
        }

        public void Abort()
        {
            _renderThread.Abort();
        }

        public void Render()
        {
            if (needToReRender)
            {
                ResetDisplay();

                ResetInput();

                RenderDisplay();

                RenderInput();

                needToReRender = false;
            }
        }

        private void RenderDisplay()
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;

            Console.SetCursorPosition(1,1);

            foreach (var message in _messages)
            {
                Console.WriteLine(message);
                Console.CursorLeft = 1;
            }

            Console.SetCursorPosition(x,y);
        }

        private void RenderInput()
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;

            Console.SetCursorPosition(1, Height + 1);
            Console.WriteLine(_question);

            Console.CursorLeft = 1;
            Console.WriteLine(input);

            Console.SetCursorPosition(x, y);
        }

        void ResetDisplay()
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;

            Console.SetCursorPosition(0,0);

            DrawRow("+", "-");

            for (int i = 0; i < Height; i++)
            {
                DrawRow("|", " ");
            }

            DrawRow("+", "-");

            Console.SetCursorPosition(x,y);

        }

        void ResetInput()
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;

            Console.SetCursorPosition(0, Height);

            DrawRow("+", "-");

            for (int i = 0; i < 2; i++)
            {
                DrawRow("|", " ");
            }

            DrawRow("+", "-");

            Console.SetCursorPosition(x, y);
        }


        void DrawRow(string edge, string content)
        {
            string temp = edge;
            for (int i = 0; i < Width; i++)
            {
                temp += content;
            }

            temp += edge;

            Console.WriteLine(temp);

        }


        public string GetInputWithQuestion(string question)
        {
            _question = question;

            return GetInput();
        }

        public string GetInput()
        {
            input = "";

            while (true)
            {
                var key = Console.ReadKey();
                needToReRender = true;
                if (key.Key == ConsoleKey.Enter)
                    break;

                input += key.KeyChar;
            }

            var ret = input;
            input = "";
            return ret;
        }


    }
}