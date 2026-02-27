using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HbtFatura.Api.Migrations
{
    /// <inheritdoc />
    public partial class nineth_mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "CompanySettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankName",
                table: "CompanySettings");
        }
    }
}
