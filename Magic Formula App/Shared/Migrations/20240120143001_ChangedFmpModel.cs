using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations
{
    /// <inheritdoc />
    public partial class ChangedFmpModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxRequestsPerDay",
                table: "Fmp");

            migrationBuilder.RenameColumn(
                name: "RequestsToday",
                table: "Fmp",
                newName: "LastBatch");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastBatch",
                table: "Fmp",
                newName: "RequestsToday");

            migrationBuilder.AddColumn<int>(
                name: "MaxRequestsPerDay",
                table: "Fmp",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
