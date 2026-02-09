# AGENTS.md - dewit Project Guide

## Project Overview

**dewit** is a CLI task management application built with C# and .NET 7. It provides a simple command-line interface for tracking tasks with statuses (doing, later, done), tags, and filtering capabilities. Data is persisted in a local SQLite database using Entity Framework Core.

### Key Features
- Add tasks with immediate ("now") or deferred ("later") status
- Mark tasks as complete
- Tag-based organization and filtering
- Search and sort capabilities
- Export/import functionality (CSV/JSON)
- Cross-platform potential (currently Windows-focused)

### Technology Stack
- **Runtime**: .NET 7.0
- **Language**: C#
- **Database**: SQLite with Entity Framework Core 7
- **CLI Framework**: System.CommandLine (beta)
- **Console UI**: Spectre.Console
- **Logging**: Serilog (file-based)
- **Data Export**: CsvHelper

---

## Architecture

### Project Structure
```
dewit/
├── src/
│   ├── Dewit.sln                  # Solution file
│   └── Dewit.CLI/                 # Main application project
│       ├── Commands/              # Command implementations
│       ├── Data/                  # Repository pattern + EF Core context
│       ├── Migrations/            # EF Core migrations
│       ├── Models/                # Domain models
│       ├── Utils/                 # Utilities (formatting, sanitization)
│       ├── App.cs                 # Command registration
│       ├── Program.cs             # Entry point + DI setup
│       └── config.json            # Configuration file
├── assets/                        # Documentation assets
├── README.md
└── LICENSE
```

### Key Components

#### 1. **Program.cs** - Application Bootstrap
- Configures Serilog for file logging (`logs/dewit.log`)
- Sets up dependency injection container
- Registers `TaskContext` (EF Core) and `ITaskRepository`
- Runs database migrations on startup
- Global exception handling

#### 2. **App.cs** - Command Router
- Registers all available commands with System.CommandLine
- Commands: `now`, `later`, `done`, `edit`, `list`, `delete`, `export`, `import`

#### 3. **Data Layer**
- **`ITaskRepository`**: Abstraction for data access
- **`SqlTaskRepository`**: SQLite implementation using EF Core
- **`TaskContext`**: DbContext for database operations
- **`TaskItem`**: Domain model with validation attributes

#### 4. **Commands/** - Business Logic
Each command is a separate class inheriting from System.CommandLine's `Command`:
- `AddTaskCommand`: Creates tasks with "now" or "later" status
- `UpdateStatusCommand`: Marks tasks as done
- `UpdateTaskCommand`: Edits title and tags
- `GetTasksCommand`: Lists/filters tasks
- `DeleteTaskCommand`: Removes tasks
- `ExportTasksCommand`: Exports to CSV/JSON
- `ImportTasksCommand`: Imports from CSV/JSON

#### 5. **Utils/**
- **`Output`**: Spectre.Console table formatting
- **`Sanitizer`**: Tag validation (alphanumeric + underscore only)
- **`FormatData`**: Data transformation utilities

---

## Development Setup

### Prerequisites
- .NET 7 SDK or later
- Git
- (Optional) Visual Studio 2022, VS Code, or Rider

### Building the Project
```bash
# Clone repository
git clone https://github.com/Mihier-Roy/dewit.git
cd dewit

# Restore dependencies
cd src/Dewit.CLI
dotnet restore

# Build
dotnet build

# Run
dotnet run -- now "My first task" --tags work,urgent

# Publish for distribution
dotnet publish -c Release -r win-x64 --self-contained
```

### Database
- SQLite database file: `dewit_tasks.db` (created automatically)
- Location: Same directory as executable
- Migrations: Applied automatically on startup
- Connection string: Defined in `config.json`

---


## Quick Reference: Common Tasks

### Adding a New Command
1. Create `Commands/MyCommand.cs` inheriting from `Command`
2. Implement command logic in constructor
3. Register in `App.cs`: `new MyCommand(_repository, "mycommand")`
4. Add tests in `Tests/Commands/MyCommandTests.cs`
5. Update README.md with usage example

### Modifying Database Schema
1. Update model in `Models/TaskItem.cs`
2. Create migration: `dotnet ef migrations add MigrationName`
3. Test migration: `dotnet ef database update`
4. Commit migration file

### Updating Dependencies
1. Update version in `.csproj` (or `Directory.Packages.props` if using central management)
2. Run `dotnet restore`
3. Test thoroughly
4. Update lock file: `dotnet restore --force-evaluate`

---

## Useful Commands

```bash
# Build
dotnet build

# Run tests (when implemented)
dotnet test

# Run application
dotnet run -- [command] [args]

# Publish for specific platform
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true

# Create migration
dotnet ef migrations add MigrationName --project src/Dewit.CLI

# Update database
dotnet ef database update --project src/Dewit.CLI

# Format code
dotnet format

# List outdated packages
dotnet list package --outdated

# Check for vulnerable packages
dotnet list package --vulnerable
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
