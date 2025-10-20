using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MuseumApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "MuseumDailyHours",
                columns: new[] { "Id", "Date", "TimeClosed", "TimeOpen" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 17, 0, 0, 0), new TimeSpan(0, 9, 0, 0, 0) },
                    { 2, new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 17, 0, 0, 0), new TimeSpan(0, 9, 0, 0, 0) },
                    { 3, new DateTime(2025, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 20, 0, 0, 0), new TimeSpan(0, 9, 0, 0, 0) }
                });

            migrationBuilder.InsertData(
                table: "SpecialEvents",
                columns: new[] { "EventId", "EventDescription", "EventName", "Price" },
                values: new object[,]
                {
                    { new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b35"), "Experience the prehistoric era", "Dinosaur Exhibition", 25.00m },
                    { new Guid("da2fd609-d754-4feb-8acd-c4f9ff13ba96"), "Journey through the cosmos", "Space Adventure", 30.00m }
                });

            migrationBuilder.InsertData(
                table: "SpecialEventDates",
                columns: new[] { "Id", "Date", "EventId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b35") },
                    { 2, new DateTime(2025, 10, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b35") },
                    { 3, new DateTime(2025, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("da2fd609-d754-4feb-8acd-c4f9ff13ba96") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MuseumDailyHours",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MuseumDailyHours",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MuseumDailyHours",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SpecialEventDates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SpecialEventDates",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SpecialEventDates",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SpecialEvents",
                keyColumn: "EventId",
                keyValue: new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b35"));

            migrationBuilder.DeleteData(
                table: "SpecialEvents",
                keyColumn: "EventId",
                keyValue: new Guid("da2fd609-d754-4feb-8acd-c4f9ff13ba96"));
        }
    }
}
