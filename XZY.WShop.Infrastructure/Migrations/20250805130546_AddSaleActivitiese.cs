using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XZY.WShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSaleActivitiese : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GeoLocation",
                table: "SaleActivities",
                newName: "Country");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "SaleActivities",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "SaleActivities");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "SaleActivities",
                newName: "GeoLocation");
        }
    }
}
