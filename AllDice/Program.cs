using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AllDice
{
    class Program
    {
        static void Main(string[] args)
        {
            new DiscordBot().RunBotAsync().GetAwaiter().GetResult();
        }
    }
}
