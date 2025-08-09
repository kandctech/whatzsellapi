using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XZY.WShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImageurl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Products",
                newName: "UnitPrice");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrls",
                table: "Products",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "Products",
                newName: "Amount");
        }
    }
}
