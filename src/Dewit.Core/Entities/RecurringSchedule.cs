using System.ComponentModel.DataAnnotations;

namespace Dewit.Core.Entities
{
    public class RecurringSchedule : EntityBase
    {
        [Required, MaxLength(16)]
        public string FrequencyType { get; set; } = string.Empty;

        public int Interval { get; set; } = 1;

        public DateTime ComputeNextDueDate(DateTime baseDate)
        {
            return FrequencyType switch
            {
                "daily" => baseDate.Date.AddDays(Interval),
                "weekly" => baseDate.Date.AddDays(Interval * 7),
                "monthly" => baseDate.Date.AddMonths(Interval),
                "yearly" => baseDate.Date.AddMonths(Interval * 12),
                _ => throw new InvalidOperationException(
                    $"Unknown FrequencyType '{FrequencyType}'"
                ),
            };
        }

        public string ToLabel() =>
            (FrequencyType, Interval) switch
            {
                ("daily", 1) => "Daily",
                ("weekly", 1) => "Weekly",
                ("monthly", 1) => "Monthly",
                ("yearly", 1) => "Yearly",
                ("daily", var n) => $"Every {n} Days",
                ("weekly", var n) => $"Every {n} Weeks",
                ("monthly", var n) => $"Every {n} Months",
                ("yearly", var n) => $"Every {n} Years",
                _ => FrequencyType,
            };
    }
}