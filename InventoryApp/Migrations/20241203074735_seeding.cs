using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InventoryApp.Migrations
{
    /// <inheritdoc />
    public partial class seeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Email", "ForgotCode", "IsDeleted", "IsVerified", "MailVerificationCode", "Name", "Password", "Salt" },
                values: new object[,]
                {
                    { 1, "a@a.com", null, false, true, null, "A", "111", "111" },
                    { 2, "s@s.com", null, false, true, null, "S", "111", "111" }
                });

            migrationBuilder.InsertData(
                table: "Invoices",
                columns: new[] { "Id", "FirmName", "InvoiceNo", "IsDeleted", "Price", "PurchaseDate" },
                values: new object[,]
                {
                    { 1, "Tekno", 4321, false, "6000", new DateTime(2024, 12, 3, 10, 47, 33, 951, DateTimeKind.Local).AddTicks(9976) },
                    { 2, "GardenShop", 1234, false, "5000", new DateTime(2024, 12, 3, 10, 47, 33, 952, DateTimeKind.Local).AddTicks(275) }
                });

            migrationBuilder.InsertData(
                table: "ProductTypes",
                columns: new[] { "Id", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, false, "Elektronik" },
                    { 2, false, "Bahçe" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "InvoiceId", "IsDeleted", "Name", "ProductTypeId" },
                values: new object[,]
                {
                    { 1, 1, false, "Klavye", 1 },
                    { 2, 2, false, "Kürek", 2 }
                });

            migrationBuilder.InsertData(
                table: "Inventories",
                columns: new[] { "Id", "DeliveredDate", "GivenByEmployeeId", "IsDeleted", "IsTaken", "ProductId", "ReceivedByEmployeeId", "ReturnDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 12, 3, 7, 47, 33, 947, DateTimeKind.Utc).AddTicks(2611), 1, false, false, 1, 2, null },
                    { 2, new DateTime(2024, 12, 3, 7, 47, 33, 947, DateTimeKind.Utc).AddTicks(3195), 2, false, false, 2, 1, new DateTime(2024, 12, 3, 10, 47, 33, 947, DateTimeKind.Local).AddTicks(3205) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Inventories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Inventories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductTypes",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
