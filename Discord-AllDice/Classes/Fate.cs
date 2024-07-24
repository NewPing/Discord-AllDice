using Discord_AllDice.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Discord_AllDice.Classes
{
    public class Fate
    {
        #region Fields

        private static string _abilityHighName = "Hyperdimensional";
        private static string _abilityLowName = "Verheerend";
        private static string _outcomeHighName = "Magischer Erfolg";

        #endregion

        #region Properties

        public static string AbilityHighName
        {
            get => _abilityHighName;

            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _abilityHighName = value;
                }
                else
                {
                    throw new ArgumentException("Must not be null or white space", "nameof(value)");
                }
            }
        }
        public static string AbilityLowName
        {
            get => _abilityLowName;

            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _abilityLowName = value;
                }
                else
                {
                    throw new ArgumentException("Must not be null or white space", "nameof(value)");
                }
            }
        }
        public static string OutcomeHighName
        {
            get => "♾️" + _outcomeHighName;

            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _outcomeHighName = value;
                }
                else
                {
                    throw new ArgumentException("Must not be null or white space", "nameof(value)");
                }
            }
        }

        #endregion

        #region GetNameMethods

        private static string GetAbilityName(int i)
        {
            switch (i)
            {
                case 8:
                    return "Legendär";
                case 7:
                    return "Episch";
                case 6:
                    return "Fantastisch";
                case 5:
                    return "Hervorragend";
                case 4:
                    return "Großartig";
                case 3:
                    return "Gut";
                case 2:
                    return "Ordentlich";
                case 1:
                    return "Durchschnittlich";
                case 0:
                    return "Mäßig";
                case -1:
                    return "Schwach";
                case -2:
                    return "Fürchterlich";
            }

            if (i > 8)
                return AbilityHighName;
            else if (i < -2)
                return AbilityLowName;

            return "Unbeschreibar";
        }

        private static string GetOutcomeName(int i)
        {
            switch (i)
            {
                case 4:
                    return "⏫ Voller Erfolg";
                case 3:
                    return "🔼 Erfolg";
                case 2:
                    return "🔼 Erfolg";
                case 1:
                    return "⏸️ Gleichstand";
                case 0:
                    return "🔽 Fehlschlag oder Erfolg mit Haken";
            }

            if (i > 4)
                return OutcomeHighName;

            return "⏬ Fehlschlag";
        }

        #endregion

        #region ChecksMethods

        public static void PassivCheck(int skill = 0, int mod = 0, int goal = 0)
        {
            var roll = new FateRoll();

            int ability = roll.Result + skill + mod;
            int outcome = ability - goal;

            string modString = mod >= 0 ? "+" + mod.ToString() : mod.ToString();

            string output = $"würfelt einen Fate wurf. Fertigkeit ist {skill}{modString}, passiver Widerstand ist eine {goal}.\n\n" +
                $"Wurf: {roll.GetEmojis()} = {roll.Result}\n" +
                $"Rechnung: {roll.Result}+{skill}{modString} = {ability} ({GetAbilityName(ability)})\n" +
                $"Ergebniss: {outcome} ({GetOutcomeName(outcome)})";

            //Todo: Call back and return output

        }

        public static void ActiveCheck(int skill = 0, int mod = 0, int skillOpponent = 0, int modOpponent = 0)
        {
            var roll = new FateRoll();
            int ability = roll.Result + skill + mod;
            int outcome = ability - roll.Result;

            var rollOpponent = new FateRoll();
            int abilityOpponent = rollOpponent.Result + skillOpponent + modOpponent;
            int outcomeOpponent = abilityOpponent - rollOpponent.Result;

            string modString = mod >= 0 ? "+" + mod.ToString() : mod.ToString();
            string modStringOpponent = modOpponent >= 0 ? "+" + modOpponent.ToString() : modOpponent.ToString();


            string output = $"würfelt einen Fate wurf. Fertigkeit ist {skill}{modString}, aktiver Widerstand ist Fertigkeit {skillOpponent}{modStringOpponent}.\n\n" +
                $"Wurf Spieler: {roll.GetEmojis()} = {roll.Result}\n" +
                $"Rechnung: {roll.Result}+{skill}{modString} = {ability} ({GetAbilityName(ability)})\n" +
                $"Wurf Widerstand: {rollOpponent.GetEmojis()} = {rollOpponent.Result}\n" +
                $"Rechnung: {rollOpponent.Result}+{skillOpponent}{modStringOpponent} = {abilityOpponent} ({GetAbilityName(abilityOpponent)})\n" +
                $"Ergebniss: {outcome} ({GetOutcomeName(outcome)})";

            //Todo: Call back and return output

        }



        #endregion

        #region FateRollClass

        private class FateRoll
        {
            public readonly int[] rollNumbers = new int[4];
            public int Result
            {
                get
                {
                    return rollNumbers[0] + rollNumbers[1] + rollNumbers[2] + rollNumbers[3];
                }
            }

            public FateRoll()
            {
                for (int i = 0; i < rollNumbers.Length; i++)
                {
                    rollNumbers[i] = Helper.getRandomNumber(-1, 1);
                }
            }

            public string GetEmojis()
            {
                string output = "";

                foreach (var curentRollNumber in rollNumbers)
                {
                    switch (curentRollNumber)
                    {
                        case 1:
                            output += "🔼";
                            break;
                        case 0:
                            output += "⏺️";
                            break;
                        case -1:
                            output += "🔽";
                            break;
                    }
                }
                return output;
            }
        }

        #endregion
    }
}
