using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ChatClientApp
{
    public class MessageHistory
    {
        private const string FileName = "chat_history.json";
        private readonly List<Message> _messages = new();

        public void Add(Message message)
        {
            _messages.Add(message);
            Save();
        }

        public void ShowLast(int count)
        {
            foreach (var msg in _messages.TakeLast(count))
                Console.WriteLine(msg);
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(_messages, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FileName, json);
        }

        public void Load()
        {
            if (!File.Exists(FileName)) return;

            var json = File.ReadAllText(FileName);
            var messages = JsonSerializer.Deserialize<List<Message>>(json);

            if (messages != null)
                _messages.AddRange(messages);
        }
    }
}
