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
            migrationBuilder.AddColumn<string>(
                name: "CustomerTaxOffice",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TaxOfficeId",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_TaxOfficeId",
                table: "Customers",
                column: "TaxOfficeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_TaxOffices_TaxOfficeId",
                table: "Customers",
                column: "TaxOfficeId",
                principalTable: "TaxOffices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_TaxOffices_TaxOfficeId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_TaxOfficeId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CustomerTaxOffice",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TaxOfficeId",
                table: "Customers");
        }
    }
}
