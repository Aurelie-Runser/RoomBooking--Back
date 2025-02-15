using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationDeSalle__Back.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookingDateFormate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateTo",
                table: "Bookings",
                newName: "TimeTo");

            migrationBuilder.RenameColumn(
                name: "DateFrom",
                table: "Bookings",
                newName: "TimeFrom");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Day",
                table: "Bookings",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }
    }
}
