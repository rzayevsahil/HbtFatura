using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HbtFatura.Api.Migrations
{
    /// <inheritdoc />
    public partial class fourth_mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MainAccountCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainAccountCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MainAccountCodes_Firms_FirmId",
                        column: x => x.FirmId,
                        principalTable: "Firms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MainAccountCodes_FirmId",
                table: "MainAccountCodes",
                column: "FirmId");

            migrationBuilder.CreateIndex(
                name: "IX_MainAccountCodes_FirmId_Code",
                table: "MainAccountCodes",
                columns: new[] { "FirmId", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MainAccountCodes");
        }
    }
}
