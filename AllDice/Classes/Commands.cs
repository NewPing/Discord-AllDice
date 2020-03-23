using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
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
            commands_def.Add(new CommandDef( //Hilfeseite
                @"^!help(:?( [0-9]+)?)$",
                "Help",
                "help (index)",
                "Gibt die Hilfeseite aus",
                "!help 1",
                help_Async)); //help [command]

            commands_def.Add(new CommandDef( //Normaler Wurf
                @"^!((?:[0-9])?)+w[0-9]+((:?((:?\+)?|(:?\-)?)[0-9]+)?)$",
                "Probewurf",
                "(zahl)w(zahl)(+/-zahl)",
                "Würfelt einen virtuellen Würfel mit der angegebenen Augenzahl",
                "!2w6+1",
                w_Async)); //[zahl]w[zahl]+/-[zahl]

            commands_def.Add(new CommandDef( //Savage Worlds Wildcard
                @"^!sww[0-9]+((:?((:?\+)?|(:?\-)?)[0-9]+)?)$",
                "Savage Worlds Wildcard",
                "sww(zahl)(+/-zahl)",
                "Savage Worlds Wildcard-Eigenschaftsprobe",
                "!sww8+1",
                sww_Async)); //sww[zahl]+/-[zahl]

            commands_def.Add(new CommandDef( //Savage Worlds Wildcard
                @"^!sws[0-9]+((:?((:?\+)?|(:?\-)?)[0-9]+)?)$",
                "Savage Worlds Statist",
                "sws(zahl)(+/-zahl)",
                "Savage Worlds Statist Probe",
                "!sws8+1",
                sws_Async)); //sws[zahl]+/-[zahl]

            commands_def.Add(new CommandDef( //Savage Worlds Wildcard
                @"^!swd[0-9]+,[0-9]+(:?(,)?)((:?((:?\+)?|(:?\-)?)[0-9]+)?)$",
                "Savage Worlds Damage",
                "swd(zahl),(zahl),(+/-zahl)",
                "Savage Worlds Damage Probe",
                "!swd5,4,+1",
                swd_Async)); //swd[zahl],[zahl],+/-[zahl]

            commands_def.Add(new CommandDef( //Savage Worlds Wildcard
                @"^!swh$",
                "Savage Worlds Damage-Zone",
                "swd)",
                "Savage Worlds Damage-Zone Probe",
                "!swh",
                swh_Async)); //swh

            commands_def.Add(new CommandDef( //Channel aktivieren
                @"^!enableChannel$",
                "Channel aktivieren",
                "enableChannel",
                "Aktivert einen deaktivierten Channel",
                "!enableChannel",
                enableChannel_Async));

            commands_def.Add(new CommandDef( //Channel deaktiveren
                @"^!disableChannel$",
                "Channel deaktieren",
                "disableChannel",
                "Deakativert einen aktiven Channel (Nachrichten in diesem Channel werden dann vom Bot ignoriert)",
                "!disableChannel",
                disableChannel_Async));

            commands_def.Add(new CommandDef( //Deaktiverte Channel anzeigen
                @"^!showDisabledChannels",
                "Deaktivierte Channel anzeigen",
                "showDisabledChannels",
                "Listet alle deaktiverten Channels auf",
                "!showDisabledChannels",
                showDisabledChannels_Async));

            commands_def.Add(new CommandDef( //Testcommand
                @"^!test$",
                "-",
                "-",
                "-",
                "-",
                test_Async));
        }

        public async Task handleCommandInput(SocketUserMessage message)
        {
            bool commandFound = false;
            
            foreach (CommandDef command in commands_def) //mögliche Commands durchlaufen
            {
                if (command.matchPattern.IsMatch(message.Content.ToLower()))
                {
                    commandFound = true;
                    await command.function(message);
                }
            }
            
            if (commandFound == false)
            {
                if (Helper.isChannelEnabled(message.Channel.Id.ToString()))
                {
                    await ReplyManager.send_Async(message, "Syntax Error", "Kein Befehl mit diesem Syntax gefunden...");
                    await help_Async(message);
                }
            }
        }
        #endregion

        #region command_functions
        #region pnpCommands
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
                await ReplyManager.send_Async(message, "Exception in w_Async... Versuche es bitte erneut mit anderen Inputs...");
            }
        }

        private async Task sww_Async(SocketUserMessage message)
        {
            try
            {
                if (Helper.isChannelEnabled(message.Channel.Id.ToString()))
                {
                    string blancOutput = Helper.blanc_sww_Output;
                    int[] inputNumbers = null;
                    Tuple<int, string> explodingDice0 = null;
                    Tuple<int, string> explodingDice1 = null;
                    string reply = "";

                    MatchCollection values = new Regex(@"\d+").Matches(message.Content);
                    inputNumbers = new int[2];
                    inputNumbers[1] = 0;
                    for (int i = 0; i < values.Count; i++)
                    {
                        inputNumbers[i] = Int32.Parse(values[i].ToString());
                    }

                    if (inputNumbers[0] < 2)
                    {
                        await ReplyManager.send_Async(message, "Syntax Error", "❗Syntax Error : Eingabe muss größer als 1 sein!");
                        return;
                    }
                    else
                    {
                        explodingDice0 = Helper.getExplodingDice(inputNumbers[0]);
                        explodingDice1 = Helper.getExplodingDice(6);
                        //Output zusammenbauen
                        if (message.Content.Contains("-") || message.Content.Contains("+"))
                        {
                            if (message.Content.Contains("-")) //nachricht enthält minus
                            {
                                inputNumbers[1] = -inputNumbers[1];  //letzte zahl umkehren
                                blancOutput = blancOutput.Replace("+", "");
                            }
                        }
                        else
                        {
                            blancOutput = blancOutput.Replace("+$ADD$ = $RESULT0$", "");
                            blancOutput = blancOutput.Replace("+$ADD$ = $RESULT1$", "");
                        }
                        if (!explodingDice0.Item2.Contains("+")) //Addition wird nicht benötigt
                        {
                            blancOutput = blancOutput.Replace("Rechnung: $RANDNUMBERS0$\nSumme", "Ergebnis");
                        }
                        if (!explodingDice1.Item2.Contains("+")) //Addition wird nicht benötigt
                        {
                            blancOutput = blancOutput.Replace("Rechnung: $RANDNUMBERS1$\nSumme", "Ergebnis");
                        }

                        reply += blancOutput;
                        reply = reply.Replace("$INPUTNUMBER$", inputNumbers[0].ToString());
                        reply = reply.Replace("$RANDNUMBERS0$", explodingDice0.Item2);
                        reply = reply.Replace("$RANDNUMBERS1$", explodingDice1.Item2);
                        reply = reply.Replace("$SUM0$", explodingDice0.Item1.ToString());
                        reply = reply.Replace("$SUM1$", explodingDice1.Item1.ToString());
                        reply = reply.Replace("$ADD$", inputNumbers[1].ToString());

                        reply = reply.Replace("$RESULT0$", (explodingDice0.Item1 + inputNumbers[1]).ToString());
                        reply = reply.Replace("$RESULT1$", (explodingDice1.Item1 + inputNumbers[1]).ToString());

                        if (explodingDice0.Item1 == explodingDice1.Item1 && explodingDice0.Item1 == 1)
                        {
                            reply = reply.Replace("$OUTPUT0$", "Kritischer Fehlschlag!");
                            reply = reply.Replace("$OUTPUT1$", "Kritischer Fehlschlag!");
                        } else
                        {
                            reply = reply.Replace("$OUTPUT0$", Helper.getSWResultOutput(explodingDice0.Item1 + inputNumbers[1]));
                            reply = reply.Replace("$OUTPUT1$", Helper.getSWResultOutput(explodingDice1.Item1 + inputNumbers[1]));
                        }

                        await ReplyManager.send_Async(message, reply);
                    }
                }
            }
            catch (Exception)
            {
                await ReplyManager.send_Async(message, "Exception in sww_Async... Versuche es bitte erneut mit anderen Inputs...");
            }
        }

        private async Task sws_Async(SocketUserMessage message)
        {
            try
            {
                if (Helper.isChannelEnabled(message.Channel.Id.ToString()))
                {
                    string blancOutput = Helper.blanc_sws_Output;
                    int[] inputNumbers = null;
                    Tuple<int, string> explodingDice0 = null;
                    string reply = "";

                    MatchCollection values = new Regex(@"\d+").Matches(message.Content);
                    inputNumbers = new int[2];
                    inputNumbers[1] = 0;
                    for (int i = 0; i < values.Count; i++)
                    {
                        inputNumbers[i] = Int32.Parse(values[i].ToString());
                    }

                    if (inputNumbers[0] < 2)
                    {
                        await ReplyManager.send_Async(message, "Syntax Error", "❗Syntax Error : Eingabe muss größer als 1 sein!");
                        return;
                    }
                    else
                    {
                        explodingDice0 = Helper.getExplodingDice(inputNumbers[0]);
                        //Output zusammenbauen
                        if (message.Content.Contains("-") || message.Content.Contains("+"))
                        {
                            if (message.Content.Contains("-")) //nachricht enthält minus
                            {
                                inputNumbers[1] = -inputNumbers[1];  //letzte zahl umkehren
                                blancOutput = blancOutput.Replace("+", "");
                            }
                        }
                        else
                        {
                            blancOutput = blancOutput.Replace("+$ADD$ = $RESULT0$", "");
                        }
                        if (!explodingDice0.Item2.Contains("+")) //Addition wird nicht benötigt
                        {
                            blancOutput = blancOutput.Replace("Rechnung: $RANDNUMBERS0$\nSumme", "Ergebnis");
                        }

                        reply += blancOutput;
                        reply = reply.Replace("$INPUTNUMBER$", inputNumbers[0].ToString());
                        reply = reply.Replace("$RANDNUMBERS0$", explodingDice0.Item2);
                        reply = reply.Replace("$SUM0$", explodingDice0.Item1.ToString());
                        reply = reply.Replace("$ADD$", inputNumbers[1].ToString());

                        reply = reply.Replace("$RESULT0$", (explodingDice0.Item1 + inputNumbers[1]).ToString());

                        if (explodingDice0.Item1 == 1)
                        {
                            reply = reply.Replace("$OUTPUT0$", "Kritischer Fehlschlag!");
                        }
                        else
                        {
                            reply = reply.Replace("$OUTPUT0$", Helper.getSWResultOutput(explodingDice0.Item1 + inputNumbers[1]));
                        }

                        await ReplyManager.send_Async(message, reply);
                    }
                }
            }
            catch (Exception)
            {
                await ReplyManager.send_Async(message, "Exception in sww_Async... Versuche es bitte erneut mit anderen Inputs...");
            }
        }

        private async Task swd_Async(SocketUserMessage message)
        {
            try
            {
                if (Helper.isChannelEnabled(message.Channel.Id.ToString()))
                {
                    string blancOutput = Helper.blanc_swd_Output;
                    int[] inputNumbers = null;
                    Tuple<int, string> explodingDice0 = null;
                    Tuple<int, string> explodingDice1 = null;
                    string reply = "";

                    MatchCollection values = new Regex(@"\d+").Matches(message.Content);
                    inputNumbers = new int[3];
                    inputNumbers[2] = 0;
                    for (int i = 0; i < values.Count; i++)
                    {
                        inputNumbers[i] = Int32.Parse(values[i].ToString());
                    }

                    if (inputNumbers[0] < 2 || inputNumbers[1] < 2)
                    {
                        await ReplyManager.send_Async(message, "Syntax Error", "❗Syntax Error : Eingabe muss größer als 1 sein!");
                        return;
                    }
                    else
                    {
                        explodingDice0 = Helper.getExplodingDice(inputNumbers[0]);
                        explodingDice1 = Helper.getExplodingDice(inputNumbers[1]);
                        //Output zusammenbauen
                        if (message.Content.Count(x => x == ',') == 2)
                        {
                            if (message.Content.Contains("-")) //nachricht enthält minus
                            {
                                inputNumbers[2] = -inputNumbers[2];  //letzte zahl umkehren
                                blancOutput = blancOutput.Replace("+$ADD$", "$ADD$");
                            }
                        }
                        else
                        {
                            blancOutput = blancOutput.Replace("+$ADD$ ", "");
                        }
                        if (!explodingDice0.Item2.Contains("+")) //Addition wird nicht benötigt
                        {
                            blancOutput = blancOutput.Replace("Rechnung: $RANDNUMBERS0$\nSumme", "Ergebnis");
                        }
                        if (!explodingDice1.Item2.Contains("+")) //Addition wird nicht benötigt
                        {
                            blancOutput = blancOutput.Replace("Rechnung: $RANDNUMBERS1$\nSumme", "Ergebnis");
                        }

                        reply += blancOutput;
                        reply = reply.Replace("$INPUTNUMBER0$", inputNumbers[0].ToString());
                        reply = reply.Replace("$INPUTNUMBER1$", inputNumbers[1].ToString());
                        reply = reply.Replace("$RANDNUMBERS0$", explodingDice0.Item2);
                        reply = reply.Replace("$RANDNUMBERS1$", explodingDice1.Item2);
                        reply = reply.Replace("$SUM0$", explodingDice0.Item1.ToString());
                        reply = reply.Replace("$SUM1$", explodingDice1.Item1.ToString());
                        reply = reply.Replace("$ADD$", inputNumbers[2].ToString());

                        reply = reply.Replace("$RESULT$", (explodingDice0.Item1 + explodingDice1.Item1 + inputNumbers[2]).ToString());

                        await ReplyManager.send_Async(message, reply);
                    }
                }
            }
            catch (Exception)
            {
                await ReplyManager.send_Async(message, "Exception in sww_Async... Versuche es bitte erneut mit anderen Inputs...");
            }
        }

        private async Task swh_Async(SocketUserMessage message)
        {
            try
            {
                if (Helper.isChannelEnabled(message.Channel.Id.ToString()))
                {
                    string blancOutput = Helper.blanc_swh_Output;
                    int[] randomNumbers = new int[3];
                    string reply = "";

                    randomNumbers[0] = Helper.getRandomNumber(6);
                    randomNumbers[1] = Helper.getRandomNumber(6);
                    randomNumbers[2] = Helper.getRandomNumber(6);

                    string[] zones = Helper.getSWHZoneOutput(randomNumbers[0] + randomNumbers[1], randomNumbers[2]);
                    if (String.IsNullOrWhiteSpace(zones[1]))
                    {
                        blancOutput = blancOutput.Replace("\nZusatzwurf W6: $RANDOMNUMBER2$ - **$ZONE1$**", "");
                    }

                    //Output zusammenbauen
                    reply += blancOutput;
                    reply = reply.Replace("$RANDNUMBER0$", randomNumbers[0].ToString());
                    reply = reply.Replace("$RANDNUMBER1$", randomNumbers[1].ToString());
                    reply = reply.Replace("$RANDNUMBER2$", randomNumbers[2].ToString());
                    reply = reply.Replace("$SUM$", (randomNumbers[0] + randomNumbers[1]).ToString());

                    reply = reply.Replace("$ZONE0$", zones[0]);
                    reply = reply.Replace("$ZONE1$", zones[1]);

                    await ReplyManager.send_Async(message, reply);

                }
            }
            catch (Exception)
            {
                await ReplyManager.send_Async(message, "Exception in sww_Async... Versuche es bitte erneut mit anderen Inputs...");
            }
        }
        #endregion

        #region additionalCommands
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
                            reply += "**" + command.index + " - " + command.name + "**" + " : " + command.syntax + "\n\n";
                        }

                        await ReplyManager.send_Async(message, "❓ - Hilfeseite : Mögliche Befehle - ❓", reply, false);
                    }
                    else //ausgabe hilfe für befehl (!help [zahl])
                    {
                        bool commandFound = false;
                        int inputNumber = Int32.Parse(values[0].ToString());

                        string tmpCommandName = "";
                        foreach (CommandDef command in commands_def)
                        {
                            if (inputNumber == command.index)
                            {
                                tmpCommandName = command.name;
                                reply += "**" + command.syntax + "**\n";
                                reply += command.description + "\n\n";
                                reply += "Beispiel: " + command.example;
                                commandFound = true;
                            }
                        }

                        if (commandFound)
                        {
                            await ReplyManager.send_Async(message, "❓ - Hilfeseite : " + tmpCommandName + " - ❓", reply, false);
                        } else
                        {
                            await ReplyManager.send_Async(message, "❓ - Hilfeseite : NoCommand - ❓", "Es wurde kein Command mit dem angegebenen Index gefunden...\nBitte versuchen Sie es erneut!", false);
                        }
                        
                    }
                }
            }
            catch (Exception)
            {
                await ReplyManager.send_Async(message, "Exception in help_Async... Versuche es bitte erneut mit anderen Inputs...");
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
        #endregion
    }
}
