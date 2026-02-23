using System.Text.RegularExpressions;
using Dewit.Core.Entities;

namespace Dewit.Core.Utils
{
    public static class RecurParser
    {
        private static readonly Regex ShorthandPattern = new(
            @"^(\d+)([dwmy])$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        /// <summary>
        /// Parse a recur string into a RecurringSchedule.
        /// Valid inputs: daily, weekly, monthly, yearly (case-insensitive),
        ///               Nd (every N days), Nw (every N weeks),
        ///               Nm (every N months), Ny (every N years).
        /// Throws ArgumentException on invalid input.
        /// </summary>
        public static RecurringSchedule Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException(
                    "Recurrence string cannot be empty.",
                    nameof(input)
                );

            var normalized = input.Trim().ToLowerInvariant();

            return normalized switch
            {
                "daily" => new RecurringSchedule { FrequencyType = "daily", Interval = 1 },
                "weekly" => new RecurringSchedule { FrequencyType = "weekly", Interval = 1 },
                "monthly" => new RecurringSchedule { FrequencyType = "monthly", Interval = 1 },
                "yearly" => new RecurringSchedule { FrequencyType = "yearly", Interval = 1 },
                _ => ParseShorthand(normalized, input),
            };
        }

        private static RecurringSchedule ParseShorthand(string normalized, string original)
        {
            var match = ShorthandPattern.Match(normalized);
            if (!match.Success)
                throw new ArgumentException(
                    $"Cannot parse recurrence '{original}'. "
                        + "Accepted: daily, weekly, monthly, yearly, "
                        + "Nd (N days), Nw (N weeks), Nm (N months), Ny (N years).",
                    nameof(original)
                );

            var n = int.Parse(match.Groups[1].Value);
            if (n <= 0)
                throw new ArgumentException(
                    $"Recurrence interval must be a positive integer, got '{n}'.",
                    nameof(original)
                );

            var frequencyType = match.Groups[2].Value switch
            {
                "d" => "daily",
                "w" => "weekly",
                "m" => "monthly",
                "y" => "yearly",
                _ => throw new ArgumentException($"Unknown unit '{match.Groups[2].Value}'."),
            };

            return new RecurringSchedule { FrequencyType = frequencyType, Interval = n };
        }

        /// <summary>
        /// Try-parse variant. Returns false and sets schedule to null on failure.
        /// </summary>
        public static bool TryParse(string input, out RecurringSchedule? schedule)
        {
            try
            {
                schedule = Parse(input);
                return true;
            }
            catch (ArgumentException)
            {
                schedule = null;
                return false;
            }
        }
    }
}