# AGENTS.md - dewit Project Guide

## Project Overview

**dewit** is a CLI task management application built with C# and .NET. It provides a simple command-line interface for tracking tasks with statuses (doing, later, done), tags, descriptions, and recurring schedules. It also includes mood tracking with a markdown journal system and customisable mood descriptors. Data is persisted in a local SQLite database using Entity Framework Core.

### Key Features
- Add tasks with immediate ("now") or deferred ("later") status
- Task descriptions and full edit support (title, description, tags, recurrence)
- Recurring task support with flexible schedules (daily, weekly, monthly, yearly)
- Mark tasks as complete (auto-creates next occurrence for recurring tasks)
- Tag-based organisation, filtering, search, and sort capabilities
- Export/import functionality (CSV/JSON)
- Mood tracking with a linked markdown journal system
- Per-mood descriptor customisation with interactive multi-select
- Interactive mood calendar with multiple views (week/month/quarter/year)
- DB-backed configuration system
- Cross-platform potential (currently Windows-focused)

### Technology Stack
- **Runtime**: .NET 10.0
- **Language**: C#
- **Database**: SQLite with Entity Framework Core
- **CLI Framework**: System.CommandLine (beta)
- **Console UI**: Spectre.Console
- **Logging**: Serilog (file-based)
- **Data Export**: CsvHelper
- **Testing**: xUnit + Verify (snapshot testing)

---

## Architecture

### Project Structure
```
dewit/
├── src/
│   ├── Dewit.sln
│   ├── Dewit.Core/                    # Domain logic (entities, interfaces, services, utils)
│   │   ├── Entities/                  # Domain models: TaskItem, ConfigItem, MoodEntry, MoodDescriptorItem, JournalEntry, RecurringSchedule, EntityBase
│   │   ├── Enums/                     # DataFormats, Mood (+ MoodExtensions)
│   │   ├── Interfaces/                # IRepository<T>, ITaskService, IConfigurationService, IMoodService, IJournalService, IDataConverter
│   │   ├── Services/                  # TaskService, ConfigurationService, MoodService, JournalService, DataConverterService
│   │   └── Utils/                     # Sanitizer, DateParser, RecurParser, DewitDirectory, MoodDescriptorDefaults
│   ├── Dewit.Data/                    # Data access layer
│   │   ├── Data/
│   │   │   ├── DewitDbContext.cs      # EF Core DbContext (Tasks, ConfigItems, MoodEntries, MoodDescriptors, JournalEntries, RecurringSchedules)
│   │   │   ├── DbConnectionString.cs  # Resolves SQLite path relative to executable
│   │   │   └── Repositories/
│   │   │       └── Repository.cs      # Generic IRepository<T> implementation
│   │   └── Migrations/               # EF Core migrations
│   ├── Dewit.CLI/                     # CLI presentation layer
│   │   ├── Commands/                  # Task command implementations
│   │   │   ├── Config/               # config + descriptors subcommands
│   │   │   ├── Journal/              # journal subcommands (add, update, view)
│   │   │   └── Task/                 # task subcommands (add, edit, get, delete, export, import, complete)
│   │   ├── Utils/                     # Output (table formatting), MoodCalendar, EditorHelper
│   │   ├── App.cs                     # Command registration
│   │   └── Program.cs                 # Entry point + DI setup + startup seeding
│   └── Dewit.CLI.Tests/               # xUnit test project
│       ├── Data/
│       ├── Entities/
│       ├── Enums/
│       ├── Infrastructure/
│       ├── Repositories/
│       ├── Services/
│       ├── Snapshots/
│       └── Utils/
├── assets/                            # Documentation assets
├── README.md
└── LICENSE
```

### Key Components

#### 1. **Program.cs** - Application Bootstrap
- Configures Serilog for file logging (`logs/dewit.log`)
- Sets up dependency injection container
- Registers `DewitDbContext`, generic `IRepository<T>`, and all services
- Runs EF Core migrations on startup
- Seeds `MoodDescriptorDefaults` into the `MoodDescriptors` table if missing
- Seeds default config keys (`export.csv.title`, `export.json.title`) if missing
- Global exception handling

#### 2. **App.cs** - Command Router
Registers all commands with System.CommandLine, with a global `--verbose` recursive option:
- Top-level task commands: `now`, `later`
- `TaskCommand` — task management command group (`complete`, `edit`, `get`/`list`, `delete`/`rm`, `export`, `import`)
- `JournalCommand` — mood & journal command group (`add`, `update`, `view`)
- `ConfigCommand` — configuration command group (`list`, `set`, `descriptors`)

#### 3. **Data Layer (Dewit.Data)**
- **`DewitDbContext`**: EF Core DbContext with six `DbSet`s: `Tasks`, `ConfigItems`, `MoodEntries`, `MoodDescriptors`, `JournalEntries`, `RecurringSchedules`
- **`IRepository<T>`**: Generic repository abstraction with `List`, `GetById`, `Add`, `Update`, `Remove`
- **`Repository<T>`**: SQLite implementation using EF Core
- **`DbConnectionString`**: Resolves the SQLite path relative to the executable

#### 4. **Domain Layer (Dewit.Core)**

##### Entities
- **`EntityBase`**: Abstract base class with `Id`
- **`TaskItem`**: Task with `Title` (required), `Description` (optional), status, tags, `AddedOn`, `CompletedOn`, and optional `RecurringScheduleId` FK
- **`RecurringSchedule`**: Recurrence definition with `FrequencyType` ("daily"/"weekly"/"monthly"/"yearly") and `Interval`; includes `ComputeNextDueDate(DateTime)` and `ToLabel()` methods
- **`ConfigItem`**: Key/value configuration entry with `CreatedAt`/`UpdatedAt`
- **`MoodEntry`**: Daily mood record with mood enum name and comma-separated descriptors string
- **`MoodDescriptorItem`**: Per-mood descriptor list, customisable by the user
- **`JournalEntry`**: Tracks a journal markdown file by date; stores `FilePath` (format: `{baseDir}/{YYYY}/{MM-dd}.md`) and timestamps

##### Services
- **`ITaskService` / `TaskService`**: Full task CRUD with filtering (duration, status, tags, search), recurring task support (auto-creates next occurrence on completion), and tag sanitization/deduplication
- **`IConfigurationService` / `ConfigurationService`**: DB-backed key/value config (`GetValue`, `SetValue`, `DeleteValue`, `KeyExists`, `GetAll`)
- **`IMoodService` / `MoodService`**: Mood entry CRUD (`GetEntryForDate`, `GetEntriesInRange`, `AddEntry`, `UpdateEntry`) + descriptor management (`GetDescriptors`, `GetAllDescriptors`, `SetDescriptors`, `ResetDescriptors`)
- **`IJournalService` / `JournalService`**: Manages journal markdown files with YAML frontmatter (date, mood, mood-descriptors); lazy-creates files on first access; tracks `CreatedAt`/`UpdatedAt` in the database; provides `CreateOrGetEntry`, `GetEntryForDate`, `GetEntriesInRange`, `GetFilePath`, `TouchUpdatedAt`
- **`IDataConverter` / `DataConverterService`**: CSV/JSON export and import

##### Utils
- **`Sanitizer`**: Tag validation (`SanitizeTags` — alphanumeric + underscore only) and `DeduplicateTags`
- **`DateParser`**: Parses natural-language date strings (e.g. "today", "yesterday", "last monday", "YYYY-MM-DD", "MM-DD"); only allows past or today dates
- **`RecurParser`**: Parses recurrence shorthand ("daily", "weekly", "monthly", "yearly", or "Nd"/"Nw"/"Nm"/"Ny" for intervals); returns `RecurringSchedule`; provides `TryParse` variant
- **`DewitDirectory`**: Resolves base data directory from `DEWIT_DIR` env var or defaults to `~/.dewit`; provides `EnsureExists()`
- **`MoodDescriptorDefaults`**: Built-in default descriptors per mood; seeds DB on startup via `SeedIfMissing()`

#### 5. **Commands (Dewit.CLI)**

##### Task Commands
- `AddTaskCommand` (`now` / `later`): Creates tasks with optional title, description, tags, and recurrence
- `UpdateStatusCommand` (`task complete`): Marks tasks as done with optional completion date; auto-creates next occurrence for recurring tasks
- `UpdateTaskCommand` (`task edit`): Edits title, description, tags (add/remove/reset), and recurrence (set/remove)
- `GetTasksCommand` (`task get` / `task list`): Lists/filters tasks by duration, status, tags, search term, and sort order
- `DeleteTaskCommand` (`task delete` / `task rm`): Removes tasks
- `ExportTasksCommand` (`task export`): Exports to CSV/JSON using config for default filename
- `ImportTasksCommand` (`task import`): Imports from CSV/JSON

##### Journal Commands (`journal`)
- `AddJournalCommand` (`journal add`): Records today's (or a specified date's) mood with interactive multi-select descriptors; optionally creates a markdown journal file and opens it in `$EDITOR`/`$VISUAL`
- `UpdateJournalCommand` (`journal update`): Updates an existing mood/descriptors or opens the linked journal file for editing
- `ViewJournalCommand` (`journal view`): Displays an interactive mood calendar (week/month/quarter/year) with journal entry markers (J); supports navigating entries, opening journal files, and editing

##### Config Commands (`config`)
- `ConfigListCommand` (`config list`): Lists all config key/value pairs
- `ConfigSetCommand` (`config set`): Sets a config value by key
- `DescriptorsCommand` / subcommands (`config descriptors`):
  - `DescriptorsListCommand`: Lists descriptors for a mood
  - `DescriptorsSetCommand`: Sets custom descriptors for a mood
  - `DescriptorsResetCommand`: Resets descriptors for a mood to defaults

#### 6. **Utils (Dewit.CLI)**
- **`Output`**: Spectre.Console table formatting; `WriteTable` renders tasks with coloured status, recurrence indicator, and description preview; `WriteTaskDetail` renders a full detail panel; `WriteText`, `WriteError`, `WriteVerbose`
- **`MoodCalendar`**: Renders week/month/quarter/year calendar grids with coloured mood blocks (██), empty cell markers (░░), and journal entry indicators (J); interactive navigation: d=details, ↑↓=navigate, enter=open journal, e=edit, b=back, esc=exit; includes mood colour legend
- **`EditorHelper`**: Opens files using `$EDITOR` or `$VISUAL` env var; falls back to default OS file opener

---

## Development Setup

### Prerequisites
- .NET 10 SDK or later
- Git
- (Optional) Visual Studio 2022, VS Code, or Rider

### Building the Project
```bash
# Clone repository
git clone https://github.com/Mihier-Roy/dewit.git
cd dewit

# Restore dependencies
dotnet restore src/Dewit.sln

# Build
dotnet build src/Dewit.sln

# Run
dotnet run --project src/Dewit.CLI -- now "My first task" --tags work,urgent

# Publish for distribution
dotnet publish src/Dewit.CLI -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

### Database
- SQLite database file: `dewit.db` (created automatically)
- Default location: `~/.dewit/` (configurable via `DEWIT_DIR` env var)
- Migrations: Applied automatically on startup
- Tables: `Tasks`, `ConfigItems`, `MoodEntries`, `MoodDescriptors`, `JournalEntries`, `RecurringSchedules`

---

## Quick Reference: Common Tasks

### Adding a New Command
1. Create `Commands/<Group>/MyCommand.cs` inheriting from `Command`
2. Inject required services via constructor
3. Register in `App.cs`
4. Add tests in `Dewit.CLI.Tests/Services/` or `Dewit.CLI.Tests/Utils/`
5. Update README.md with usage example

### Adding a New Service
1. Define interface in `Dewit.Core/Interfaces/IMyService.cs`
2. Implement in `Dewit.Core/Services/MyService.cs`
3. Register in `Program.cs`: `services.AddTransient<IMyService, MyService>()`
4. Inject into commands via constructor

### Modifying Database Schema
1. Update entity in `Dewit.Core/Entities/`
2. Add `DbSet<T>` to `DewitDbContext` if needed
3. Create migration: `dotnet ef migrations add MigrationName --project src/Dewit.Data --startup-project src/Dewit.CLI`
4. Test migration: `dotnet ef database update --project src/Dewit.Data --startup-project src/Dewit.CLI`
5. Commit migration file

### Updating Dependencies
1. Update version in the relevant `.csproj`
2. Run `dotnet restore`
3. Test thoroughly
4. Update lock file: `dotnet restore --force-evaluate`

---

## Useful Commands

```bash
# Build solution
dotnet build src/Dewit.sln

# Run tests
dotnet test src/Dewit.sln

# Run application
dotnet run --project src/Dewit.CLI -- [command] [args]

# Publish for specific platform
dotnet publish src/Dewit.CLI -c Release -r win-x64 --self-contained -p:PublishSingleFile=true

# Create migration
dotnet ef migrations add MigrationName --project src/Dewit.Data --startup-project src/Dewit.CLI

# Update database
dotnet ef database update --project src/Dewit.Data --startup-project src/Dewit.CLI

# Format code
dotnet format src/Dewit.sln

# List outdated packages
dotnet list src/Dewit.sln package --outdated

# Check for vulnerable packages
dotnet list src/Dewit.sln package --vulnerable
```

---

## Resources

### Official Documentation
- [System.CommandLine Docs](https://github.com/dotnet/command-line-api/wiki)
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [Spectre.Console](https://spectreconsole.net/)
- [Serilog](https://serilog.net/)
- [Verify (snapshot testing)](https://github.com/VerifyTests/Verify)

### Learning Resources
- [.NET CLI Best Practices](https://learn.microsoft.com/dotnet/core/tools/)
- [Repository Pattern](https://learn.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [Dependency Injection in .NET](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection)

### Similar Projects
- [Taskwarrior](https://taskwarrior.org/) - Inspiration for features
- [todo.txt-cli](https://github.com/todotxt/todo.txt-cli) - Alternative approach
- [dotnet/try](https://github.com/dotnet/try) - Example of well-structured .NET CLI app

---
