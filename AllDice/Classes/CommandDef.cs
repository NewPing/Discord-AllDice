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
        public string description;
        public string example;
        public Func<SocketUserMessage, Task> function;

        public CommandDef(int _index, string _matchPattern, string _description, string _example, Func<SocketUserMessage, Task> _function)
        {
            index = _index;
            matchPattern = new Regex(_matchPattern);
            description = _description;
            example = _example;
            function = _function;
        }
    }
}
