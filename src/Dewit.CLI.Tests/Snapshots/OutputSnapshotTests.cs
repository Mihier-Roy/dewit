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
        "Status",
        "Tags",
        "Added On",
        "Completed On",
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
                TaskDescription = "Implement login screen",
                Status = "Doing",
                Tags = "feature,auth",
                AddedOn = new DateTime(2024, 6, 3, 9, 0, 0),
            },
            new()
            {
                TaskDescription = "Fix signup validation bug",
                Status = "Done",
                Tags = "bug",
                AddedOn = new DateTime(2024, 6, 1, 10, 30, 0),
                CompletedOn = new DateTime(2024, 6, 2, 14, 0, 0),
            },
            new()
            {
                TaskDescription = "Write documentation",
                Status = "Later",
                Tags = "",
                AddedOn = new DateTime(2024, 6, 4, 8, 15, 0),
            },
        ];

    [Test]
    public Task WriteTable_WithTasks() =>
        Verify(Render(c => Output.WriteTable(c, Columns, SampleTasks())));

    [Test]
    public Task WriteTable_Empty() => Verify(Render(c => Output.WriteTable(c, Columns, [])));
}
