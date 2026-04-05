
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HbtFatura.Api.Migrations
{
    /// <inheritdoc />
    public partial class mig_10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotent: NameEn / DisplayNameEn bazı ortamlarda zaten eklendi (silinmiş mig_9, manuel script vb.)
            migrationBuilder.Sql("""
                IF COL_LENGTH(N'dbo.Lookups', N'NameEn') IS NULL
                    ALTER TABLE [Lookups] ADD [NameEn] nvarchar(max) NULL;
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH(N'dbo.LookupGroups', N'DisplayNameEn') IS NULL
                    ALTER TABLE [LookupGroups] ADD [DisplayNameEn] nvarchar(max) NULL;
                """);

            migrationBuilder.CreateTable(
                name: "UiTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Culture = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UiTranslations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UiTranslations_Key_Culture",
                table: "UiTranslations",
                columns: new[] { "Key", "Culture" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UiTranslations");

            migrationBuilder.Sql("""
                IF COL_LENGTH(N'dbo.Lookups', N'NameEn') IS NOT NULL
                    ALTER TABLE [Lookups] DROP COLUMN [NameEn];
                """);

            migrationBuilder.Sql("""
                IF COL_LENGTH(N'dbo.LookupGroups', N'DisplayNameEn') IS NOT NULL
                    ALTER TABLE [LookupGroups] DROP COLUMN [DisplayNameEn];
                """);
        }
    }
}
