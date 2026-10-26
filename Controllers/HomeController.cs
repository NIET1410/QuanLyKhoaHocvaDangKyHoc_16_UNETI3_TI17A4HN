// Họ và tên: Nguyễn Duy Trung
// Mã sinh viên: 23103100256
// Nội dung thực hiện: Trang chủ hiển thị tổng quan khóa học và môn học

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;

namespace QuanLyKhoaHoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TongKhoaHoc = await _context.KhoaHocs.CountAsync();
            ViewBag.TongMonHoc = await _context.MonHocs.CountAsync(m => m.TrangThai);
            ViewBag.TongHocVien = await _context.HocViens.CountAsync();

            // Lấy danh sách môn học đang áp dụng
            ViewBag.DanhSachMonHoc = await _context.MonHocs
                .Where(m => m.TrangThai)
                .Include(m => m.KhoaHocs)
                .ToListAsync();

            // Lấy 6 khóa học đang mở nổi bật
            var khoaHocDangMo = await _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .Include(k => k.DangKyKhoaHocs)
                .Where(k => k.TrangThai == "Đang mở")
                .OrderBy(k => k.NgayBatDau)
                .Take(6)
                .ToListAsync();

            return View(khoaHocDangMo);
        }
    }
}