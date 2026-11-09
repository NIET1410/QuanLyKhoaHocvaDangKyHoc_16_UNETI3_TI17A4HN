// Họ và tên: Nguyễn Minh Tiến
// Mã sinh viên: 23103100298
// Nội dung thực hiện: Module 5 - Quản lý Kết quả học tập, tự động tính điểm tổng kết và xếp loại

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Filters;
using QuanLyKhoaHoc.Models;

namespace QuanLyKhoaHoc.Controllers
{
    [RoleAuthorize]
    public class KetQuaHocTapController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KetQuaHocTapController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. DÀNH CHO NHÂN VIÊN & ADMIN: DANH SÁCH HỒ SƠ ĐỦ ĐIỀU KIỆN NHẬP ĐIỂM
        [RoleAuthorize("Admin", "NhanVien")]
        public async Task<IActionResult> Index(int? maKhoaHoc, string? ketQuaFilter)
        {
            var query = _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                .Include(d => d.KetQuaHocTap)
                .Where(d => d.TrangThai == "Hoàn thành")
                .AsQueryable();

            if (maKhoaHoc.HasValue)
            {
                query = query.Where(d => d.MaKhoaHoc == maKhoaHoc.Value);
            }

            if (!string.IsNullOrWhiteSpace(ketQuaFilter))
            {
                if (ketQuaFilter == "ChuaCoDiem")
                    query = query.Where(d => d.KetQuaHocTap == null);
                else
                    query = query.Where(d => d.KetQuaHocTap != null && d.KetQuaHocTap.KetQua == ketQuaFilter);
            }

            ViewBag.KhoaHocs = await _context.KhoaHocs.OrderByDescending(k => k.MaKhoaHoc).ToListAsync();
            ViewBag.MaKhoaHoc = maKhoaHoc;
            ViewBag.KetQuaFilter = ketQuaFilter;

            var list = await query.OrderByDescending(d => d.MaDangKy).ToListAsync();
            return View(list);
        }

        // GET: /KetQuaHocTap/UpdateScore?maDangKy=1
        [RoleAuthorize("Admin", "NhanVien")]
        [HttpGet]
        public async Task<IActionResult> UpdateScore(int maDangKy)
        {
            var dk = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                .Include(d => d.KetQuaHocTap)
                .FirstOrDefaultAsync(d => d.MaDangKy == maDangKy);

            if (dk == null) return NotFound();

            // Kiểm tra điều kiện: Không cho nhập điểm nếu đăng ký chưa ở trạng thái "Hoàn thành"
            if (dk.TrangThai != "Hoàn thành")
            {
                TempData["Error"] = $"Hồ sơ đăng ký #{maDangKy} đang ở trạng thái '{dk.TrangThai}', chưa đủ điều kiện nhập kết quả học tập!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.DangKy = dk;
            var model = dk.KetQuaHocTap ?? new KetQuaHocTap { MaDangKy = maDangKy };
            return View(model);
        }

        // POST: /KetQuaHocTap/UpdateScore
        [RoleAuthorize("Admin", "NhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateScore(KetQuaHocTap model)
        {
            var dk = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                .Include(d => d.KetQuaHocTap)
                .FirstOrDefaultAsync(d => d.MaDangKy == model.MaDangKy);

            if (dk == null) return NotFound();

            if (dk.TrangThai != "Hoàn thành")
            {
                TempData["Error"] = "Không được phép cập nhật kết quả khi đăng ký chưa ở trạng thái Hoàn thành!";
                return RedirectToAction(nameof(Index));
            }

            if (model.DiemChuyenCan < 0 || model.DiemChuyenCan > 10 ||
                model.DiemGiuaKy < 0 || model.DiemGiuaKy > 10 ||
                model.DiemCuoiKy < 0 || model.DiemCuoiKy > 10)
            {
                ModelState.AddModelError(string.Empty, "Tất cả các cột điểm phải nằm trong khoảng từ 0.0 đến 10.0.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.DangKy = dk;
                return View(model);
            }

            if (dk.KetQuaHocTap == null)
            {
                model.TinhKetQuaTuDong();
                _context.KetQuaHocTaps.Add(model);
            }
            else
            {
                dk.KetQuaHocTap.DiemChuyenCan = model.DiemChuyenCan;
                dk.KetQuaHocTap.DiemGiuaKy = model.DiemGiuaKy;
                dk.KetQuaHocTap.DiemCuoiKy = model.DiemCuoiKy;
                dk.KetQuaHocTap.TinhKetQuaTuDong();
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã lưu điểm cho học viên {dk.HocVien?.HoTen}. Điểm tổng kết: {(dk.KetQuaHocTap ?? model).DiemTongKet} ({(dk.KetQuaHocTap ?? model).XepLoai} - {(dk.KetQuaHocTap ?? model).KetQua}).";
            return RedirectToAction(nameof(Index));
        }

        // 2. DÀNH CHO HỌC VIÊN: XEM KẾT QUẢ HỌC TẬP CỦA CHÍNH MÌNH
        [RoleAuthorize("HocVien")]
        public async Task<IActionResult> MyResults()
        {
            int maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan")!.Value;
            var hocVien = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);
            if (hocVien == null) return NotFound();

            var results = await _context.KetQuaHocTaps
                .Include(kq => kq.DangKyKhoaHoc)
                    .ThenInclude(d => d!.KhoaHoc)
                        .ThenInclude(k => k!.MonHoc)
                .Where(kq => kq.DangKyKhoaHoc != null && kq.DangKyKhoaHoc.MaHocVien == hocVien.MaHocVien)
                .OrderByDescending(kq => kq.NgayCapNhat)
                .ToListAsync();

            return View(results);
        }
    }
}