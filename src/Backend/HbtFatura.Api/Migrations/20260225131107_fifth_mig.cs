using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HbtFatura.Api.Migrations
{
    /// <inheritdoc />
    public partial class fifth_mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MainAccountCodes_FirmId_Code",
                table: "MainAccountCodes");

            migrationBuilder.AlterColumn<Guid>(
                name: "FirmId",
                table: "MainAccountCodes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_MainAccountCodes_Code",
                table: "MainAccountCodes",
                column: "Code",
                unique: true,
                filter: "[FirmId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MainAccountCodes_FirmId_Code",
                table: "MainAccountCodes",
                columns: new[] { "FirmId", "Code" },
                unique: true,
                filter: "[FirmId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MainAccountCodes_Code",
                table: "MainAccountCodes");

            migrationBuilder.DropIndex(
                name: "IX_MainAccountCodes_FirmId_Code",
                table: "MainAccountCodes");

            migrationBuilder.AlterColumn<Guid>(
                name: "FirmId",
                table: "MainAccountCodes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MainAccountCodes_FirmId_Code",
                table: "MainAccountCodes",
                columns: new[] { "FirmId", "Code" },
                unique: true);
        }
    }
}
