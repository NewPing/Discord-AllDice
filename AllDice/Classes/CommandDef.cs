using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AllDice.Classes
{
    public class CommandDef
    {
        public int index;
        public Regex matchPattern;
        public string name;
        public string syntax;
        public string description;
        public string example;
        public Func<SocketUserMessage, Task> function;

        public CommandDef(string _matchPattern, string _name, string _syntax, string _description, string _example, Func<SocketUserMessage, Task> _function)
        {
            index = Helper.commandIndexCounter++;
            matchPattern = new Regex(_matchPattern.ToLower());
            name = _name;
            syntax = _syntax;
            description = _description;
            example = _example;
            function = _function;
        }
    }
}
