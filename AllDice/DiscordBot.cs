using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using AllDice.Classes;

namespace AllDice //https://discord.foxbot.me/stable/
{
    public class DiscordBot
    {
        private DiscordSocketClient _client;
        private IServiceProvider _services;
        private Commands _commands;
        
        #region setup
        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new Commands();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            #if DEBUG
                string token = File.ReadAllText(Path.GetFullPath(@"..\..\..\") + "DiscordToken");
            #else
                string token = File.ReadAllText("DiscordToken");
            #endif

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

        #region evenhandling

        public void RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            if (message.Author.IsBot)
            {
                return;
            }

            int argPos = 0;
            if (message.HasStringPrefix("!", ref argPos))
            {
                await _commands.handleCommandInput(message);
            }
        }

        #endregion
    }
}
