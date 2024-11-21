using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASM_CS4.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MaSanPham",
                table: "Carts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_MaSanPham",
                table: "Carts",
                column: "MaSanPham");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Products_MaSanPham",
                table: "Carts",
                column: "MaSanPham",
                principalTable: "Products",
                principalColumn: "MaSanPham",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Products_MaSanPham",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_MaSanPham",
                table: "Carts");

            migrationBuilder.AlterColumn<string>(
                name: "MaSanPham",
                table: "Carts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
