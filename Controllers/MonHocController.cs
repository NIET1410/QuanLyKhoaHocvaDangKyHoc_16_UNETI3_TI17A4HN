// Họ và tên: Nguyễn Duy Trung
// Mã sinh viên: 23103100256
// Nội dung thực hiện: Module 1 - Quản lý Môn học (CRUD + Chặn xóa khi đã có khóa học)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Filters;
using QuanLyKhoaHoc.Models;

namespace QuanLyKhoaHoc.Controllers
{
    [RoleAuthorize("Admin", "NhanVien")]
    public class MonHocController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MonHocController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.MonHocs
                .Include(m => m.KhoaHocs)
                .OrderByDescending(m => m.MaMonHoc)
                .ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            var monHoc = await _context.MonHocs
                .Include(m => m.KhoaHocs)
                .ThenInclude(k => k.GiangVien)
                .FirstOrDefaultAsync(m => m.MaMonHoc == id);

            if (monHoc == null) return NotFound();
            return View(monHoc);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new MonHoc());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MonHoc model)
        {
            if (await _context.MonHocs.AnyAsync(m => m.TenMonHoc == model.TenMonHoc))
            {
                ModelState.AddModelError("TenMonHoc", "Tên môn học này đã tồn tại.");
            }

            if (model.HocPhi < 0)
            {
                ModelState.AddModelError("HocPhi", "Học phí không được nhỏ hơn 0.");
            }

            if (!ModelState.IsValid) return View(model);

            _context.MonHocs.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm môn học mới thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var monHoc = await _context.MonHocs.FindAsync(id);
            if (monHoc == null) return NotFound();
            return View(monHoc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MonHoc model)
        {
            if (id != model.MaMonHoc) return NotFound();

            if (await _context.MonHocs.AnyAsync(m => m.TenMonHoc == model.TenMonHoc && m.MaMonHoc != id))
            {
                ModelState.AddModelError("TenMonHoc", "Tên môn học đã bị trùng.");
            }

            if (!ModelState.IsValid) return View(model);

            _context.Update(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật môn học thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var monHoc = await _context.MonHocs.FindAsync(id);
            if (monHoc == null) return NotFound();

            monHoc.TrangThai = !monHoc.TrangThai;
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã chuyển trạng thái môn học '{monHoc.TenMonHoc}'.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var monHoc = await _context.MonHocs
                .Include(m => m.KhoaHocs)
                .FirstOrDefaultAsync(m => m.MaMonHoc == id);
            if (monHoc == null) return NotFound();
            return View(monHoc);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var monHoc = await _context.MonHocs
                .Include(m => m.KhoaHocs)
                .FirstOrDefaultAsync(m => m.MaMonHoc == id);

            if (monHoc == null) return NotFound();

            // Kiểm tra toàn vẹn dữ liệu: Không cho xóa nếu đã có Khóa học tham chiếu
            if (monHoc.KhoaHocs.Any())
            {
                TempData["Error"] = $"Không thể xóa môn học '{monHoc.TenMonHoc}' vì đã có {monHoc.KhoaHocs.Count} khóa học thuộc môn này! Hãy chuyển trạng thái sang Ngừng áp dụng.";
                return RedirectToAction(nameof(Index));
            }

            _context.MonHocs.Remove(monHoc);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa môn học thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}