using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortalHelpdesk.Migrations
{
    /// <inheritdoc />
    public partial class TicketUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Tickets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tickets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Tickets",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
