using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryApp.Migrations
{
    /// <inheritdoc />
    public partial class diagram_fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "ProductId1",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_InvoiceId",
                table: "Products",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ProductId1",
                table: "Invoices",
                column: "ProductId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Products_ProductId1",
                table: "Invoices",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Invoices_InvoiceId",
                table: "Products",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Products_ProductId1",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Invoices_InvoiceId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_InvoiceId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_ProductId1",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "Invoices");

            migrationBuilder.AddColumn<int>(
                name: "InvoiceId1",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
    }
}
