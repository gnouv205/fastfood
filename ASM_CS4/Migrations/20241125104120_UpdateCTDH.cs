using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASM_CS4.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCTDH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MatKhau",
                table: "Customers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "ChiTietDonDatHangs",
                columns: table => new
                {
                    MaChiTiet = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaKhachHang = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaSanPham = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayGiao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayNhan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDonDatHangs", x => x.MaChiTiet);
                    table.ForeignKey(
                        name: "FK_ChiTietDonDatHangs_Customers_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "Customers",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDonDatHangs_Products_MaSanPham",
                        column: x => x.MaSanPham,
                        principalTable: "Products",
                        principalColumn: "MaSanPham",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonDatHangs_MaKhachHang",
                table: "ChiTietDonDatHangs",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonDatHangs_MaSanPham",
                table: "ChiTietDonDatHangs",
                column: "MaSanPham");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietDonDatHangs");

            migrationBuilder.AlterColumn<string>(
                name: "MatKhau",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
