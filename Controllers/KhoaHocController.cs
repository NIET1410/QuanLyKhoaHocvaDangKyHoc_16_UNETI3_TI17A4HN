// Họ và tên: Phan Anh Tú
// Mã sinh viên: 23103100258
// Nội dung thực hiện: Module 2 - Quản lý Khóa học, Tìm kiếm, Lọc, Sắp xếp, Phân trang

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Filters;
using QuanLyKhoaHoc.Models;
using QuanLyKhoaHoc.ViewModels;

namespace QuanLyKhoaHoc.Controllers
{
    public class KhoaHocController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KhoaHocController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /KhoaHoc/Index (Dùng chung cho cả Học viên tra cứu và Admin/Nhân viên quản lý)
        public async Task<IActionResult> Index(
            string? tuKhoa,
            int? maMonHoc,
            string? hinhThuc,
            string? trangThai,
            decimal? hocPhiTu,
            decimal? hocPhiDen,
            string? tinhTrangCho,
            string? sapXep = "ngay_tang",
            int page = 1)
        {
            var query = _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .Include(k => k.DangKyKhoaHocs)
                .AsQueryable();

            // 1. TÌM KIẾM (Theo Tên khóa học, Tên môn học, Tên giảng viên)
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                string kw = tuKhoa.Trim();
                query = query.Where(k =>
                    k.TenKhoaHoc.Contains(kw) ||
                    (k.MonHoc != null && k.MonHoc.TenMonHoc.Contains(kw)) ||
                    (k.GiangVien != null && k.GiangVien.HoTen.Contains(kw)));
            }

            // 2. LỌC (FILTER)
            if (maMonHoc.HasValue)
            {
                query = query.Where(k => k.MaMonHoc == maMonHoc.Value);
            }

            if (!string.IsNullOrWhiteSpace(hinhThuc))
            {
                query = query.Where(k => k.HinhThuc == hinhThuc);
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(k => k.TrangThai == trangThai);
            }

            if (hocPhiTu.HasValue)
            {
                query = query.Where(k => k.HocPhi >= hocPhiTu.Value);
            }

            if (hocPhiDen.HasValue)
            {
                query = query.Where(k => k.HocPhi <= hocPhiDen.Value);
            }

            if (!string.IsNullOrWhiteSpace(tinhTrangCho))
            {
                if (tinhTrangCho == "ConCho")
                {
                    query = query.Where(k =>
                        k.SoLuongToiDa - k.DangKyKhoaHocs.Count(d => d.TrangThai != "Hủy" && d.TrangThai != "Từ chối") > 0);
                }
                else if (tinhTrangCho == "HetCho")
                {
                    query = query.Where(k =>
                        k.SoLuongToiDa - k.DangKyKhoaHocs.Count(d => d.TrangThai != "Hủy" && d.TrangThai != "Từ chối") <= 0);
                }
            }

            // 3. SẮP XẾP (SORT - 8 kiểu)
            query = sapXep switch
            {
                "ten_az" => query.OrderBy(k => k.TenKhoaHoc),
                "ten_za" => query.OrderByDescending(k => k.TenKhoaHoc),
                "ngay_tang" => query.OrderBy(k => k.NgayBatDau),
                "ngay_giam" => query.OrderByDescending(k => k.NgayBatDau),
                "hocphi_tang" => query.OrderBy(k => k.HocPhi),
                "hocphi_giam" => query.OrderByDescending(k => k.HocPhi),
                "cho_tang" => query.OrderBy(k => k.SoLuongToiDa - k.DangKyKhoaHocs.Count(d => d.TrangThai != "Hủy" && d.TrangThai != "Từ chối")),
                "cho_giam" => query.OrderByDescending(k => k.SoLuongToiDa - k.DangKyKhoaHocs.Count(d => d.TrangThai != "Hủy" && d.TrangThai != "Từ chối")),
                _ => query.OrderBy(k => k.NgayBatDau)
            };

            // 4. PHÂN TRANG BẰNG LINQ SKIP() / TAKE()
            int pageSize = 6;
            if (page < 1) page = 1;

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Đổ dữ liệu Dropdown Môn học
            ViewBag.MonHocs = new SelectList(await _context.MonHocs.Where(m => m.TrangThai).ToListAsync(), "MaMonHoc", "TenMonHoc", maMonHoc);

            var vm = new KhoaHocFilterViewModel
            {
                TuKhoa = tuKhoa,
                MaMonHoc = maMonHoc,
                HinhThuc = hinhThuc,
                TrangThai = trangThai,
                HocPhiTu = hocPhiTu,
                HocPhiDen = hocPhiDen,
                TinhTrangCho = tinhTrangCho,
                SapXep = sapXep,
                TrangHienTai = page,
                TongSoTrang = totalPages,
                TongSoBanGhi = totalRecords,
                KichThuocTrang = pageSize,
                DanhSachKhoaHoc = items
            };

            return View(vm);
        }

        // GET: /KhoaHoc/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .Include(k => k.DangKyKhoaHocs)
                    .ThenInclude(d => d.HocVien)
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == id);

            if (khoaHoc == null) return NotFound();

            return View(khoaHoc);
        }

        // GET: /KhoaHoc/Create
        [RoleAuthorize("Admin", "NhanVien")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View(new KhoaHoc());
        }

        // POST: /KhoaHoc/Create
        [RoleAuthorize("Admin", "NhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhoaHoc model)
        {
            ValidateKhoaHocBusinessRules(model);

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(model.MaMonHoc, model.MaGiangVien);
                return View(model);
            }

            _context.KhoaHocs.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã tạo khóa học '{model.TenKhoaHoc}' thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /KhoaHoc/Edit/5
        [RoleAuthorize("Admin", "NhanVien")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var khoaHoc = await _context.KhoaHocs.FindAsync(id);
            if (khoaHoc == null) return NotFound();

            await LoadDropdownsAsync(khoaHoc.MaMonHoc, khoaHoc.MaGiangVien);
            return View(khoaHoc);
        }

        // POST: /KhoaHoc/Edit/5
        [RoleAuthorize("Admin", "NhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KhoaHoc model)
        {
            if (id != model.MaKhoaHoc) return NotFound();

            ValidateKhoaHocBusinessRules(model);

            // Kiểm tra số lượng tối đa không được nhỏ hơn số lượng học viên đã đăng ký
            int daDangKy = await _context.DangKyKhoaHocs
                .CountAsync(d => d.MaKhoaHoc == id && d.TrangThai != "Hủy" && d.TrangThai != "Từ chối");

            if (model.SoLuongToiDa < daDangKy)
            {
                ModelState.AddModelError("SoLuongToiDa", $"Sĩ số tối đa ({model.SoLuongToiDa}) không được nhỏ hơn số học viên đang đăng ký ({daDangKy}).");
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(model.MaMonHoc, model.MaGiangVien);
                return View(model);
            }

            _context.Update(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật thông tin khóa học thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /KhoaHoc/ChangeStatus
        [RoleAuthorize("Admin", "NhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, string trangThaiMoi)
        {
            var khoaHoc = await _context.KhoaHocs.FindAsync(id);
            if (khoaHoc == null) return NotFound();

            string[] validStatuses = { "Chưa mở", "Đang mở", "Đã đầy", "Đang học", "Đã kết thúc", "Đã hủy" };
            if (!validStatuses.Contains(trangThaiMoi))
            {
                TempData["Error"] = "Trạng thái khóa học không hợp lệ.";
                return RedirectToAction(nameof(Details), new { id });
            }

            khoaHoc.TrangThai = trangThaiMoi;
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã cập nhật trạng thái khóa học thành '{trangThaiMoi}'.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: /KhoaHoc/Delete/5
        [RoleAuthorize("Admin", "NhanVien")]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .Include(k => k.DangKyKhoaHocs)
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == id);

            if (khoaHoc == null) return NotFound();
            return View(khoaHoc);
        }

        // POST: /KhoaHoc/Delete/5
        [RoleAuthorize("Admin", "NhanVien")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.DangKyKhoaHocs)
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == id);

            if (khoaHoc == null) return NotFound();

            if (khoaHoc.DangKyKhoaHocs.Any())
            {
                TempData["Error"] = $"Không thể xóa khóa học '{khoaHoc.TenKhoaHoc}' vì đã có {khoaHoc.DangKyKhoaHocs.Count} hồ sơ đăng ký! Hãy chuyển trạng thái sang 'Đã hủy'.";
                return RedirectToAction(nameof(Index));
            }

            _context.KhoaHocs.Remove(khoaHoc);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã xóa khóa học thành công!";
            return RedirectToAction(nameof(Index));
        }

        // Hàm kiểm tra nghiệp vụ ngày tháng, số lượng, học phí
        private void ValidateKhoaHocBusinessRules(KhoaHoc model)
        {
            if (model.NgayKetThuc <= model.NgayBatDau)
            {
                ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải lớn hơn ngày bắt đầu khóa học.");
            }

            if (model.SoLuongToiDa <= 0)
            {
                ModelState.AddModelError("SoLuongToiDa", "Số lượng tối đa phải lớn hơn 0.");
            }

            if (model.HocPhi < 0)
            {
                ModelState.AddModelError("HocPhi", "Học phí không được âm.");
            }
        }

        private async Task LoadDropdownsAsync(int? selectedMonHoc = null, int? selectedGiangVien = null)
        {
            var monHocs = await _context.MonHocs.Where(m => m.TrangThai).OrderBy(m => m.TenMonHoc).ToListAsync();
            var giangViens = await _context.GiangViens.Where(g => g.TrangThai).OrderBy(g => g.HoTen).ToListAsync();

            ViewBag.MaMonHoc = new SelectList(monHocs, "MaMonHoc", "TenMonHoc", selectedMonHoc);
            ViewBag.MaGiangVien = new SelectList(giangViens, "MaGiangVien", "HoTen", selectedGiangVien);
        }
    }
}