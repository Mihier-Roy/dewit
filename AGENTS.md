# AGENTS.md - dewit Project Guide

## Project Overview

**dewit** is a CLI task management application built with C# and .NET. It provides a simple command-line interface for tracking tasks with statuses (doing, later, done), tags, and filtering capabilities. It also includes mood tracking with customisable descriptors and a DB-backed configuration system. Data is persisted in a local SQLite database using Entity Framework Core.

### Key Features
- Add tasks with immediate ("now") or deferred ("later") status
- Mark tasks as complete
- Tag-based organization and filtering
- Search and sort capabilities
- Export/import functionality (CSV/JSON)
- Mood tracking with per-mood descriptor customisation
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
- **Testing**: xUnit

---

## Architecture

### Project Structure
```
dewit/
├── src/
│   ├── Dewit.sln
│   ├── Dewit.Core/                    # Domain logic (entities, interfaces, services, utils)
│   │   ├── Entities/                  # Domain models: TaskItem, ConfigItem, MoodEntry, MoodDescriptorItem, EntityBase
│   │   ├── Enums/                     # DataFormats, Mood
│   │   ├── Interfaces/                # IRepository<T>, ITaskService, IConfigurationService, IMoodService, IDataConverter
│   │   ├── Services/                  # TaskService, ConfigurationService, MoodService, DataConverterService
│   │   └── Utils/                     # Sanitizer, DateParser, MoodDescriptorDefaults
│   ├── Dewit.Data/                    # Data access layer
│   │   ├── Data/
│   │   │   ├── DewitDbContext.cs      # EF Core DbContext (Tasks, ConfigItems, MoodEntries, MoodDescriptors)
│   │   │   ├── DbConnectionString.cs  # Resolves SQLite path relative to executable
│   │   │   └── Repositories/
│   │   │       └── Repository.cs      # Generic IRepository<T> implementation
│   │   └── Migrations/               # EF Core migrations
│   ├── Dewit.CLI/                     # CLI presentation layer
│   │   ├── Commands/                  # Task command implementations
│   │   │   ├── Config/               # config + descriptors subcommands
│   │   │   └── Mood/                 # mood subcommands
│   │   ├── Utils/                     # Output (table formatting), MoodCalendar
│   │   ├── App.cs                     # Command registration
│   │   └── Program.cs                 # Entry point + DI setup + startup seeding
│   └── Dewit.CLI.Tests/               # xUnit test project
│       ├── Entities/
│       ├── Infrastructure/
│       ├── Repositories/
│       ├── Services/
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
Registers all commands with System.CommandLine:
- Task commands: `now`, `later`, `done`, `edit`, `list`, `delete`, `export`, `import`
- `MoodCommand` — mood tracking command group
- `ConfigCommand` — configuration command group

#### 3. **Data Layer (Dewit.Data)**
- **`DewitDbContext`**: EF Core DbContext with four `DbSet`s: `Tasks`, `ConfigItems`, `MoodEntries`, `MoodDescriptors`
- **`IRepository<T>`**: Generic repository abstraction with `List`, `Add`, `Update`, `Remove`
- **`Repository<T>`**: SQLite implementation using EF Core
- **`DbConnectionString`**: Resolves the SQLite path relative to the executable

#### 4. **Domain Layer (Dewit.Core)**

##### Entities
- **`EntityBase`**: Base class with `Id`, `CreatedAt`, `UpdatedAt`
- **`TaskItem`**: Task with title, status, tags
- **`ConfigItem`**: Key/value configuration entry
- **`MoodEntry`**: Daily mood record with mood enum value and descriptors string
- **`MoodDescriptorItem`**: Per-mood descriptor list, customisable by the user

##### Services
- **`ITaskService` / `TaskService`**: CRUD for tasks
- **`IConfigurationService` / `ConfigurationService`**: DB-backed key/value config (`GetValue`, `SetValue`, `DeleteValue`, `KeyExists`, `GetAll`)
- **`IMoodService` / `MoodService`**: Mood entry CRUD (`GetEntryForDate`, `GetEntriesInRange`, `AddEntry`, `UpdateEntry`) + descriptor management (`GetDescriptors`, `GetAllDescriptors`, `SetDescriptors`, `ResetDescriptors`)
- **`IDataConverter` / `DataConverterService`**: CSV/JSON export and import

##### Utils
- **`Sanitizer`**: Tag validation (alphanumeric + underscore only)
- **`DateParser`**: Parses natural-language date strings (e.g. "today", "yesterday")
- **`MoodDescriptorDefaults`**: Built-in default descriptors per mood; seeds DB on startup

#### 5. **Commands (Dewit.CLI)**

##### Task Commands
- `AddTaskCommand`: Creates tasks with `now` or `later` status
- `UpdateStatusCommand` (`done`): Marks tasks as done
- `UpdateTaskCommand` (`edit`): Edits title and tags
- `GetTasksCommand` (`list`): Lists/filters tasks
- `DeleteTaskCommand` (`delete`): Removes tasks
- `ExportTasksCommand` (`export`): Exports to CSV/JSON using config for default filename
- `ImportTasksCommand` (`import`): Imports from CSV/JSON

##### Mood Commands (`mood`)
- `AddMoodCommand` (`mood add`): Records today's (or a specified date's) mood + descriptors
- `UpdateMoodCommand` (`mood update`): Updates an existing mood entry
- `ViewMoodCommand` (`mood view`): Displays a calendar-style mood view for a date range

##### Config Commands (`config`)
- `ConfigListCommand` (`config list`): Lists all config key/value pairs
- `ConfigSetCommand` (`config set`): Sets a config value by key
- `DescriptorsCommand` / subcommands (`config descriptors`):
  - `DescriptorsListCommand`: Lists descriptors for a mood
  - `DescriptorsSetCommand`: Sets custom descriptors for a mood
  - `DescriptorsResetCommand`: Resets descriptors for a mood to defaults

#### 6. **Utils (Dewit.CLI)**
- **`Output`**: Spectre.Console table formatting
- **`MoodCalendar`**: Renders a calendar grid of mood entries using Spectre.Console

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
- SQLite database file: `dewit_tasks.db` (created automatically)
- Location: Same directory as executable
- Migrations: Applied automatically on startup
- Tables: `Tasks`, `ConfigItems`, `MoodEntries`, `MoodDescriptors`

---

## Quick Reference: Common Tasks

### Adding a New Command
1. Create `Commands/MyCommand.cs` (or a subdirectory) inheriting from `Command`
2. Inject required services via constructor
3. Register in `App.cs`
4. Add tests in `Tests/Services/` or `Tests/Commands/`
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

### Learning Resources
- [.NET CLI Best Practices](https://learn.microsoft.com/dotnet/core/tools/)
- [Repository Pattern](https://learn.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [Dependency Injection in .NET](https://learn.microsoft.com/dotnet/core/extensions/dependency-injection)

### Similar Projects
- [Taskwarrior](https://taskwarrior.org/) - Inspiration for features
- [todo.txt-cli](https://github.com/todotxt/todo.txt-cli) - Alternative approach
- [dotnet/try](https://github.com/dotnet/try) - Example of well-structured .NET CLI app

---
