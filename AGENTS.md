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

## Critical Gaps & Recommendations

### 🚨 **Immediate Priorities**

#### 1. Testing Infrastructure (CRITICAL)
**Current State**: Zero tests exist
**Impact**: No confidence in refactoring, high risk of regressions

**Action Items**:
- [ ] Add `Dewit.CLI.Tests` project with xUnit
- [ ] Mock `ITaskRepository` with Moq for command testing
- [ ] Add integration tests for `SqlTaskRepository` with in-memory SQLite
- [ ] Test all command handlers
- [ ] Test data sanitization and validation
- [ ] Target: Minimum 70% code coverage

**Example Structure**:
```
tests/
├── Dewit.CLI.Tests/
│   ├── Commands/
│   │   ├── AddTaskCommandTests.cs
│   │   └── GetTasksCommandTests.cs
│   ├── Data/
│   │   └── SqlTaskRepositoryTests.cs
│   └── Utils/
│       └── SanitizerTests.cs
```

#### 2. CI/CD Pipeline (CRITICAL)
**Current State**: No automation, manual releases
**Impact**: Inconsistent builds, slow release cycle, no quality gates

**Action Items**:
- [ ] Create `.github/workflows/ci.yml` for automated builds
- [ ] Add test execution to CI pipeline
- [ ] Add code coverage reporting (Coverlet + Codecov)
- [ ] Create release workflow for multi-platform binaries (win-x64, linux-x64, osx-x64, osx-arm64)
- [ ] Add automated dependency updates (Dependabot)
- [ ] Add security scanning (GitHub Advanced Security or Snyk)

**Recommended CI Workflow**:
```yaml
name: CI
on: [push, pull_request]
jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '7.0.x'
      - run: dotnet restore
      - run: dotnet build --no-restore
      - run: dotnet test --no-build --verbosity normal /p:CollectCoverage=true
      - run: dotnet publish -c Release -r win-x64 --self-contained
      - run: dotnet publish -c Release -r linux-x64 --self-contained
      - run: dotnet publish -c Release -r osx-x64 --self-contained
```

#### 3. Code Quality Tools (HIGH)
**Current State**: No static analysis, linting, or formatting enforcement
**Impact**: Inconsistent code style, potential bugs missed

**Action Items**:
- [ ] Add `.editorconfig` for consistent formatting
- [ ] Enable nullable reference types enforcement (partially enabled)
- [ ] Add Roslyn analyzers to project file:
  ```xml
  <PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="8.0.0" />
  <PackageReference Include="StyleCop.Analyzers" Version="1.2.0-beta.556" />
  <PackageReference Include="SonarAnalyzer.CSharp" Version="9.16.0.82469" />
  ```
- [ ] Configure analyzer severity in `.editorconfig`
- [ ] Add pre-commit hooks with Husky.NET
- [ ] Enforce analysis in CI (treat warnings as errors in Release)

#### 4. Dependency Updates (HIGH)
**Current State**: Using RC and beta versions
**Impact**: Missing bug fixes, security patches, and features

**Action Items**:
- [ ] Update Entity Framework Core from `7.0.0-rc.1` to latest stable (8.0.x or 9.0.x)
- [ ] Update System.CommandLine from `2.0.0-beta1` to stable version
- [ ] Update all Microsoft.Extensions.* packages to stable versions
- [ ] Add `Directory.Packages.props` for central package management
- [ ] Configure Dependabot or Renovate for automatic updates

**Central Package Management**:
```xml
<!-- Directory.Packages.props -->
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <PackageVersion Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.0" />
    <!-- ... -->
  </ItemGroup>
</Project>
```

### ⚠️ **High Impact Improvements**

#### 5. Multi-Platform Support
**Current Issue**: Hardcoded `<RuntimeIdentifier>win-x64</RuntimeIdentifier>`
**Impact**: Cannot build for Linux/macOS

**Fix**:
Remove the hardcoded RID from `.csproj`, add runtime-specific publish profiles:
```xml
<!-- Remove from .csproj -->
<RuntimeIdentifier>win-x64</RuntimeIdentifier>

<!-- Use during publish -->
dotnet publish -c Release -r win-x64 --self-contained
dotnet publish -c Release -r linux-x64 --self-contained
dotnet publish -c Release -r osx-x64 --self-contained
dotnet publish -c Release -r osx-arm64 --self-contained
```

#### 6. Configuration Management
**Current Issue**: Hardcoded paths, config file in repo
**Impact**: Difficult to configure, not following .NET best practices

**Recommendations**:
- Use .NET User Secrets for development: `dotnet user-secrets init`
- Support environment variables: `DEWIT_CONNECTION_STRING`, `DEWIT_LOG_PATH`
- Use `IOptions<T>` pattern for strongly-typed configuration
- Document configuration options in README
- Add `config.example.json` template, gitignore actual `config.json`

#### 7. Logging Configuration
**Current Issue**: Hardcoded Debug level, file-only logging
**Impact**: Cannot adjust verbosity, no console output option

**Improvements**:
- Move log level to configuration
- Add console logging option
- Use structured logging with Serilog's `LogContext`
- Add log levels per command (verbose mode flag)
- Consider application insights or similar for production telemetry

#### 8. Docker Support
**Current State**: No containerization
**Benefits**: Consistent environment, easier deployment, Linux support

**Action Items**:
- [ ] Create `Dockerfile` for multi-stage build
- [ ] Create `docker-compose.yml` for local development
- [ ] Publish Docker images to GitHub Container Registry
- [ ] Document Docker usage in README

**Example Dockerfile**:
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Dewit.CLI/Dewit.CLI.csproj", "Dewit.CLI/"]
RUN dotnet restore "Dewit.CLI/Dewit.CLI.csproj"
COPY src/ .
RUN dotnet publish "Dewit.CLI/Dewit.CLI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["./dewit"]
```

### 📚 **Documentation Improvements**

#### 9. Missing Documentation
**Add**:
- [ ] `CONTRIBUTING.md` - How to contribute, code style, PR process
- [ ] `CHANGELOG.md` - Version history and release notes
- [ ] `CODE_OF_CONDUCT.md` - Community guidelines
- [ ] `ARCHITECTURE.md` - Design decisions, patterns used
- [ ] XML documentation comments on public APIs
- [ ] GitHub issue templates
- [ ] Pull request template

#### 10. API Documentation
**Current**: No XML docs
**Add**: XML documentation comments for all public types and members

```csharp
/// <summary>
/// Represents a task item with status tracking and tagging capabilities.
/// </summary>
public class TaskItem
{
    /// <summary>
    /// Gets or sets the unique identifier for the task.
    /// </summary>
    [Key]
    public int? Id { get; set; }
    // ...
}
```

Enable in `.csproj`:
```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn> <!-- Suppress missing XML comment warnings initially -->
</PropertyGroup>
```

### 🔒 **Security Enhancements**

#### 11. Security Scanning
**Action Items**:
- [ ] Enable GitHub Dependabot security alerts
- [ ] Add `dotnet list package --vulnerable` to CI
- [ ] Consider adding SAST tool (SonarCloud, Snyk)
- [ ] Run regular dependency audits
- [ ] Add security policy (`SECURITY.md`)

#### 12. Input Validation
**Current**: Basic validation via data annotations
**Improvements**:
- Add comprehensive input sanitization for file paths (import/export)
- Validate file formats before parsing
- Add size limits for imported files
- Sanitize SQL inputs (already handled by EF, but document)

### 🎯 **Code Architecture Improvements**

#### 13. Separation of Concerns
**Current**: All logic in CLI project
**Recommendation**: Split into libraries for reusability

**Proposed Structure**:
```
Dewit.Core/           # Domain models, interfaces
Dewit.Data/           # EF Core, repository implementations
Dewit.CLI/            # Command-line interface
Dewit.Tests/          # Tests
```

#### 14. Error Handling
**Current**: Global try-catch in Program.cs
**Improvements**:
- Add specific exception types (`TaskNotFoundException`, etc.)
- Add validation errors with user-friendly messages
- Return appropriate exit codes (0 = success, 1 = error)
- Add `--verbose` flag for detailed error output

#### 15. Testability
**Improvements**:
- Extract file I/O operations to `IFileSystem` interface
- Make `System.DateTime.Now` injectable via `ISystemClock`
- Avoid static dependencies
- Add factory patterns where needed

### 📊 **Performance & Optimization**

#### 16. Database Performance
**Potential Issues**:
- No indexes on frequently queried fields
- Loading all tasks with `ToList()` (could be slow with many tasks)

**Recommendations**:
- Add indexes on `AddedOn`, `Status`, `Tags` columns
- Use pagination for `list` command with large datasets
- Add `AsNoTracking()` for read-only queries
- Consider adding `--limit` and `--offset` options

#### 17. Build Optimization
**Current**: Trimming enabled, good
**Additional**:
- Consider NativeAOT for even faster startup
- Benchmark trimmed vs non-trimmed size
- Profile startup time

### 🔄 **Continuous Improvement**

#### 18. Versioning
**Add**:
- Semantic versioning in `.csproj`
- `--version` command
- Automatic version bumping in CI

```xml
<PropertyGroup>
  <Version>1.0.0</Version>
  <AssemblyVersion>1.0.0.0</AssemblyVersion>
  <FileVersion>1.0.0.0</FileVersion>
</PropertyGroup>
```

#### 19. Release Automation
**Current**: Manual releases
**Automate**:
- [ ] Create GitHub Release on tag push
- [ ] Attach binaries for all platforms
- [ ] Generate release notes from commits/PRs
- [ ] Publish to package managers (chocolatey, winget, homebrew)

#### 20. Telemetry & Analytics (Optional)
**Consider**:
- Anonymous usage statistics (opt-in)
- Error reporting (Sentry, Application Insights)
- Feature usage metrics to guide development

---

## Development Workflow Recommendations

### For Contributors

1. **Setup**:
   ```bash
   git clone <repo>
   cd dewit/src/Dewit.CLI
   dotnet restore
   dotnet build
   ```

2. **Making Changes**:
   - Create feature branch: `git checkout -b feature/my-feature`
   - Write tests first (TDD approach)
   - Run tests: `dotnet test`
   - Run linting: `dotnet format --verify-no-changes`
   - Commit with conventional commits: `feat: add task priority field`

3. **Before PR**:
   - Ensure all tests pass
   - Update documentation
   - Add changelog entry
   - Ensure code coverage hasn't decreased

### For Maintainers

1. **Review Checklist**:
   - [ ] Tests included and passing
   - [ ] Documentation updated
   - [ ] No security vulnerabilities introduced
   - [ ] Code follows project style
   - [ ] Breaking changes documented
   - [ ] Changelog updated

2. **Release Process**:
   - Update version in `.csproj`
   - Update `CHANGELOG.md`
   - Create git tag: `git tag v1.2.0`
   - Push tag: `git push --tags`
   - CI automatically builds and publishes release

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

## Roadmap Suggestions

### Version 1.x (Stabilization)
- [ ] Add comprehensive test suite
- [ ] Set up CI/CD pipeline
- [ ] Update all dependencies to stable versions
- [ ] Multi-platform releases
- [ ] Code quality tools integration

### Version 2.x (Features)
- [ ] Task priorities (high, medium, low)
- [ ] Due dates and reminders
- [ ] Task dependencies/subtasks
- [ ] Time tracking (start/stop timer)
- [ ] Recurring tasks
- [ ] Task notes/descriptions
- [ ] Sync with cloud services (optional)

### Version 3.x (Advanced)
- [ ] Web UI (Blazor)
- [ ] Mobile app
- [ ] Team collaboration features
- [ ] Integrations (GitHub, Jira, etc.)
- [ ] AI-powered task suggestions
- [ ] Natural language input

---

## Contact & Support

- **Issues**: [GitHub Issues](https://github.com/Mihier-Roy/dewit/issues)
- **Discussions**: [GitHub Discussions](https://github.com/Mihier-Roy/dewit/discussions)
- **Contributing**: See `CONTRIBUTING.md` (to be created)
- **License**: MIT

---

**Last Updated**: 2026-02-09
**Project Status**: Active Development
**Current Version**: Pre-release (no semantic versioning yet)
