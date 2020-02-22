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
        public Commands()
        {
            commands_def.Add(new CommandDef(
                0,
                @"^!help(:?( [0-9]+)?)$", 
                "!help ([index]): Gibt die Hilfeseite aus.",
                "!help 1",
                help_Async)); //help [command]
            commands_def.Add(new CommandDef(
                1,
                @"^!((?:[0-9])?)+w[0-9]+((:?((:?\+)?|(:?\-)?)[0-9]+)?)$",
                "!([zahl])w[zahl](+/-[zahl]) : Würfelt einen virtuellen Würfel mit der angegebenen Augenzahl.",
                "!2w6+1",
                w_Async)); //[zahl]w[zahl]+/-[zahl]
            commands_def.Add(new CommandDef(
                2,
                @"^!test$",
                "-",
                "-",
                test_Async));
        }

        public async Task<bool> handleCommandInput(SocketUserMessage message)
        {
            bool commandFound = false;
            Error commandExec = new Error();
            Regex foundRegex = new Regex("-");
            
            foreach (CommandDef command in commands_def) //mögliche Commands durchlaufen
            {
                if (command.matchPattern.IsMatch(message.Content))
                {
                    commandFound = true;
                    foundRegex = command.matchPattern;
                    commandExec = await command.function(message);
                }
            }
            
            if (commandFound && commandExec.success)
            {
                return true;
            } else
            {
                if (!commandExec.success)
                {
                    await message.Channel.SendMessageAsync(commandExec.description);
                }
                return false;
            }
        }
        #endregion

        #region command_functions
        private async Task<Error> w_Async(SocketUserMessage message)
        {
            Error retObj = new Error();
            try
            {
                ReplyManager replyManager = new ReplyManager(message);
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
                        retObj.success = false;
                        retObj.description = "❗Syntax Error : Eingabe muss größer als 0 sein!";
                        return retObj;
                    }
                    else
                    {
                        randNumbers[0] = Helper.getRandomNumber(inputNumbers[0]);
                        //Output zusammenbauen
                        blancOutput = blancOutput.Replace("+", "");
                        reply += blancOutput;
                        reply = reply.Replace("$INPUTMESSAGE$", replyManager.inputMessage);
                        reply = reply.Replace("$RANDNUMBER$", randNumbers[0].ToString());
                        sum = randNumbers[0];
                        reply = reply.Replace("$SUM$", sum.ToString());
                        reply = reply.Replace("$ADD$", "");
                        reply = reply.Replace("$RESULT$", (sum).ToString());

                        await replyManager.send_Async(reply);
                        
                    }
                    #endregion
                }
                else if (inputNumbers.Length == 2)
                {
                    if (message.Content.Contains("+") || message.Content.Contains("-")) {

                        #region Logik_w3+3 
                        if (inputNumbers[0] < 1)
                        {
                            retObj.success = false;
                            retObj.description = "❗Syntax Error : Eingabe muss größer als 0 sein!";
                            return retObj;
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
                            reply = reply.Replace("$INPUTMESSAGE$", replyManager.inputMessage);
                            reply = reply.Replace("$RANDNUMBER$", randNumbers[0].ToString());
                            sum = randNumbers[0];
                            reply = reply.Replace("$SUM$", sum.ToString());
                            reply = reply.Replace("$ADD$", inputNumbers[1].ToString());
                            reply = reply.Replace("$RESULT$", (sum + inputNumbers[1]).ToString());

                            await replyManager.send_Async(reply);
                        }
                        #endregion
                    } else
                    {
                        #region Logik_3w3
                        if (inputNumbers[0] < 1 || inputNumbers[1] < 1)
                        {
                            retObj.success = false;
                            retObj.description = "❗Syntax Error : Eingabe muss größer als 0 sein!";
                            return retObj;
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
                            reply = reply.Replace("$INPUTMESSAGE$", replyManager.inputMessage);
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

                            await replyManager.send_Async(reply);
                        }
                        #endregion
                    }
                } else if (inputNumbers.Length == 3 || inputNumbers[1] < 1) //3w3+3 OR 3w3-3
                {
                    #region Logik_3w3+3
                    if (inputNumbers[0] < 1)
                    {
                        retObj.success = false;
                        retObj.description = "❗Syntax Error : Eingabe muss größer als 0 sein!";
                        return retObj;
                    }
                    else
                    {
                        if (message.Content.Contains("-"))
                        {
                            inputNumbers[2] = 0-inputNumbers[2];  //letzte zahl umkehren
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
                        reply = reply.Replace("$INPUTMESSAGE$", replyManager.inputMessage);
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

                        await replyManager.send_Async(reply);
                    }
                    #endregion
                }
                retObj.success = true;
                return retObj;
            }
            catch (Exception)
            {
                await message.Channel.SendMessageAsync("Exception in _w_Async... Try again with other inputs...");
                return retObj;
            }

        }

        private async Task<Error> help_Async(SocketUserMessage message)
        {
            Error retObj = new Error();
            try
            {
                string reply = "";
                MatchCollection values = new Regex(@"\d+").Matches(message.Content);
                
                if (values.Count == 0) //ausgabe mögliche befehle (!help)
                {
                    reply += "❓ - Hilfeseite : Mögliche Befehle - ❓\n";

                    foreach (CommandDef command in commands_def)
                    {
                        reply += command.index + " - Index\n";
                        reply += "- " + command.description + "\n";
                    }

                    await message.Channel.SendMessageAsync(reply);

                    retObj.success = true;
                } else //ausgabe hilfe für befehl (!help [zahl])
                {
                    int inputNumber = Int32.Parse(values[0].ToString());
                    reply += "❓ - Hilfeseite : Command - ❓\n";

                    foreach(CommandDef command in commands_def)
                    {
                        if (inputNumber == command.index)
                        {
                            reply += command.description + "\n";
                            reply += "- Beispiel: " + command.example;
                        }
                    }

                    await message.Channel.SendMessageAsync(reply);

                    retObj.success = true;
                }
                return retObj;
            } catch (Exception ex)
            {
                await message.Channel.SendMessageAsync("Exception in help_Async\n" + ex.ToString());
                return retObj;
            }
        }


        private async Task<Error> test_Async(SocketUserMessage message)
        { 
            Embed embd = new EmbedBuilder()
                .WithAuthor(message.Author)
                .WithColor(Color.Blue)
                .WithImageUrl("https://vignette.wikia.nocookie.net/gods-school/images/0/07/ErisProfile.png/revision/latest?cb=20190703212021")
                .WithTitle("würfelte einen w3+3")
                .WithDescription("Ergebnis: w3 + 3(2)\nSumme: (2 + 3) = 5")
                .Build();

            await message.Channel.SendMessageAsync("", false, embd);

            Error retObj = new Error();
            retObj.success = true;
            return retObj;
        }
        #endregion
    }
}
