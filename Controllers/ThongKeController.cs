// Họ và tên: Nguyễn Minh Tiến
// Mã sinh viên: 23103100298
// Nội dung thực hiện: Module 5 - Dashboard & Thống kê dữ liệu bằng LINQ (Count, Sum, GroupBy, OrderBy, OrderByDescending, Any)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Filters;
using QuanLyKhoaHoc.ViewModels;

namespace QuanLyKhoaHoc.Controllers
{
    [RoleAuthorize("Admin", "NhanVien")]
    public class ThongKeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThongKeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ThongKe/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var vm = await BuildStatisticsViewModelAsync();
            return View(vm);
        }

        // GET: /ThongKe/Index (Trang Báo cáo & Thống kê LINQ chi tiết)
        public async Task<IActionResult> Index()
        {
            var vm = await BuildStatisticsViewModelAsync();
            return View(vm);
        }

        private async Task<DashboardViewModel> BuildStatisticsViewModelAsync()
        {
            var vm = new DashboardViewModel
            {
                TongSoMonHoc = await _context.MonHocs.CountAsync(),
                TongSoKhoaHoc = await _context.KhoaHocs.CountAsync(),
                SoKhoaHocDangKhaDung = await _context.KhoaHocs.CountAsync(k => k.TrangThai == "Đang mở"),
                TongSoHocVien = await _context.HocViens.CountAsync(),
                TongSoDangKy = await _context.DangKyKhoaHocs.CountAsync(),
                SoDangKyChoXuLy = await _context.DangKyKhoaHocs.CountAsync(d => d.TrangThai == "Chờ xử lý"),
                SoDangKyDangXuLy = await _context.DangKyKhoaHocs.CountAsync(d => d.TrangThai == "Đang xử lý"),
                SoDangKyHoanThanh = await _context.DangKyKhoaHocs.CountAsync(d => d.TrangThai == "Hoàn thành"),
                TongHocPhiThuDuoc = await _context.DangKyKhoaHocs
                    .Where(d => d.TrangThai != "Hủy" && d.TrangThai != "Từ chối")
                    .SumAsync(d => (decimal?)d.SoTienDaDong) ?? 0,
                TongCongNo = await _context.DangKyKhoaHocs
                    .Where(d => d.TrangThai != "Hủy" && d.TrangThai != "Từ chối")
                    .SumAsync(d => (decimal?)d.SoTienConLai) ?? 0
            };

            // 1. Số khóa học theo môn học (GroupBy + Count + OrderByDescending)
            vm.KhoaHocTheoMon = await _context.KhoaHocs
                .Include(k => k.MonHoc)
                .GroupBy(k => k.MonHoc!.TenMonHoc)
                .Select(g => new ThongKeMonHocItem
                {
                    TenMonHoc = g.Key,
                    SoKhoaHoc = g.Count()
                })
                .OrderByDescending(x => x.SoKhoaHoc)
                .ToListAsync();

            // 2. Số học viên, Doanh thu và Công nợ theo từng khóa học
            vm.HocVienTheoKhoa = await _context.KhoaHocs
                .Select(k => new ThongKeKhoaHocItem
                {
                    MaKhoaHoc = k.MaKhoaHoc,
                    TenKhoaHoc = k.TenKhoaHoc,
                    SoHocVien = k.DangKyKhoaHocs.Count(d => d.TrangThai != "Hủy" && d.TrangThai != "Từ chối"),
                    DoanhThu = k.DangKyKhoaHocs.Where(d => d.TrangThai != "Hủy" && d.TrangThai != "Từ chối").Sum(d => (decimal?)d.SoTienDaDong) ?? 0,
                    CongNo = k.DangKyKhoaHocs.Where(d => d.TrangThai != "Hủy" && d.TrangThai != "Từ chối").Sum(d => (decimal?)d.SoTienConLai) ?? 0
                })
                .OrderByDescending(x => x.SoHocVien)
                .ToListAsync();

            // 3. Khóa học có nhiều học viên nhất (Sử dụng Any + FirstOrDefault)
            if (vm.HocVienTheoKhoa.Any())
            {
                vm.KhoaHocNhieuHocVienNhat = vm.HocVienTheoKhoa
                    .OrderByDescending(x => x.SoHocVien)
                    .FirstOrDefault();
            }

            // 4. Điểm trung bình theo khóa học (GroupBy)
            vm.DiemTrungBinhTheoKhoa = await _context.KetQuaHocTaps
                .Include(kq => kq.DangKyKhoaHoc)
                    .ThenInclude(d => d!.KhoaHoc)
                .GroupBy(kq => kq.DangKyKhoaHoc!.KhoaHoc!.TenKhoaHoc)
                .Select(g => new ThongKeDiemKhoaHocItem
                {
                    TenKhoaHoc = g.Key,
                    SoBaiThi = g.Count(),
                    DiemTrungBinh = Math.Round(g.Average(x => x.DiemTongKet), 2)
                })
                .OrderByDescending(x => x.DiemTrungBinh)
                .ToListAsync();

            // 5. Tỷ lệ Đạt / Không đạt
            int tongKetQua = await _context.KetQuaHocTaps.CountAsync();
            if (tongKetQua > 0)
            {
                vm.SoLuongDat = await _context.KetQuaHocTaps.CountAsync(k => k.KetQua == "Đạt");
                vm.SoLuongKhongDat = tongKetQua - vm.SoLuongDat;
                vm.TyLeDat = Math.Round((double)vm.SoLuongDat * 100 / tongKetQua, 1);
                vm.TyLeKhongDat = Math.Round(100 - vm.TyLeDat, 1);
            }

            // 6. Số đăng ký theo tháng (GroupBy Tháng/Năm + OrderBy)
            vm.DangKyTheoThang = await _context.DangKyKhoaHocs
                .GroupBy(d => new { d.NgayDangKy.Year, d.NgayDangKy.Month })
                .Select(g => new ThongKeDangKyThangItem
                {
                    Nam = g.Key.Year,
                    Thang = g.Key.Month,
                    SoDangKy = g.Count(),
                    TongThu = g.Sum(x => x.SoTienDaDong)
                })
                .OrderBy(x => x.Nam)
                .ThenBy(x => x.Thang)
                .ToListAsync();

            return vm;
        }
    }
}