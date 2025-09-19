using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortalHelpdesk.Migrations
{
    /// <inheritdoc />
    public partial class UserUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ADUsername",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 19, 13, 12, 19, 863, DateTimeKind.Utc).AddTicks(9769));

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 19, 13, 12, 19, 863, DateTimeKind.Utc).AddTicks(9771));

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 19, 13, 12, 19, 863, DateTimeKind.Utc).AddTicks(9772));

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 19, 13, 12, 19, 863, DateTimeKind.Utc).AddTicks(9772));

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 19, 13, 12, 19, 863, DateTimeKind.Utc).AddTicks(9773));

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 19, 13, 12, 19, 863, DateTimeKind.Utc).AddTicks(9774));

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 19, 13, 12, 19, 863, DateTimeKind.Utc).AddTicks(9775));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 19, 13, 12, 19, 863, DateTimeKind.Utc).AddTicks(9584));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ADUsername",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 12, 12, 41, 17, 603, DateTimeKind.Utc).AddTicks(5748));

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 12, 12, 41, 17, 603, DateTimeKind.Utc).AddTicks(5750));

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 12, 12, 41, 17, 603, DateTimeKind.Utc).AddTicks(5750));

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 12, 12, 41, 17, 603, DateTimeKind.Utc).AddTicks(5751));

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 12, 12, 41, 17, 603, DateTimeKind.Utc).AddTicks(5752));

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 12, 12, 41, 17, 603, DateTimeKind.Utc).AddTicks(5753));

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 9, 12, 12, 41, 17, 603, DateTimeKind.Utc).AddTicks(5754));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ADUsername", "CreatedAt" },
                values: new object[] { "SYSTEM\\system", new DateTime(2025, 9, 12, 12, 41, 17, 603, DateTimeKind.Utc).AddTicks(5438) });
        }
    }
}
