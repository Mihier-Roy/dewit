# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

Also refer to [AGENTS.md](./AGENTS.md)

## Commands

```bash
# Build
dotnet build src/Dewit.sln

# Run (migrations apply automatically on first run)
dotnet run --project src/Dewit.CLI -- now "My task" --tags work

# Run all tests
dotnet test src/Dewit.sln

# Run a single test class
dotnet test src/Dewit.CLI.Tests/Dewit.CLI.Tests.csproj --filter "FullyQualifiedName~TaskServiceTests"

# Check formatting (must pass in CI)
dotnet format whitespace src/Dewit.sln --verify-no-changes

# Fix formatting
dotnet format src/Dewit.sln

# Add a migration (run from repo root)
dotnet ef migrations add MigrationName --project src/Dewit.Data --startup-project src/Dewit.CLI
```

### Snapshot Tests

TUI output is snapshot-tested with Verify. When a snapshot test fails, a `*.received.txt` file is written next to the `*.verified.txt` in `src/Dewit.CLI.Tests/Snapshots/`. If the change is intentional, accept it:

```bash
# Accept all pending snapshots
for f in src/Dewit.CLI.Tests/Snapshots/*.received.txt; do
  mv "$f" "${f/.received./.verified.}"
done
```

Never commit `*.received.txt` files (they are git-ignored).

## Architecture

Three-layer architecture: **CLI → Core → Data**

- **`Dewit.Core`** — entities, service interfaces, service implementations, and utilities. No dependencies on CLI or Data layers.
- **`Dewit.Data`** — EF Core DbContext, generic `Repository<T>`, and migrations. Depends only on Core.
- **`Dewit.CLI`** — System.CommandLine command handlers, Spectre.Console output utilities, DI wiring (`Program.cs`), and command registration (`App.cs`). Depends on Core and Data.

### Key Patterns

**Generic repository:** All data access goes through `IRepository<T>` (methods: `List`, `GetById`, `Add`, `Update`, `Remove`). Services receive repositories via constructor injection.

**Command registration (`App.cs`):** `now` and `later` are registered both as top-level commands *and* as subcommands under `task`, so `dewit now "..."` and `dewit task now "..."` both work. All other task operations are under `task`, mood/journal operations are under `journal`, and configuration under `config`.

**Data directory:** All persistent data lives under `DEWIT_DIR` env var or `~/.dewit` by default (resolved via `DewitDirectory` in Core). The SQLite database is `$DEWIT_DIR/dewit.db`. Markdown journal files are at `$DEWIT_DIR/{year}/{MM-dd}.md`.

**Recurring tasks:** When a recurring task is marked complete, `TaskService` automatically creates the next task instance using `RecurringSchedule.ComputeNextDueDate()`. The `RecurParser` utility handles shorthand like `"2d"`, `"3w"` in addition to `"daily"`, `"weekly"`.

**Journal + Mood integration:** Mood entries (in the DB) and journal files (markdown on disk) are separate but linked by date. `JournalService` creates markdown files with YAML frontmatter (date, mood, mood-descriptors). The `journal add` command orchestrates both.

**Startup seeding (`Program.cs`):** On every run, migrations are applied and then two seed checks run: mood descriptor defaults (if `MoodDescriptors` table is empty) and default config keys (`export.csv.title`, `export.json.title`).

**Snapshot tests:** `MoodCalendarSnapshotTests` and `OutputSnapshotTests` use `Spectre.Console.Testing`'s `TestConsole` to capture rendered output, then verify it with the Verify package. Add new snapshot tests when changing any `Output` or `MoodCalendar` rendering.
