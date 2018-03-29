using System.Threading;
namespace System
{
    public class ThreadWraper
    {
        public delegate void ThreadHandler();
        private Thread _thread;
        private ThreadHandler _handler;
        private bool _running;

        public ThreadWraper(ThreadHandler handler)
        {
            _handler = handler;
        }

        public void Start()
        {
            _thread = new Thread(() =>
            {
                while (_running) _handler();
            });
            _running = true;
            _thread.Start();
        }

        public void Abort()
        {
            _running = false;
            while (_thread.IsAlive)
            {

            }

            _thread = null;
        }

    }
}