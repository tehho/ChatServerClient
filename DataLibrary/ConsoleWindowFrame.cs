using System.Collections.Generic;
using System.Data;

namespace System
{
    public class ConsoleWindowFrame
    {
        public int Width { get; set; }
        public int Height { get; set; }

        private string _question;
        private string _input;

        public Queue<WebMessage> Messages;

        private ThreadWraper _renderThread;
        private bool _needToReRender;

        public ConsoleWindowFrame()
        {
            Width = 40;
            Height = 10;
            Messages = new Queue<WebMessage>();
            _input = "";
            _question = "";
            _renderThread = null;
            _needToReRender = false;
            Console.CursorVisible = false;
        }
        
        public void Add(WebMessage message)
        {
            _needToReRender = true;
            Messages.Enqueue(message);
            if (Messages.Count > Height)
            {
                Messages.Dequeue();
            }
        }

        public void StartRender()
        {
            _renderThread = new ThreadWraper(Render);
            _needToReRender = true;
            _renderThread.Start();
        }

        public void Abort()
        {
            _renderThread.Abort();
        }

        public void Render()
        {
            if (_needToReRender)
            {
                int x = Console.CursorLeft;
                int y = Console.CursorTop;

                ResetDisplay();
                ResetInput();

                RenderDisplay();
                RenderInput();

                Console.SetCursorPosition(x,y);

                _needToReRender = false;
            }
        }

        private void RenderDisplay()
        {
                Console.SetCursorPosition(1, 1);

                foreach (var message in Messages)
                {
                    Console.WriteLine(message.ToString().FitTo(Width - 2));
                    Console.CursorLeft = 1;
                }
        }

        private void RenderInput()
        {
            Console.SetCursorPosition(1, Height + 1);
            Console.WriteLine(_question);

            Console.CursorLeft = 1;
            Console.WriteLine(_input);
        }

        void ResetDisplay()
        {
            Console.SetCursorPosition(0,0);

            DrawRow("+", "-");

            for (int i = 0; i < Height; i++)
            {
                DrawRow("|", " ");
            }

            DrawRow("+", "-");

        }

        void ResetInput()
        {
            Console.SetCursorPosition(0, Height);

            DrawRow("+", "-");

            for (int i = 0; i < 2; i++)
            {
                DrawRow("|", " ");
            }

            DrawRow("+", "-");
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

            _needToReRender = true;

            return GetInput();
        }

        public string GetInput()
        {
            _input = "";

            while (true)
            {
                var key = Console.ReadKey();

                
                if (key.Key == ConsoleKey.Enter)
                    break;

                if (key.Key == ConsoleKey.Backspace)
                {
                    _input = _input.Substring(0, _input.Length - 1);
                }
                else
                {
                    _input += key.KeyChar;
                }
                _needToReRender = true;

            }

            var ret = _input;

            _input = "";

            _needToReRender = true;

            return ret;
        }


    }
}