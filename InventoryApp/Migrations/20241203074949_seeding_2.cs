using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryApp.Migrations
{
    /// <inheritdoc />
    public partial class seeding_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Inventories",
                keyColumn: "Id",
                keyValue: 1,
                column: "DeliveredDate",
                value: new DateTime(2024, 12, 3, 7, 49, 49, 5, DateTimeKind.Utc).AddTicks(1695));

            migrationBuilder.UpdateData(
                table: "Inventories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DeliveredDate", "ReturnDate" },
                values: new object[] { new DateTime(2024, 12, 3, 7, 49, 49, 5, DateTimeKind.Utc).AddTicks(2252), new DateTime(2024, 12, 3, 10, 49, 49, 5, DateTimeKind.Local).AddTicks(2258) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: 1,
                column: "PurchaseDate",
                value: new DateTime(2024, 12, 3, 10, 49, 49, 10, DateTimeKind.Local).AddTicks(4383));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: 2,
                column: "PurchaseDate",
                value: new DateTime(2024, 12, 3, 10, 49, 49, 10, DateTimeKind.Local).AddTicks(4667));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Inventories",
                keyColumn: "Id",
                keyValue: 1,
                column: "DeliveredDate",
                value: new DateTime(2024, 12, 3, 7, 47, 33, 947, DateTimeKind.Utc).AddTicks(2611));

            migrationBuilder.UpdateData(
                table: "Inventories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DeliveredDate", "ReturnDate" },
                values: new object[] { new DateTime(2024, 12, 3, 7, 47, 33, 947, DateTimeKind.Utc).AddTicks(3195), new DateTime(2024, 12, 3, 10, 47, 33, 947, DateTimeKind.Local).AddTicks(3205) });

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: 1,
                column: "PurchaseDate",
                value: new DateTime(2024, 12, 3, 10, 47, 33, 951, DateTimeKind.Local).AddTicks(9976));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: 2,
                column: "PurchaseDate",
                value: new DateTime(2024, 12, 3, 10, 47, 33, 952, DateTimeKind.Local).AddTicks(275));
        }
    }
}
