using DSharpPlus.Entities;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace Astra
{
    public static partial class AstraUtilities
    {
        [GeneratedRegex(@"([A-Za-z]+)(\d+)([a-zA-Z])", RegexOptions.Compiled)] private static partial Regex _planetNameRegex();

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

        public static string DisplayPlanetName(this string planetName)
        {
            Match match = _planetNameRegex().Matches(planetName).First();

            string concatenatedMatches = string.Join(" ",
                Enumerable.Range(1, match.Groups.Count - 1)
                          .Select(i => match.Groups[i].Value)
            );

            return concatenatedMatches;
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

        public static string Humanize(this ulong value)
        {
            int magnitude = (int)(Math.Floor(Math.Log10(value)) / 3); // Truncates to 6, divides to 2
            double divisor = Math.Pow(10, magnitude * 3);

            double shortNumber = value / divisor;

            string suffix = "";
            switch (magnitude)
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

            return Math.Round(shortNumber, 3).ToString() + suffix;
        }
    }
}
