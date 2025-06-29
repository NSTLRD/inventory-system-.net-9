using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ms_products.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceHistoryOwnedType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PriceHistory",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceHistory",
                table: "Products");
        }
    }
}
