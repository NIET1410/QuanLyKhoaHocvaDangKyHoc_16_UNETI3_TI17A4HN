// Họ và tên: Nguyễn Duy Trung
// Mã sinh viên: 23103100256
// Nội dung thực hiện: Module 1 - Quản lý Giảng viên

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Filters;
using QuanLyKhoaHoc.Models;

namespace QuanLyKhoaHoc.Controllers
{
    [RoleAuthorize("Admin", "NhanVien")]
    public class GiangVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GiangVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.GiangViens
                .Include(g => g.KhoaHocs)
                .OrderByDescending(g => g.MaGiangVien)
                .ToListAsync();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View(new GiangVien());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GiangVien model)
        {
            if (!ModelState.IsValid) return View(model);
            _context.GiangViens.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm giảng viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var gv = await _context.GiangViens.FindAsync(id);
            if (gv == null) return NotFound();
            return View(gv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GiangVien model)
        {
            if (id != model.MaGiangVien) return NotFound();
            if (!ModelState.IsValid) return View(model);
            _context.Update(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật giảng viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var gv = await _context.GiangViens.Include(g => g.KhoaHocs).FirstOrDefaultAsync(g => g.MaGiangVien == id);
            if (gv == null) return NotFound();

            if (gv.KhoaHocs.Any())
            {
                TempData["Error"] = $"Không thể xóa giảng viên '{gv.HoTen}' vì đang phụ trách {gv.KhoaHocs.Count} khóa học!";
                return RedirectToAction(nameof(Index));
            }

            _context.GiangViens.Remove(gv);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa giảng viên thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}