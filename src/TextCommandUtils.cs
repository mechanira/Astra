using DSharpPlus.Entities;

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
            if (timeSpan.Hours > 23) { return $"{timeSpan.Days}:{timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds}"; }
            if (timeSpan.Minutes > 59) { return $"{timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds}"; }
            else { return $"{timeSpan.Minutes}:{timeSpan.Seconds}"; }
        }
    }
}
