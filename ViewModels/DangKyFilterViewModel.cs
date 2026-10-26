// Họ và tên: Ngô Duy Thái
// Mã sinh viên: 23103100247
// Nội dung thực hiện: Module 4 - ViewModel tìm kiếm và lọc danh sách đăng ký cho Nhân viên

using QuanLyKhoaHoc.Models;

namespace QuanLyKhoaHoc.ViewModels
{
    public class DangKyFilterViewModel
    {
        public string? TimHocVien { get; set; }
        public string? TimKhoaHoc { get; set; }
        public string? TrangThai { get; set; }
        public int? MaMonHoc { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string? SapXep { get; set; } = "moi_nhat";

        public List<DangKyKhoaHoc> DanhSachDangKy { get; set; } = new List<DangKyKhoaHoc>();
    }
}