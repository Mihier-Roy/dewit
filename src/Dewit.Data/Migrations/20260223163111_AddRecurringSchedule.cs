using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dewit.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRecurringSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecurringScheduleId",
                table: "Tasks",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RecurringSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FrequencyType = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    Interval = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringSchedules", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecurringSchedules");

            migrationBuilder.DropColumn(
                name: "RecurringScheduleId",
                table: "Tasks");
        }
    }
}
