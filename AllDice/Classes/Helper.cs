using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllDice.Classes
{
    public static class Helper
    {
        public static int maxReplyLength = 1000;
        public static string ownerID;
        public static Random random = new Random();
        public static Dictionary<string, string> disabledChannels = new Dictionary<string, string>();
        public static Dictionary<string, Tuple<Func<SocketUserMessage, Task>, SocketUserMessage>> lastSendMsgsAndFuncs = new Dictionary<string, Tuple<Func<SocketUserMessage, Task>, SocketUserMessage>>();
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
        public static string blanc_swd_Output =
            "**Schadenswurf W$INPUTNUMBER0$**\nRechnung: $RANDNUMBERS0$\nSumme: $SUM0$\n\n" +
            "**Schadenswurf W$INPUTNUMBER1$**\nRechnung: $RANDNUMBERS1$\nSumme: $SUM1$\n\n" +
            "Ergebnis: $SUM0$ + $SUM1$ +$ADD$ = $RESULT$";
        public static string blanc_swh_Output =
            "Trefferzonenwurf W6: $RANDNUMBER0$\n" +
            "Trefferzonenwurf W6: $RANDNUMBER1$\n" +
            "**Ergebnis: $SUM$**\n\n" +
            "Trefferzone: **$ZONE0$**\n" +
            "Zusatzwurf W6: $RANDNUMBER2$ - **$ZONE1$**";

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

        public static void setLastSendMsgAndFunc(string userid, Tuple<Func<SocketUserMessage, Task>, SocketUserMessage> lastSendMsgAndFunc)
        {
            if (lastSendMsgsAndFuncs.ContainsKey(userid))
            {
                lastSendMsgsAndFuncs[userid] = lastSendMsgAndFunc;
            } else
            {
                lastSendMsgsAndFuncs.Add(userid, lastSendMsgAndFunc);
            }
        }

        public static Tuple<Func<SocketUserMessage, Task>, SocketUserMessage> getLastSendMsgAndFunc(string userid)
        {
            if (lastSendMsgsAndFuncs.ContainsKey(userid))
            {
                return lastSendMsgsAndFuncs[userid];
            } else
            {
                throw new Exception();
            }
        }

        public static string[] getSWHZoneOutput(int number, int number2)
        {
            string[] ret = new string[2];
            ret[0] = "";
            ret[1] = "";
            if (number == 2)
            {
                ret[0] = "Geschlechtsteile";
            } else if (number < 5)
            {
                if (getRandomNumber(2) == 1)
                {
                    ret[0] = "Linker Arm";
                } else
                {
                    ret[0] = "Rechter Arm";
                }
            } else if (number < 10)
            {
                ret[0] = "Eingeweide";
                if (number2 < 3)
                {
                    ret[1] = "Gebrochen";
                }
                else if (number2 < 5)
                {
                    ret[1] = "Zerschmettert";
                }
                else
                {
                    ret[1] = "Ruiniert";
                }
            } else if (number == 10)
            {
                if (getRandomNumber(2) == 1)
                {
                    ret[0] = "Linkes Bein";
                }
                else
                {
                    ret[0] = "Rechtes Bein";
                }
            } else
            {
                ret[0] = "Kopf";
                if (number2 < 3)
                {
                    ret[1] = "Scheußliche Narbe";
                }
                else if (number2 < 5)
                {
                    ret[1] = "Geblendet";
                }
                else
                {
                    ret[1] = "Gehirnerschütterung";
                }
            }
            return ret;
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
