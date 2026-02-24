using Dewit.CLI.Utils;
using Dewit.Core.Entities;
using Spectre.Console;
using Spectre.Console.Testing;

namespace Dewit.CLI.Tests.Snapshots;

public class OutputSnapshotTests
{
    private static readonly string[] Columns =
    [
        "ID",
        "Task",
        "Description",
        "Status",
        "Tags",
        "Added On",
        "Completed On",
        "Recur",
    ];

    private static string Render(Action<IAnsiConsole> renderAction)
    {
        var console = new TestConsole();
        console.Profile.Width = 120;
        renderAction(console);
        return console.Output;
    }

    private static TaskItem[] SampleTasks() =>
        [
            new()
            {
                Title = "Implement login screen",
                Status = "Doing",
                Tags = "feature,auth",
                AddedOn = new DateTime(2024, 6, 3, 9, 0, 0),
                Description = "See Figma designs at https://figma.com/file/abc and follow the auth spec doc for implementation details.",
            },
            new()
            {
                Title = "Fix signup validation bug",
                Status = "Done",
                Tags = "bug",
                AddedOn = new DateTime(2024, 6, 1, 10, 30, 0),
                CompletedOn = new DateTime(2024, 6, 2, 14, 0, 0),
            },
            new()
            {
                Title = "Write documentation",
                Status = "Later",
                Tags = "",
                AddedOn = new DateTime(2024, 6, 4, 8, 15, 0),
                RecurringSchedule = new RecurringSchedule { FrequencyType = "weekly", Interval = 1 },
            },
        ];

    [Test]
    public Task WriteTable_WithTasks() =>
        Verify(Render(c => Output.WriteTable(c, Columns, SampleTasks())));

    [Test]
    public Task WriteTable_Empty() => Verify(Render(c => Output.WriteTable(c, Columns, [])));
}