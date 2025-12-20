using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatClientApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Chat Client";
            Console.WriteLine("== Chat Client with Socket.IO ==");

            string username = PromptUsername();
            var user = new User(username);

            var history = new MessageHistory();
            history.Load();

            const string ServerUrl = "wss://api.leetcode.se";

            var client = new ChatClient(user, history, ServerUrl);
            var handler = new CommandHandler(client, history);

            Console.CancelKeyPress += async (_, e) =>
            {
                e.Cancel = true;
                await client.DisconnectAsync();
                Environment.Exit(0);
            };

            await client.ConnectAsync();
            Console.WriteLine("Type /help for commands.");

            while (true)
            {
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) continue;

                if (input.StartsWith("/"))
                    await handler.HandleAsync(input);
                else
                    await client.SendMessageAsync(input);
            }
        }

        private static string PromptUsername()
        {
            var regex = new Regex(@"^[A-Za-z0-9_-]{3,16}$");

            while (true)
            {
                Console.Write("Enter username (3–16 chars, A–Z 0–9 _ -): ");
                var input = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Username cannot be empty.");
                    continue;
                }

                if (!regex.IsMatch(input))
                {
                    Console.WriteLine("Invalid username format.");
                    continue;
                }

                return input;
            }
        }
    }
}
