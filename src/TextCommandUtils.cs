using DSharpPlus.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Astra
{
    public static partial class TextCommandUtils
    {
        public static string GetDisplayName(this DiscordUser user)
        {
            if (user is DiscordMember member)
            {
                return member.DisplayName;
            }
            else if (!string.IsNullOrEmpty(user.GlobalName))
            {
                return user.GlobalName;
            }
            else if (user.Discriminator == "0")
            {
                return user.Username;
            }

            return $"{user.Username}#{user.Discriminator}";
        }

        public static double NextDouble(
        this Random random,
        double minValue,
        double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }

        public static T GetRandomEnumValue<T>() where T : Enum
        {
            Random random = new();
            T[] values = (T[])Enum.GetValues(typeof(T));
            return values[random.Next(values.Length)];
        }

        public static string TimeSpanToString(TimeSpan timeSpan)
        {
            if (timeSpan.Days > 0) { return $"{timeSpan.Days}:{timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds:D2}"; }
            if (timeSpan.Hours > 0) { return $"{timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds:D2}"; }
            else { return $"{timeSpan.Minutes}:{timeSpan.Seconds:D2}"; }
        }

        public static string AbbreviateLargeNumbers(ulong value)
        {
            int mag = (int)(Math.Floor(Math.Log10(value)) / 3); // Truncates to 6, divides to 2
            double divisor = Math.Pow(10, mag * 3);

            double shortNumber = value / divisor;

            string suffix = "";
            switch (mag)
            {
                case 1:
                    suffix = "k";
                    break;
                case 2:
                    suffix = "m";
                    break;
                case 3:
                    suffix = "b";
                    break;
                case 4:
                    suffix = "t";
                    break;
            }

            return Math.Round(shortNumber, 2).ToString() + suffix;
        }
    }
}
