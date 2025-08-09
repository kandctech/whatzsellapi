using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XZY.WShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderNumber3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Orders",
                newName: "Amount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Orders",
                newName: "TotalAmount");
        }
    }
}
