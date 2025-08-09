using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XZY.WShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReanmeBusineesNAme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BusinessName",
                table: "Busineses",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "BusinessAddress",
                table: "Busineses",
                newName: "Address");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Busineses",
                newName: "BusinessName");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Busineses",
                newName: "BusinessAddress");
        }
    }
}
