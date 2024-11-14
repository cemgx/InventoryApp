using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryApp.Migrations
{
    /// <inheritdoc />
    public partial class fully_invoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Invoices_InvoiceId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_InvoiceId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PurchaseDate",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "InvoiceInfo",
                table: "Invoices",
                newName: "FirmName");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceId1",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InvoiceNo",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchaseDate",
                table: "Invoices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Products_InvoiceId1",
                table: "Products",
                column: "InvoiceId1");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ProductId",
                table: "Invoices",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Products_ProductId",
                table: "Invoices",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Invoices_InvoiceId1",
                table: "Products",
                column: "InvoiceId1",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Products_ProductId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Invoices_InvoiceId1",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_InvoiceId1",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_ProductId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "InvoiceId1",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "InvoiceNo",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PurchaseDate",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "FirmName",
                table: "Invoices",
                newName: "InvoiceInfo");

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchaseDate",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Products_InvoiceId",
                table: "Products",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Invoices_InvoiceId",
                table: "Products",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
