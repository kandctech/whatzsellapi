using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XZY.WShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Busineses",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Busineses");
        }
    }
}
