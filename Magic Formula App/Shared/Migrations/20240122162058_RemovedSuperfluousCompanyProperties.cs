using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations
{
    /// <inheritdoc />
    public partial class RemovedSuperfluousCompanyProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployedCapital",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "EnterpriseValue",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "NetCurrentAssets",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "OperatingIncomeToEnterpriseValue",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ReturnOnEmployedCapital",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "TotalDebt",
                table: "Companies",
                newName: "IntangibleAssets");

            migrationBuilder.RenameColumn(
                name: "TangibleAssets",
                table: "Companies",
                newName: "Debt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IntangibleAssets",
                table: "Companies",
                newName: "TotalDebt");

            migrationBuilder.RenameColumn(
                name: "Debt",
                table: "Companies",
                newName: "TangibleAssets");

            migrationBuilder.AddColumn<decimal>(
                name: "EmployedCapital",
                table: "Companies",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "EnterpriseValue",
                table: "Companies",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NetCurrentAssets",
                table: "Companies",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<float>(
                name: "OperatingIncomeToEnterpriseValue",
                table: "Companies",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ReturnOnEmployedCapital",
                table: "Companies",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
