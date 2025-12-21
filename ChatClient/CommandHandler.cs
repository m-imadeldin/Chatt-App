using System;
using System.Threading.Tasks;

namespace ChatClientApp
{
    public class CommandHandler
    {
        private readonly ChatClient _client;
        private readonly MessageHistory _history;

        public CommandHandler(ChatClient client, MessageHistory history)
        {
            _client = client;
            _history = history;
        }

        public async Task HandleAsync(string input)
        {
            var parts = input.Split(' ', 3);
            var command = parts[0].ToLower();

            switch (command)
            {
                case "/help":
                    ShowHelp();
                    break;

                case "/quit":
                    await _client.DisconnectAsync();
                    Environment.Exit(0);
                    break;

                case "/history":
                    int n = parts.Length > 1 && int.TryParse(parts[1], out var x) ? x : 20;
                    _history.ShowLast(n);
                    break;

                case "/dm":
                    if (parts.Length < 3)
                        Console.WriteLine("Usage: /dm <user> <text>");
                    else
                        await _client.SendPrivateMessageAsync(parts[1], parts[2]);
                    break;

                default:
                    Console.WriteLine("Unknown command. Type /help.");
                    break;
            }
        }

        private void ShowHelp()
        {
            Console.WriteLine("/help          Show commands");
            Console.WriteLine("/quit          Exit program");
            Console.WriteLine("/history [n]   Show last messages");
            Console.WriteLine("/dm <u> <msg>  Direct message");
        }
    }
}