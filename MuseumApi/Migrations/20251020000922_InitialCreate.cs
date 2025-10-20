using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuseumApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MuseumDailyHours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TimeOpen = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    TimeClosed = table.Column<TimeSpan>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuseumDailyHours", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpecialEvents",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EventName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    EventDescription = table.Column<string>(type: "TEXT", maxLength: 750, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialEvents", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "SpecialEventDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialEventDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpecialEventDates_SpecialEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "SpecialEvents",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    TicketId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TicketDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TicketType = table.Column<string>(type: "TEXT", nullable: false),
                    EventId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.TicketId);
                    table.ForeignKey(
                        name: "FK_Tickets_SpecialEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "SpecialEvents",
                        principalColumn: "EventId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MuseumDailyHours_Date",
                table: "MuseumDailyHours",
                column: "Date",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecialEventDates_EventId_Date",
                table: "SpecialEventDates",
                columns: new[] { "EventId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_EventId",
                table: "Tickets",
                column: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MuseumDailyHours");

            migrationBuilder.DropTable(
                name: "SpecialEventDates");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "SpecialEvents");
        }
    }
}
