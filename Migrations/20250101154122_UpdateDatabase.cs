using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationDeSalle__Back.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "slug",
                table: "Rooms",
                newName: "Slug");

            migrationBuilder.RenameColumn(
                name: "isAccessible",
                table: "Rooms",
                newName: "IsAccessible");

            migrationBuilder.AddColumn<string>(
                name: "Surface",
                table: "Rooms",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Surface",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "Rooms",
                newName: "slug");

            migrationBuilder.RenameColumn(
                name: "IsAccessible",
                table: "Rooms",
                newName: "isAccessible");
        }
    }
}
