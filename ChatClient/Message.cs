using System;

namespace ChatClientApp
{
    public class Message
    {
        public string Sender { get; set; } = "";
        public string Text { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm}] {Sender}: {Text}";
        }
    }
}
