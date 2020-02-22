using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace AllDice.Classes
{
    public static class Helper
    {
        public static Random random = new Random();
        public static string blanc_w_Output = "Ergebnis: $INPUTMESSAGE$ ($RANDNUMBER$) Summe: ( $SUM$+$ADD$ ) = $RESULT$";

        #region methods
        public static int getRandomNumber(int endValue)
        {
            int result = random.Next(1, endValue + 1);

            return result;
        }

        public static int getRandomNumber(int startValue, int endValue)
        {
            int result = random.Next(startValue, endValue + 1);

            return result;
        }

        internal static Color getUserColor(SocketUser author)
        {
            if (getRandomNumber(2) == 1)
            {
                return Color.Purple;
            } else
            {
                return Color.Green;
            }
        }
        #endregion
    }

    public class ReplyManager
    {
        public SocketMessage message;
        public string inputMessage;
        public SocketUser author;
        public Color color;
        

        public ReplyManager(SocketMessage _message)
        {
            message = _message;
            author = message.Author;
            color = Helper.getUserColor(author);
            inputMessage = message.Content.ToString().Split("!")[1];
        }

        public async Task<Error> send_Async(string replyText)
        {
            Error retObj = new Error();
            Embed embed = new EmbedBuilder()
                .WithAuthor(message.Author)
                .WithColor(color)
                .WithTitle("würfelte einen " + inputMessage)
                .WithDescription(replyText)
                .Build();

            await message.Channel.SendMessageAsync("", false, embed);

            retObj.success = true;
            return retObj;
        }
    }
}
