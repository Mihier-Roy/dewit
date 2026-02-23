namespace Dewit.Core.Enums
{
    public enum Mood
    {
        VeryHappy,
        Happy,
        Meh,
        Down,
        ExtraDown,
    }

    public static class MoodExtensions
    {
        public static string ToDisplayName(this Mood mood) =>
            mood switch
            {
                Mood.VeryHappy => "Very Happy",
                Mood.Happy => "Happy",
                Mood.Meh => "Meh",
                Mood.Down => "Down",
                Mood.ExtraDown => "Extra Down",
                _ => mood.ToString(),
            };

        public static string ToSpectreColor(this Mood mood) =>
            mood switch
            {
                Mood.VeryHappy => "green",
                Mood.Happy => "chartreuse2",
                Mood.Meh => "yellow",
                Mood.Down => "darkorange",
                Mood.ExtraDown => "red",
                _ => "white",
            };

        public static string ToConfigKey(this Mood mood) =>
            mood switch
            {
                Mood.VeryHappy => "mood.descriptors.veryhappy",
                Mood.Happy => "mood.descriptors.happy",
                Mood.Meh => "mood.descriptors.meh",
                Mood.Down => "mood.descriptors.down",
                Mood.ExtraDown => "mood.descriptors.extradown",
                _ => throw new ArgumentOutOfRangeException(nameof(mood)),
            };

        public static bool TryParse(string value, out Mood mood)
        {
            mood = value.ToLowerInvariant().Replace(" ", "") switch
            {
                "veryhappy" or "very happy" => Mood.VeryHappy,
                "happy" => Mood.Happy,
                "meh" => Mood.Meh,
                "down" => Mood.Down,
                "extradown" or "extra down" => Mood.ExtraDown,
                _ => (Mood)(-1),
            };
            return (int)mood >= 0;
        }
    }
}