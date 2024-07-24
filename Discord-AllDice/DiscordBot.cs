using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Discord_AllDice.Classes;

namespace Discord_AllDice //https://discord.foxbot.me/stable/
{
    public class DiscordBot
    {
        private DiscordSocketClient _client = new DiscordSocketClient();
        private IServiceProvider? _services;
        private Commands? _commands;

        #region setup
        public async Task RunBotAsync()
        {
            string ownerID = "";
            try
            {
#if DEBUG
                ownerID = File.ReadAllText(Path.GetFullPath(@"..\..\..\") + "DiscordBotOwnerID.txt");
#else
                ownerID = File.ReadAllText("DiscordBotOwnerID.txt");
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal Error: Missing File: DiscordBotOwnerID.txt\n\n" + ex.ToString());
                return;
            }

            _commands = new Commands(ownerID);

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            string token = "";
            try
            {
#if DEBUG
                token = File.ReadAllText(Path.GetFullPath(@"..\..\..\") + "DiscordToken");
#else
                    token = File.ReadAllText("DiscordToken");
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal Erro: Missing File: DiscordToken\n\n" + ex.ToString());
                return;
            }

            _client.Log += _client_Log;

            RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, token);

            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }
        #endregion

        #region eventhandling

        public void RegisterCommandsAsync()
        {
            _client!.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message!.Author.IsBot)
            {
                return;
            }

            int argPos = 0;
            if (message.HasStringPrefix("!", ref argPos))
            {
                await _commands!.handleCommandInput(message);
            }
        }

        #endregion
    }
}
