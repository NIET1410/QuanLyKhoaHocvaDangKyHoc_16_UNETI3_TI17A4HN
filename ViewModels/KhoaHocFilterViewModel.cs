// Họ và tên: Phan Anh Tú
// Mã sinh viên: 23103100258
// Nội dung thực hiện: Module 2 - ViewModel kết hợp Tìm kiếm, Lọc, Sắp xếp và Phân trang Khóa học

using QuanLyKhoaHoc.Models;

namespace QuanLyKhoaHoc.ViewModels
{
    public class KhoaHocFilterViewModel
    {
        // 1. Từ khóa tìm kiếm (Tên khóa học, Tên môn học, Tên giảng viên)
        public string? TuKhoa { get; set; }

        // 2. Các tiêu chí Lọc (Filter)
        public int? MaMonHoc { get; set; }
        public string? HinhThuc { get; set; }
        public string? TrangThai { get; set; }
        public decimal? HocPhiTu { get; set; }
        public decimal? HocPhiDen { get; set; }
        public string? TinhTrangCho { get; set; } // "ConCho", "HetCho"

        // 3. Tiêu chí Sắp xếp (Sort)
        public string? SapXep { get; set; } = "ngay_tang";
        // ten_az, ten_za, ngay_tang, ngay_giam, hocphi_tang, hocphi_giam, cho_tang, cho_giam

        // 4. Phân trang (Pagination)
        public int TrangHienTai { get; set; } = 1;
        public int TongSoTrang { get; set; }
        public int TongSoBanGhi { get; set; }
        public int KichThuocTrang { get; set; } = 6;

        // 5. Danh sách kết quả trả về
        public List<KhoaHoc> DanhSachKhoaHoc { get; set; } = new List<KhoaHoc>();
    }
}