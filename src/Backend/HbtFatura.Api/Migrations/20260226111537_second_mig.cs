using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HbtFatura.Api.Migrations
{
    /// <inheritdoc />
    public partial class second_mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InvoiceId",
                table: "DeliveryNotes",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "DeliveryNotes");
        }
    }
}
