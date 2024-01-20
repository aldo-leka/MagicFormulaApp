using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations
{
    /// <inheritdoc />
    public partial class ChangedFmpMinimumMarketCapitalizationsCheckType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumTimeUpdateMarketCapitalizations",
                table: "Fmp");

            migrationBuilder.AddColumn<int>(
                name: "MinimumTimeinSecondsToUpdateMarketCapitalizations",
                table: "Fmp",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumTimeinSecondsToUpdateMarketCapitalizations",
                table: "Fmp");

            migrationBuilder.AddColumn<DateTime>(
                name: "MinimumTimeUpdateMarketCapitalizations",
                table: "Fmp",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
