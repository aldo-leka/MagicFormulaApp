using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations.SqlServerMigrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CIK = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastMarketCapitalization = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    LastTotalDebt = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    LastCashAndCashEquivalents = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    LastNetCurrentAssets = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    LastNetPropertyPlantAndEquipment = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    LastOperatingIncome = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    LastEnterpriseValue = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    LastEmployedCapital = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    LastOperatingIncomeToEnterpriseValue = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    LastReturnOnAssets = table.Column<decimal>(type: "decimal(19,4)", precision: 19, scale: 4, nullable: true),
                    LastFilingDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
