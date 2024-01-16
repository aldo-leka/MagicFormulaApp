using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Migrations
{
    /// <inheritdoc />
    public partial class AddedCommonSharesOutstanding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CIK = table.Column<string>(type: "TEXT", nullable: true),
                    CompanyName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommonSharesOutstanding",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FiscalYear = table.Column<int>(type: "INTEGER", nullable: false),
                    FiscalPart = table.Column<string>(type: "TEXT", nullable: true),
                    Form = table.Column<string>(type: "TEXT", nullable: true),
                    FileDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompanyId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonSharesOutstanding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommonSharesOutstanding_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommonSharesOutstanding_CompanyId",
                table: "CommonSharesOutstanding",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommonSharesOutstanding");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
