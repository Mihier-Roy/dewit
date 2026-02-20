# dewit

**dewit** is a simple CLI application for you to track what you're working on.

![dewit sample](assets/dewit-gif.gif)

## Getting Started

1. Download the binary from the [Releases page](https://github.com/Mihier-Roy/dewit/releases/latest).
2. Extract the files to a folder of your choice.
3. (Optional) Add the folder to your PATH so that you have access to the executable at all times.
4. If 3 has not been done, navigate to the folder with the extracted files and run the application!

## Usage

```
dewit [command] [arguments] [options]
```

`dewit` is organized into sub-commands for each action. The commands available are listed below, but can always be found by running `dewit -h`.

Pass `--verbose` anywhere in your command to see diagnostic output (shown in grey). Useful when something goes wrong and you want more detail:

```
dewit --verbose task list
dewit task done 5 --verbose
```

```
Commands:
  now <title>       - Add a new task and set it to 'Doing'.
  later <title>     - Add a new task and set it to 'Later'.
  task              - Manage all of your tasks.
    now <title>     - Add a new task and set it to 'Doing'.
    later <title>   - Add a new task and set it to 'Later'.
    done <id>       - Complete a task.
    edit <id>       - Edit an existing task.
    list            - List all your tasks.
    delete <id>     - Delete a task from your list.
    export          - Export all your tasks to a CSV or JSON file.
    import <path>   - Import existing tasks from another Dewit database.
  mood              - Track and view your daily mood.
    add             - Log your mood for today.
    update          - Update a mood entry.
    view            - Display your mood calendar.
  config            - View and manage application configuration.
    list            - List all configuration values.
    set <key> <val> - Set a configuration value.
    descriptors     - View and manage mood descriptors.
      list          - List descriptors for all moods.
      set <mood> <descriptors> - Set descriptors for a mood.
      reset [mood]  - Reset descriptors for a mood (or all moods) to defaults.
```

### Adding and completing tasks

-   Add a task using `dewit now` or `dewit later`. These shortcuts work at the top level and also as `dewit task now` / `dewit task later`.

    ```
    dewit [now|later] "New task" --tags tag1,tag2,tag_3
    ```

-   Complete a task using `dewit task done`. Use `--completed-at` to backdate the completion.

    ```
    dewit task done <task-ID>
    dewit task done <task-ID> --completed-at yesterday
    ```

### Editing a task

`dewit task edit` allows you to edit the title and tags of a task. Options may be used independently or in combination.

-   Edit the title of a task

    ```
    dewit task edit <task-ID> --title "New title"
    ```

-   Add/Remove Tags

    ```
    dewit task edit <task-ID> --add-tags new_tag --remove-tags old_tag
    ```

-   Reset Tags (remove all tags associated with a task)

    ```
    dewit task edit <task-ID> --reset-tags
    ```

### Displaying tasks

`dewit task list` displays tasks. By default it shows only tasks created today. The options below may be used individually or combined.

-   The `--duration` option accepts: `all|month|today|week|yesterday`.

    ```
    dewit task list --duration week
    ```

-   Filter by status using `--status`, which accepts: `doing|done|later`.

    ```
    dewit task list --status later
    ```

-   Filter by tags using `--tags`.

    ```
    dewit task list --tags tag1
    ```

-   Sort results using `--sort`, which accepts: `date|status`.

    ```
    dewit task list --sort date
    ```

-   Search task titles using `--search`.

    ```
    dewit task list --search "search expression"
    ```

### Tags

Tags can be added to a task as a comma-separated list. Only alphanumeric characters and underscores are accepted as tag names.
Example: `--tags tag1,tag2,tag_3`

### Import/Export

`dewit` supports exporting all saved tasks to a CSV or JSON file and importing them back. The import command appends tasks from the file to your existing list.

```
dewit task export --format json
dewit task export --format csv --path /some/directory

dewit task import /path/to/file.json
dewit task import /path/to/file.csv --format csv
```

### Mood tracking

`dewit mood` lets you log a daily mood, update past entries, and view a color-coded calendar.

#### Logging today's mood

Run without flags for an interactive prompt, or pass flags to skip it:

```
dewit mood add
dewit mood add --mood happy --descriptors calm,focused
```

Moods: `veryhappy` · `happy` · `meh` · `down` · `extradown`

#### Updating a mood entry

Defaults to today. Use `--date` to target a different day:

```
dewit mood update
dewit mood update --date yesterday --mood meh
dewit mood update --date 2026-02-15 --mood happy --descriptors content,relaxed
```

`--date` accepts: `today`, `yesterday`, `last monday`, `YYYY-MM-DD`, `MM-DD`

#### Viewing the calendar

```
dewit mood view                          # current week (default)
dewit mood view --duration month
dewit mood view --duration month  --period 2026-01
dewit mood view --duration quarter --period 2026-Q1
dewit mood view --duration year   --period 2026
```

Each logged day is shown as a colored block — green for Very Happy, through red for Extra Down — with a legend below.

### Configuration

`dewit config` lets you view and manage application settings.

#### Listing and setting values

```
dewit config list
dewit config set export.csv.title my_tasks
dewit config set export.json.title my_tasks
```

#### Mood descriptors

Descriptors are the words shown when logging or updating a mood. You can customize them per mood level.

```
dewit config descriptors list
dewit config descriptors set happy calm,focused,productive
dewit config descriptors reset happy
dewit config descriptors reset --all
```

## Development

Getting setup to build the project should be relatively straight-forward. The following steps will guide you through the process:

1. Clone the repository
    ```bash
    git clone https://github.com/Mihier-Roy/dewit
    ```
2. Navigate to the source directory and restore dependencies
    ```bash
    cd dewit/src
    dotnet restore
    ```
3. Build and run the application (migrations apply automatically on first run)
    ```bash
    cd Dewit.CLI
    dotnet run -- now "New task" --tags some,tags,here
    ```

    Note: Run commands from the `Dewit.CLI` directory so the config file can be found.

### Dependencies/Libraries Used

-   [System.CommandLine](https://github.com/dotnet/command-line-api)
-   [EntityFramework Core + SQLite](https://github.com/dotnet/efcore)
-   [CsvHelper](https://github.com/JoshClose/CsvHelper)
-   [Serilog](https://github.com/serilog/serilog)
-   [Spectre.Console](https://github.com/spectresystems/spectre.console)

## Contribute

`dewit` was developed using C# and .NET, mostly as a way for me to get familiar with using the `System.CommandLine` NuGet library to build command line applications.

Feel free to contribute any new features or fixes by submitting a PR or creating an Issue.

## License

-   see [LICENSE](https://github.com/username/sw-name/blob/master/LICENSE.md) file
