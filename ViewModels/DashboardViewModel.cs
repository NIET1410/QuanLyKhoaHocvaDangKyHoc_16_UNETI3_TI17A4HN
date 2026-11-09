// Họ và tên: Nguyễn Minh Tiến
// Mã sinh viên: 23103100298
// Nội dung thực hiện: Module 5 - ViewModels cho Dashboard và Thống kê LINQ

namespace QuanLyKhoaHoc.ViewModels
{
    public class DashboardViewModel
    {
        // 10 chỉ số bắt buộc trên Dashboard
        public int TongSoMonHoc { get; set; }
        public int TongSoKhoaHoc { get; set; }
        public int SoKhoaHocDangKhaDung { get; set; }
        public int TongSoHocVien { get; set; }
        public int TongSoDangKy { get; set; }
        public int SoDangKyChoXuLy { get; set; }
        public int SoDangKyDangXuLy { get; set; }
        public int SoDangKyHoanThanh { get; set; }
        public decimal TongHocPhiThuDuoc { get; set; }
        public decimal TongCongNo { get; set; }

        // Các tập dữ liệu Thống kê bằng LINQ
        public List<ThongKeMonHocItem> KhoaHocTheoMon { get; set; } = new();
        public List<ThongKeKhoaHocItem> HocVienTheoKhoa { get; set; } = new();
        public ThongKeKhoaHocItem? KhoaHocNhieuHocVienNhat { get; set; }
        public List<ThongKeDiemKhoaHocItem> DiemTrungBinhTheoKhoa { get; set; } = new();
        public List<ThongKeDangKyThangItem> DangKyTheoThang { get; set; } = new();

        public int SoLuongDat { get; set; }
        public int SoLuongKhongDat { get; set; }
        public double TyLeDat { get; set; }
        public double TyLeKhongDat { get; set; }
    }

    public class ThongKeMonHocItem
    {
        public string TenMonHoc { get; set; } = string.Empty;
        public int SoKhoaHoc { get; set; }
    }

    public class ThongKeKhoaHocItem
    {
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; } = string.Empty;
        public int SoHocVien { get; set; }
        public decimal DoanhThu { get; set; }
        public decimal CongNo { get; set; }
    }

    public class ThongKeDiemKhoaHocItem
    {
        public string TenKhoaHoc { get; set; } = string.Empty;
        public int SoBaiThi { get; set; }
        public double DiemTrungBinh { get; set; }
    }

    public class ThongKeDangKyThangItem
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
        public int SoDangKy { get; set; }
        public decimal TongThu { get; set; }
    }
}