using System;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using SocketIOClient;

namespace ChatClientApp
{
    public class ChatClient
    {
        private readonly SocketIO _socket;
        private readonly User _user;
        private readonly MessageHistory _history;

        public ChatClient(User user, MessageHistory history, string serverUrl)
        {
            _user = user;
            _history = history;

            _socket = new SocketIO(serverUrl, new SocketIOOptions
            {
                Path = "/sys25d",
                Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
            });

            RegisterHandlers();
        }

        private void RegisterHandlers()
        {
            _socket.OnConnected += async (_, _) =>
            {
                Console.WriteLine("Connected to server.");
                Console.WriteLine($"*** {_user.Username} joined the chat ***");
                await _socket.EmitAsync("join", new { username = _user.Username });
            };

            _socket.On("join", r =>
            {
                string name = GetUsername(r);
                if (name != _user.Username)
                    PrintSystem($"*** {name} joined the chat ***");
            });

            _socket.On("leave", r =>
            {
                string name = GetUsername(r);
                PrintSystem($"*** {name} left the chat ***");
            });

            _socket.On("chat_message", r =>
            {
                var obj = r.GetValue<JsonObject>();
                string sender = obj["username"]?.ToString() ?? "Unknown";
                string text = obj["message"]?.ToString() ?? "";
                string time = obj["time"]?.ToString() ?? DateTime.Now.ToString("HH:mm");

                Console.WriteLine($"[{time}] {sender}: {text}");

                _history.Add(new Message
                {
                    Sender = sender,
                    Text = text,
                    Timestamp = DateTime.Now
                });
            });
        }

        private void PrintSystem(string text)
        {
            Console.WriteLine(text);
            _history.Add(new Message
            {
                Sender = "System",
                Text = text,
                Timestamp = DateTime.Now
            });
        }

        private static string GetUsername(SocketIOResponse r)
        {
            try
            {
                var obj = r.GetValue<JsonObject>();
                return obj["username"]?.ToString() ?? "Unknown";
            }
            catch
            {
                try { return r.GetValue<string>(); }
                catch { return "Unknown"; }
            }
        }

        public async Task ConnectAsync()
        {
            Console.WriteLine("Connecting...");
            await _socket.ConnectAsync();
        }

        public async Task SendMessageAsync(string text)
        {
            await _socket.EmitAsync("chat_message", new
            {
                username = _user.Username,
                message = text,
                time = DateTime.Now.ToString("HH:mm")
            });

            Console.WriteLine($"You: {text}");
        }

        public async Task SendPrivateMessageAsync(string recipient, string text)
        {
            await _socket.EmitAsync("private_message", new
            {
                from = _user.Username,
                to = recipient,
                message = text
            });

            Console.WriteLine($"(DM to {recipient}) {text}");
        }

        public async Task DisconnectAsync()
        {
            // 👇 NYTT: visa lokal leave alltid
            Console.WriteLine($"*** {_user.Username} left the chat ***");

            try
            {
                await _socket.EmitAsync("leave", new { username = _user.Username });
            }
            catch { }

            try
            {
                await _socket.DisconnectAsync();
            }
            catch { }

            Console.WriteLine("You have left the chat.");
        }
    }
}
