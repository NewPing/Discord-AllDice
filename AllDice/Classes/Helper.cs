using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllDice.Classes
{
    public static class Helper
    {
        public static int maxReplyLength = 1000;
        public static string ownerID;
        public static Random random = new Random();
        public static Dictionary<string, string> disabledChannels = new Dictionary<string, string>();
        public static string blanc_w_Output = "Rechnung: $RANDNUMBER$\nSumme: $SUM$+$ADD$\nErgebnis = $RESULT$";

        public static int getRandomNumber(int endValue)
        {
            int result = random.Next(1, endValue + 1);

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

        public static List<string> splitIntoChunks(string str, int maxChunkSize)
        {
            List<string> chunks = new List<string>();
            for (int i = 0; i < str.Length; i += maxChunkSize)
            {
                chunks.Add(str.Substring(i, Math.Min(maxChunkSize, str.Length - i)));
            }
            return chunks;
        }

        public static bool isChannelEnabled(string channelID)
        {
            return !Helper.disabledChannels.ContainsKey(channelID);
        }

        public static bool isUserPermitted(string userID)
        {
            return ownerID == userID;
        }
    }
}
