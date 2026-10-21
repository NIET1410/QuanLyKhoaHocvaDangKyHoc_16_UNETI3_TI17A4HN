using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKhoaHoc.Migrations
{
    /// <inheritdoc />
    public partial class KhoiTao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GiangVien",
                columns: table => new
                {
                    MaGiangVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    ChuyenMon = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiangVien", x => x.MaGiangVien);
                });

            migrationBuilder.CreateTable(
                name: "MonHoc",
                columns: table => new
                {
                    MaMonHoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenMonHoc = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SoTinChi = table.Column<int>(type: "int", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    HocPhi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonHoc", x => x.MaMonHoc);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "KhoaHoc",
                columns: table => new
                {
                    MaKhoaHoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKhoaHoc = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MaMonHoc = table.Column<int>(type: "int", nullable: false),
                    MaGiangVien = table.Column<int>(type: "int", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoLuongToiDa = table.Column<int>(type: "int", nullable: false),
                    HocPhi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HinhThuc = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhoaHoc", x => x.MaKhoaHoc);
                    table.ForeignKey(
                        name: "FK_KhoaHoc_GiangVien_MaGiangVien",
                        column: x => x.MaGiangVien,
                        principalTable: "GiangVien",
                        principalColumn: "MaGiangVien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KhoaHoc_MonHoc_MaMonHoc",
                        column: x => x.MaMonHoc,
                        principalTable: "MonHoc",
                        principalColumn: "MaMonHoc",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HocVien",
                columns: table => new
                {
                    MaHocVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TrinhDo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HocVien", x => x.MaHocVien);
                    table.ForeignKey(
                        name: "FK_HocVien_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DangKyKhoaHoc",
                columns: table => new
                {
                    MaDangKy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHocVien = table.Column<int>(type: "int", nullable: false),
                    MaKhoaHoc = table.Column<int>(type: "int", nullable: false),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SoTienDaDong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoTienConLai = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayXacNhan = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyKhoaHoc", x => x.MaDangKy);
                    table.ForeignKey(
                        name: "FK_DangKyKhoaHoc_HocVien_MaHocVien",
                        column: x => x.MaHocVien,
                        principalTable: "HocVien",
                        principalColumn: "MaHocVien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DangKyKhoaHoc_KhoaHoc_MaKhoaHoc",
                        column: x => x.MaKhoaHoc,
                        principalTable: "KhoaHoc",
                        principalColumn: "MaKhoaHoc",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KetQuaHocTap",
                columns: table => new
                {
                    MaKetQua = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDangKy = table.Column<int>(type: "int", nullable: false),
                    DiemChuyenCan = table.Column<double>(type: "float", nullable: false),
                    DiemGiuaKy = table.Column<double>(type: "float", nullable: false),
                    DiemCuoiKy = table.Column<double>(type: "float", nullable: false),
                    DiemTongKet = table.Column<double>(type: "float", nullable: false),
                    XepLoai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    KetQua = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaHocTap", x => x.MaKetQua);
                    table.ForeignKey(
                        name: "FK_KetQuaHocTap_DangKyKhoaHoc_MaDangKy",
                        column: x => x.MaDangKy,
                        principalTable: "DangKyKhoaHoc",
                        principalColumn: "MaDangKy",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DangKyKhoaHoc_MaHocVien",
                table: "DangKyKhoaHoc",
                column: "MaHocVien");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyKhoaHoc_MaKhoaHoc",
                table: "DangKyKhoaHoc",
                column: "MaKhoaHoc");

            migrationBuilder.CreateIndex(
                name: "IX_HocVien_MaTaiKhoan",
                table: "HocVien",
                column: "MaTaiKhoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaHocTap_MaDangKy",
                table: "KetQuaHocTap",
                column: "MaDangKy",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KhoaHoc_MaGiangVien",
                table: "KhoaHoc",
                column: "MaGiangVien");

            migrationBuilder.CreateIndex(
                name: "IX_KhoaHoc_MaMonHoc",
                table: "KhoaHoc",
                column: "MaMonHoc");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_TenDangNhap",
                table: "TaiKhoan",
                column: "TenDangNhap",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KetQuaHocTap");

            migrationBuilder.DropTable(
                name: "DangKyKhoaHoc");

            migrationBuilder.DropTable(
                name: "HocVien");

            migrationBuilder.DropTable(
                name: "KhoaHoc");

            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "GiangVien");

            migrationBuilder.DropTable(
                name: "MonHoc");
        }
    }
}
