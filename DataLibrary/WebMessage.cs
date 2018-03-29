namespace System
{
    public class WebMessage
    {
        private string _sender;
        private string _message;

        public string From => _sender;

        public string Message => _message;

        public WebMessage(string sender, string message)
        {
            _sender = sender;
            _message = message;
        }

        public override string ToString()
        {
            return $"{_sender}: {_message}";
        }
    }
}