using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionToInventoryItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "InventoryItems",
                type: "bytea",
                rowVersion: true,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_ProductId",
                table: "InventoryItems",
                column: "ProductId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_ProductId",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "InventoryItems");
        }
    }
}
