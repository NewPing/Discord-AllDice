﻿using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord_AllDice.Classes
{
    public static class ReplyManager
    {
        public static async Task send_Async(SocketMessage message, string replyText)
        {
            Color color = Helper.getUserColor(message.Author);
            string inputMessage = message.Content.ToString().Split("!")[1];

            await sendSplitMessage(
                message,
                "würfelte einen " + inputMessage,
                replyText,
                color,
                true
                );
        }

        public static async Task send_Async(SocketMessage message, string caption, string replyText)
        {
            Color color = Helper.getUserColor(message.Author);

            await sendSplitMessage(
                message,
                caption,
                replyText,
                color,
                true
                );
        }

        public static async Task send_Async(SocketMessage message, string caption, string replyText, bool showAuthor)
        {
            Color color = Helper.getUserColor(message.Author);

            await sendSplitMessage(
                message,
                caption,
                replyText,
                color,
                showAuthor
                );
        }

        private static async Task sendSplitMessage(SocketMessage message, string caption, string replyText, Color color, bool showAuthor)
        {
            if (!String.IsNullOrWhiteSpace(replyText))
            {
                var splitString = splitIntoChunks(replyText, Helper.maxReplyLength);

                //send first block
                Embed embed;
                if (showAuthor)
                {
                    embed = new EmbedBuilder()
                    .WithAuthor(message.Author)
                    .WithColor(color)
                    .WithTitle(caption)
                    .WithDescription(splitString[0])
                    .Build();
                }
                else
                {
                    embed = new EmbedBuilder()
                    .WithColor(color)
                    .WithTitle(caption)
                    .WithDescription(splitString[0])
                    .Build();
                }

                await message.Channel.SendMessageAsync("", false, embed);

                //send following blocks
                for (int i = 1; i < splitString.Count; i++)
                {
                    embed = new EmbedBuilder()
                    .WithColor(color)
                    .WithDescription(splitString[0])
                    .Build();

                    await message.Channel.SendMessageAsync("", false, embed);
                }
            }
            else
            {
                Embed embed = new EmbedBuilder()
                    .WithAuthor(message.Author)
                    .WithColor(color)
                    .WithTitle(caption)
                    .WithDescription("")
                    .Build();

                await message.Channel.SendMessageAsync("", false, embed);
            }
        }

        public static List<string> splitIntoChunks(string str, int maxChunkSize)
        {
            List<string> chunks = new List<string>();
            for (int i = 0; i < str.Length; i += maxChunkSize)
            {
                chunks.Add(str.Substring(i, Math.Min(maxChunkSize, str.Length - i)));
            }
            return chunks;
        }
    }
}
