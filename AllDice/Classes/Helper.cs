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
        public static int commandIndexCounter = 1;
        public static int swPass = 4;

        #region blanc_outputs
        public static string blanc_w_Output =
            "Rechnung: $RANDNUMBER$\nSumme: $SUM$+$ADD$\nErgebnis = $RESULT$";
        public static string blanc_sww_Output =
            "**Probewurf W$INPUTNUMBER$**\nRechnung: $RANDNUMBERS0$\nSumme: $SUM0$+$ADD$ = $RESULT0$\n$OUTPUT0$\n\n" +
            "**Wildcardwurf: W6**\nRechnung: $RANDNUMBERS1$\nSumme: $SUM1$+$ADD$ = $RESULT1$\n$OUTPUT1$";
        public static string blanc_sws_Output =
            "**Probewurf W$INPUTNUMBER$**\nRechnung: $RANDNUMBERS0$\nSumme: $SUM0$+$ADD$ = $RESULT0$\n$OUTPUT0$";

        #endregion

        #region functions
        public static int getRandomNumber(int endValue)
        {
            int result = random.Next(1, endValue + 1);

            return result;
        }

        public static Tuple<int, string> getExplodingDice(int diceSides)
        {
            int resNumber = 0;
            string resStr = "";
            int tmpNumber = 0;

            do
            {
                tmpNumber = getRandomNumber(diceSides);
                resNumber += tmpNumber;
                resStr += tmpNumber + "+";
            } while (tmpNumber == diceSides);

            resStr = resStr.Substring(0, resStr.Length -1);
            return new Tuple<int, string>(resNumber, resStr);
        }

        public static string getSWResultOutput(int number)
        {
            if (number < swPass)
            {
                return "Fehlschlag!";
            } else if (number < swPass * 2)
            {
                return "Erfolg!";
            } else
            {
                return "Erfolg - Steigerung um **" + (Convert.ToInt32(number / swPass) -1) + "**!";
            }
        }

        public static Color getUserColor(SocketUser author)
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
        #endregion
    }
}
