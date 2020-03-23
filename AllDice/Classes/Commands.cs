using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AllDice.Classes
{
    public class Commands
    {
        public List<CommandDef> commands_def = new List<CommandDef>();

        #region setup
        public Commands(string ownerID)
        {
            Helper.ownerID = ownerID;
            initializeCommands();
        }

        private void initializeCommands()
        {
            commands_def.Add(new CommandDef(
                0,
                @"^!help(:?( [0-9]+)?)$",
                "!help ([index]): gibt die Hilfeseite aus.",
                "!help 1",
                help_Async)); //help [command]
            commands_def.Add(new CommandDef(
                1,
                @"^!((?:[0-9])?)+w[0-9]+((:?((:?\+)?|(:?\-)?)[0-9]+)?)$",
                "!([zahl])w[zahl](+/-[zahl]) : würfelt einen virtuellen Würfel mit der angegebenen Augenzahl.",
                "!2w6+1",
                w_Async)); //[zahl]w[zahl]+/-[zahl]
            commands_def.Add(new CommandDef(
                2,
                @"^!enableChannel$",
                "!enableChannel : aktivert einen deaktivierten Channel",
                "!enableChannel",
                enableChannel_Async));
            commands_def.Add(new CommandDef(
                3,
                @"^!disableChannel$",
                "!disableChannel : deakativert einen aktiven Channel (Nachrichten in diesem Channel werden dann vom Bot ignoriert)",
                "!disableChannel",
                disableChannel_Async));
            commands_def.Add(new CommandDef(
                3,
                @"^!showDisabledChannels",
                "!showDisabledChannels : listet alle deaktiverten Channels auf",
                "!showDisabledChannels",
                showDisabledChannels_Async));
            commands_def.Add(new CommandDef(
                4,
                @"^!test$",
                "-",
                "-",
                test_Async));
        }

        public async Task handleCommandInput(SocketUserMessage message)
        {
            bool commandFound = false;
            
            foreach (CommandDef command in commands_def) //mögliche Commands durchlaufen
            {
                if (command.matchPattern.IsMatch(message.Content))
                {
                    commandFound = true;
                    await command.function(message);
                }
            }
            
            if (commandFound == false)
            {
                await ReplyManager.send_Async(message, "Syntax Error", "Kein Befehl mit diesem Syntax gefunden...");
                await help_Async(message);
            }
        }
        #endregion

        #region command_functions
        private async Task help_Async(SocketUserMessage message)
        {
            try
            {
                if (Helper.isChannelEnabled(message.Channel.Id.ToString()))
                {
                    string reply = "";
                    MatchCollection values = new Regex(@"\d+").Matches(message.Content);

                    if (values.Count == 0) //ausgabe mögliche befehle (!help)
                    {
                        foreach (CommandDef command in commands_def)
                        {
                            reply += command.index + " - " + command.description + "\n";
                        }

                        await ReplyManager.send_Async(message, "❓ - Hilfeseite : Mögliche Befehle - ❓", reply);
                    }
                    else //ausgabe hilfe für befehl (!help [zahl])
                    {
                        int inputNumber = Int32.Parse(values[0].ToString());

                        foreach (CommandDef command in commands_def)
                        {
                            if (inputNumber == command.index)
                            {
                                reply += command.description + "\n";
                                reply += "- Beispiel: " + command.example;
                            }
                        }

                        await ReplyManager.send_Async(message, "❓ - Hilfeseite : Command - ❓", reply);
                    }
                }
            }
            catch (Exception)
            {
                await ReplyManager.send_Async(message, "Exception in help_Async... Versuche es bitte erneut mit anderen Inputs...");
            }
        }

        private async Task w_Async(SocketUserMessage message)
        {
            try
            {
                if (Helper.isChannelEnabled(message.Channel.Id.ToString()))
                {
                    string blancOutput = Helper.blanc_w_Output;
                    int[] inputNumbers = null;
                    int[] randNumbers = null;
                    string reply = "";
                    int sum = 0;

                    MatchCollection values = new Regex(@"\d+").Matches(message.Content);
                    inputNumbers = new int[values.Count];
                    for (int i = 0; i < values.Count; i++)
                    {
                        inputNumbers[i] = Int32.Parse(values[i].ToString());
                    }
                    randNumbers = new int[inputNumbers[0]];

                    if (inputNumbers.Length == 1)
                    {
                        #region Logik_w3
                        if (inputNumbers[0] < 1)
                        {
                            await ReplyManager.send_Async(message, "Syntax Error", "❗Syntax Error : Eingabe muss größer als 0 sein!");
                            return;
                        }
                        else
                        {
                            randNumbers[0] = Helper.getRandomNumber(inputNumbers[0]);
                            //Output zusammenbauen
                            blancOutput = blancOutput.Replace("+", "");
                            reply += blancOutput;
                            reply = reply.Replace("$RANDNUMBER$", randNumbers[0].ToString());
                            sum = randNumbers[0];
                            reply = reply.Replace("$SUM$", sum.ToString());
                            reply = reply.Replace("$ADD$", "");
                            reply = reply.Replace("$RESULT$", (sum).ToString());

                            await ReplyManager.send_Async(message, reply);

                        }
                        #endregion
                    }
                    else if (inputNumbers.Length == 2)
                    {
                        if (message.Content.Contains("+") || message.Content.Contains("-"))
                        {

                            #region Logik_w3+3 
                            if (inputNumbers[0] < 1)
                            {
                                await ReplyManager.send_Async(message, "Syntax Erro", "❗Syntax Error : Eingabe muss größer als 0 sein!");
                                return;
                            }
                            else
                            {
                                if (message.Content.Contains("-"))
                                {
                                    inputNumbers[1] = -inputNumbers[1];  //letzte zahl umkehren
                                    blancOutput = blancOutput.Replace("+", "");
                                }
                                //______________Logik w3+3______________________
                                randNumbers[0] = Helper.getRandomNumber(inputNumbers[0]);
                                //Output zusammenbauen
                                reply += blancOutput;
                                reply = reply.Replace("$RANDNUMBER$", randNumbers[0].ToString());
                                sum = randNumbers[0];
                                reply = reply.Replace("$SUM$", sum.ToString());
                                reply = reply.Replace("$ADD$", inputNumbers[1].ToString());
                                reply = reply.Replace("$RESULT$", (sum + inputNumbers[1]).ToString());

                                await ReplyManager.send_Async(message, reply);
                            }
                            #endregion
                        }
                        else
                        {
                            #region Logik_3w3
                            if (inputNumbers[0] < 1 || inputNumbers[1] < 1)
                            {
                                await ReplyManager.send_Async(message, "Syntax Erro", "❗Syntax Error : Eingabe muss größer als 0 sein!");
                                return;
                            }
                            else
                            {
                                //______________Logik 3w3______________________
                                for (int i = 0; i < inputNumbers[0]; i++) //random zahlen generieren
                                {
                                    randNumbers[i] = Helper.getRandomNumber(inputNumbers[1]);
                                }
                                //Output zusammenbauen
                                reply += blancOutput;
                                reply = reply.Replace("$RANDNUMBER$", randNumbers[0] + "+$RANDNUMBER$");
                                sum = randNumbers[0];
                                for (int i = 1; i < randNumbers.Length; i++)
                                {
                                    reply = reply.Replace("$RANDNUMBER$", randNumbers[i] + "+$RANDNUMBER$");
                                    sum += randNumbers[i];
                                }
                                reply = reply.Replace("+$RANDNUMBER$", "");
                                reply = reply.Replace("$SUM$", sum.ToString());
                                reply = reply.Replace("+$ADD$", "");
                                reply = reply.Replace("$RESULT$", (sum).ToString());

                                await ReplyManager.send_Async(message, reply);
                            }
                            #endregion
                        }
                    }
                    else if (inputNumbers.Length == 3 || inputNumbers[1] < 1) //3w3+3 OR 3w3-3
                    {
                        #region Logik_3w3+3
                        if (inputNumbers[0] < 1)
                        {
                            await ReplyManager.send_Async(message, "Syntax Erro", "❗Syntax Error : Eingabe muss größer als 0 sein!");
                            return;
                        }
                        else
                        {
                            if (message.Content.Contains("-"))
                            {
                                inputNumbers[2] = 0 - inputNumbers[2];  //letzte zahl umkehren
                                blancOutput = blancOutput.Replace("+", "");
                            }
                            //______________Logik 3w3+3______________________
                            for (int i = 0; i < inputNumbers[0]; i++) //random zahlen generieren
                            {
                                randNumbers[i] = Helper.getRandomNumber(inputNumbers[1]);
                            }
                            //Output zusammenbauen
                            reply += blancOutput;
                            reply = reply.Replace("$USERNAME$", message.Author.Username);
                            reply = reply.Replace("$RANDNUMBER$", randNumbers[0] + "+$RANDNUMBER$");
                            sum = randNumbers[0];
                            for (int i = 1; i < randNumbers.Length; i++)
                            {
                                reply = reply.Replace("$RANDNUMBER$", randNumbers[i] + "+$RANDNUMBER$");
                                sum += randNumbers[i];
                            }
                            reply = reply.Replace("+$RANDNUMBER$", "");
                            reply = reply.Replace("$SUM$", sum.ToString());
                            reply = reply.Replace("$ADD$", inputNumbers[2].ToString());
                            reply = reply.Replace("$RESULT$", (sum + inputNumbers[2]).ToString());

                            await ReplyManager.send_Async(message, reply);
                        }
                        #endregion
                    }
                }
            }
            catch (Exception)
            {
                await ReplyManager.send_Async(message, "Exception in _w_Async... Versuche es bitte erneut mit anderen Inputs...");
            }
        }

        private async Task showDisabledChannels_Async(SocketUserMessage message)
        {
            try
            {
                string reply = "";
                foreach (var disbledChannel in Helper.disabledChannels)
                {
                    reply += "- " + disbledChannel.Value + "\n";
                }
                await ReplyManager.send_Async(message, "Deaktivierte Channel", reply);
            }
            catch (Exception)
            {
                await ReplyManager.send_Async(message, "Exception in showDisabledChannels_Async... Versuche es bitte erneut mit anderen Inputs...");
            }
        }

        private async Task disableChannel_Async(SocketUserMessage message)
        {
            try 
            {
                if (Helper.isUserPermitted(message.Author.Id.ToString()))
                {
                    if (Helper.disabledChannels.ContainsKey(message.Channel.Id.ToString()))
                    {
                        await ReplyManager.send_Async(message, "", "Channel ist bereits inaktiv...");
                    }
                    else
                    {
                        Helper.disabledChannels.Add(message.Channel.Id.ToString(), message.Channel.Name);
                        await ReplyManager.send_Async(message, "", "Channel wurde deaktivert...");
                    }
                } else
                {
                    await ReplyManager.send_Async(message, "Unauthorized", "Sie verfügen leider nicht über die benötigten Berechtigungen...");
                }
            }
            catch (Exception)
            {
                await ReplyManager.send_Async(message, "Exception in disableChannel_Async... Versuche es bitte erneut mit anderen Inputs...");
            }
        }

        private async Task enableChannel_Async(SocketUserMessage message)
        {
            try 
            {
                if (Helper.isUserPermitted(message.Author.Id.ToString()))
                {
                    if (Helper.disabledChannels.ContainsKey(message.Channel.Id.ToString()))
                    {
                        Helper.disabledChannels.Remove(message.Channel.Id.ToString());
                        await ReplyManager.send_Async(message, "", "Channel wurde aktivert...");
                    }
                    else
                    {
                        await ReplyManager.send_Async(message, "", "Channel wurde aktiv...");
                    }
                }
                else
                {
                    await ReplyManager.send_Async(message, "Unauthorized", "Sie verfügen leider nicht über die benötigten Berechtigungen...");
                }
            }
            catch (Exception)
            {
                await ReplyManager.send_Async(message, "Exception in enableChannel_Async... Versuche es bitte erneut mit anderen Inputs...");
            }
        }

        private async Task test_Async(SocketUserMessage message)
        {
            try
            {
                if (Helper.isChannelEnabled(message.Channel.Id.ToString()))
                {
                    if (message.Author.Id.ToString() == Helper.ownerID)
                    {
                        await message.Channel.SendMessageAsync("Hey BotOwner");
                    }
                    else
                    {
                        await message.Channel.SendMessageAsync("funktioniert nicht...");
                    }
                    Embed embd = new EmbedBuilder()
                        .WithAuthor(message.Author)
                        .WithColor(Color.Blue)
                        .WithImageUrl("https://vignette.wikia.nocookie.net/gods-school/images/0/07/ErisProfile.png/revision/latest?cb=20190703212021")
                        .WithTitle("würfelte einen w3+3")
                        .WithDescription("Ergebnis: w3 + 3(2)\nSumme: (2 + 3) = 5")
                        .Build();

                    await message.Channel.SendMessageAsync("", false, embd);
                }
            }
            catch (Exception)
            {
                await ReplyManager.send_Async(message, "Exception in test_Async... Versuche es bitte erneut mit anderen Inputs...");
            }
        }
        #endregion
    }
}
