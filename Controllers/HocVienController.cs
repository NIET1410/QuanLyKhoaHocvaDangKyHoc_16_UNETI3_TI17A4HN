// Họ và tên: Tạ Xuân Thưng
// Mã sinh viên: 23103100286
// Nội dung thực hiện: Module 3 - Quản lý Học viên và Hồ sơ cá nhân (Chống sửa ID trên URL)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Filters;
using QuanLyKhoaHoc.Models;

namespace QuanLyKhoaHoc.Controllers
{
    [RoleAuthorize]
    public class HocVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HocVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. DÀNH CHO HỌC VIÊN: XEM & CẬP NHẬT HỒ SƠ CỦA CHÍNH MÌNH
        // Tuyệt đối không nhận id từ URL để ngăn học viên xem/sửa hồ sơ người khác
        // ============================================================
        [RoleAuthorize("HocVien")]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            int maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan")!.Value;
            var hocVien = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);

            if (hocVien == null) return NotFound();
            return View(hocVien);
        }

        [RoleAuthorize("HocVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(HocVien model)
        {
            int maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan")!.Value;
            var hocVienDb = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);

            if (hocVienDb == null) return NotFound();

            // Bảo mật: Chỉ cập nhật đúng bản ghi thuộc Session hiện tại
            hocVienDb.HoTen = model.HoTen.Trim();
            hocVienDb.NgaySinh = model.NgaySinh;
            hocVienDb.GioiTinh = model.GioiTinh;
            hocVienDb.SoDienThoai = model.SoDienThoai.Trim();
            hocVienDb.Email = model.Email.Trim();
            hocVienDb.DiaChi = model.DiaChi;
            hocVienDb.TrinhDo = model.TrinhDo;

            if (hocVienDb.TaiKhoan != null)
            {
                hocVienDb.TaiKhoan.HoTen = hocVienDb.HoTen;
                hocVienDb.TaiKhoan.Email = hocVienDb.Email;
            }

            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("HoTen", hocVienDb.HoTen);

            TempData["Success"] = "Cập nhật hồ sơ cá nhân thành công!";
            return RedirectToAction(nameof(Profile));
        }

        // ============================================================
        // 2. DÀNH CHO ADMIN & NHÂN VIÊN: QUẢN LÝ DANH SÁCH HỌC VIÊN
        // ============================================================
        [RoleAuthorize("Admin", "NhanVien")]
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyKhoaHocs)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(h =>
                    h.HoTen.Contains(search) ||
                    h.Email.Contains(search) ||
                    h.SoDienThoai.Contains(search));
            }

            ViewBag.Search = search;
            var list = await query.OrderByDescending(h => h.MaHocVien).ToListAsync();
            return View(list);
        }

        [RoleAuthorize("Admin", "NhanVien")]
        public async Task<IActionResult> Details(int id)
        {
            var hv = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .Include(h => h.DangKyKhoaHocs)
                    .ThenInclude(d => d.KhoaHoc)
                .Include(h => h.DangKyKhoaHocs)
                    .ThenInclude(d => d.KetQuaHocTap)
                .FirstOrDefaultAsync(h => h.MaHocVien == id);

            if (hv == null) return NotFound();
            return View(hv);
        }
    }
}