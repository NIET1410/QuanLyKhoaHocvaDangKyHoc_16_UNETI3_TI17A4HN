// Họ và tên: Tạ Xuân Thưng (23103100286) & Ngô Duy Thái (23103100247)
// Mã sinh viên: 23103100286, 23103100247
// Nội dung thực hiện: Module 3 & Module 4 - Học viên đăng ký khóa học & Nhân viên tiếp nhận, xử lý luồng trạng thái

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Filters;
using QuanLyKhoaHoc.Models;
using QuanLyKhoaHoc.ViewModels;

namespace QuanLyKhoaHoc.Controllers
{
    [RoleAuthorize]
    public class DangKyKhoaHocController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DangKyKhoaHocController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================================
        // PHẦN 1 (SINH VIÊN 3): HỌC VIÊN ĐĂNG KÝ KHÓA HỌC & THEO DÕI ĐĂNG KÝ CỦA MÌNH
        // =========================================================================

        // GET: /DangKyKhoaHoc/Confirm?maKhoaHoc=3
        [RoleAuthorize("HocVien")]
        [HttpGet]
        public async Task<IActionResult> Confirm(int maKhoaHoc)
        {
            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.MonHoc)
                .Include(k => k.GiangVien)
                .Include(k => k.DangKyKhoaHocs)
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == maKhoaHoc);

            if (khoaHoc == null) return NotFound();

            return View(khoaHoc);
        }

        // POST: /DangKyKhoaHoc/RegisterCourse
        [RoleAuthorize("HocVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterCourse(int maKhoaHoc, string? ghiChu)
        {
            int maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan")!.Value;

            // 1. Kiểm tra Học viên tồn tại & Tài khoản đang hoạt động
            var hocVien = await _context.HocViens
                .Include(h => h.TaiKhoan)
                .FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);

            if (hocVien == null || hocVien.TaiKhoan == null || !hocVien.TaiKhoan.TrangThai || !hocVien.TrangThai)
            {
                TempData["Error"] = "Hồ sơ học viên hoặc tài khoản của bạn không hợp lệ / đang bị khóa.";
                return RedirectToAction("Index", "KhoaHoc");
            }

            // 2. Kiểm tra Khóa học tồn tại
            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.DangKyKhoaHocs)
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == maKhoaHoc);

            if (khoaHoc == null)
            {
                TempData["Error"] = "Khóa học không tồn tại trong hệ thống.";
                return RedirectToAction("Index", "KhoaHoc");
            }

            // 3. Kiểm tra Khóa học đang mở
            if (khoaHoc.TrangThai != "Đang mở")
            {
                TempData["Error"] = $"Khóa học '{khoaHoc.TenKhoaHoc}' hiện đang ở trạng thái '{khoaHoc.TrangThai}', không thể đăng ký!";
                return RedirectToAction("Details", "KhoaHoc", new { id = maKhoaHoc });
            }

            // 4. Kiểm tra Ngày giao dịch hợp lệ (Khóa học chưa kết thúc)
            if (DateTime.Today > khoaHoc.NgayKetThuc)
            {
                TempData["Error"] = "Khóa học đã quá hạn đăng ký.";
                return RedirectToAction("Details", "KhoaHoc", new { id = maKhoaHoc });
            }

            // 5. Kiểm tra Chưa đăng ký khóa đó trước đây (không tính đơn đã Hủy)
            bool daDangKy = await _context.DangKyKhoaHocs.AnyAsync(d =>
                d.MaHocVien == hocVien.MaHocVien &&
                d.MaKhoaHoc == maKhoaHoc &&
                d.TrangThai != "Hủy");

            if (daDangKy)
            {
                TempData["Error"] = "Bạn đã đăng ký khóa học này trước đó rồi! Không thể tạo đăng ký trùng.";
                return RedirectToAction(nameof(MyRegistrations));
            }

            // 6. Kiểm tra Khóa học còn chỗ & Không vượt quá SoLuongToiDa
            int soLuongHienTai = khoaHoc.DangKyKhoaHocs.Count(d => d.TrangThai != "Hủy" && d.TrangThai != "Từ chối");
            if (soLuongHienTai >= khoaHoc.SoLuongToiDa)
            {
                khoaHoc.TrangThai = "Đã đầy";
                await _context.SaveChangesAsync();
                TempData["Error"] = "Rất tiếc, khóa học này vừa đủ sĩ số tối đa (Đã đầy)!";
                return RedirectToAction("Details", "KhoaHoc", new { id = maKhoaHoc });
            }

            // 7. Tạo bản ghi đăng ký mới (Thời gian giao dịch do hệ thống tự xác định)
            var dangKy = new DangKyKhoaHoc
            {
                MaHocVien = hocVien.MaHocVien,
                MaKhoaHoc = maKhoaHoc,
                NgayDangKy = DateTime.Now,
                TrangThai = "Chờ xử lý",
                SoTienDaDong = 0,
                SoTienConLai = khoaHoc.HocPhi,
                GhiChu = string.IsNullOrWhiteSpace(ghiChu) ? "Học viên đăng ký trực tuyến" : ghiChu.Trim()
            };

            _context.DangKyKhoaHocs.Add(dangKy);

            // Nếu sau lượt đăng ký này chạm ngưỡng tối đa -> Tự động chuyển khóa học sang "Đã đầy"
            if (soLuongHienTai + 1 >= khoaHoc.SoLuongToiDa)
            {
                khoaHoc.TrangThai = "Đã đầy";
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đăng ký khóa học '{khoaHoc.TenKhoaHoc}' thành công! Mã đăng ký của bạn là #{dangKy.MaDangKy}.";
            return RedirectToAction(nameof(MyRegistrations));
        }

        // GET: /DangKyKhoaHoc/MyRegistrations (Học viên chỉ xem được dữ liệu của chính mình)
        [RoleAuthorize("HocVien")]
        public async Task<IActionResult> MyRegistrations()
        {
            int maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan")!.Value;
            var hocVien = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);
            if (hocVien == null) return NotFound();

            var list = await _context.DangKyKhoaHocs
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.MonHoc)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.GiangVien)
                .Include(d => d.KetQuaHocTap)
                .Where(d => d.MaHocVien == hocVien.MaHocVien)
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();

            return View(list);
        }

        // POST: /DangKyKhoaHoc/CancelMyRegistration (Học viên chỉ được hủy khi đang "Chờ xử lý")
        [RoleAuthorize("HocVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelMyRegistration(int maDangKy)
        {
            int maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan")!.Value;
            var hocVien = await _context.HocViens.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);
            if (hocVien == null) return NotFound();

            // Bảo mật: Kiểm tra đúng MaDangKy thuộc về MaHocVien của Session hiện tại
            var dk = await _context.DangKyKhoaHocs
                .Include(d => d.KhoaHoc)
                .FirstOrDefaultAsync(d => d.MaDangKy == maDangKy && d.MaHocVien == hocVien.MaHocVien);

            if (dk == null)
            {
                TempData["Error"] = "Không tìm thấy hồ sơ đăng ký hoặc bạn không có quyền thao tác!";
                return RedirectToAction(nameof(MyRegistrations));
            }

            if (dk.TrangThai != "Chờ xử lý")
            {
                TempData["Error"] = $"Không thể tự hủy đăng ký khi hồ sơ đã ở trạng thái '{dk.TrangThai}'. Vui lòng liên hệ nhân viên đào tạo!";
                return RedirectToAction(nameof(MyRegistrations));
            }

            dk.TrangThai = "Hủy";
            dk.SoTienConLai = 0;
            dk.GhiChu += " | Học viên tự hủy khi chờ xử lý.";

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã hủy đăng ký #{maDangKy} thành công.";
            return RedirectToAction(nameof(MyRegistrations));
        }

        // =========================================================================
        // PHẦN 2 (SINH VIÊN 4): NHÂN VIÊN TIẾP NHẬN & XỬ LÝ LUỒNG TRẠNG THÁI ĐĂNG KÝ
        // =========================================================================

        [RoleAuthorize("Admin", "NhanVien")]
        public async Task<IActionResult> Index(
            string? timHocVien,
            string? timKhoaHoc,
            string? trangThai,
            int? maMonHoc,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? sapXep = "moi_nhat")
        {
            var query = _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.MonHoc)
                .Include(d => d.KetQuaHocTap)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(timHocVien))
            {
                query = query.Where(d => d.HocVien != null &&
                    (d.HocVien.HoTen.Contains(timHocVien) || d.HocVien.SoDienThoai.Contains(timHocVien)));
            }

            if (!string.IsNullOrWhiteSpace(timKhoaHoc))
            {
                query = query.Where(d => d.KhoaHoc != null && d.KhoaHoc.TenKhoaHoc.Contains(timKhoaHoc));
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(d => d.TrangThai == trangThai);
            }

            if (maMonHoc.HasValue)
            {
                query = query.Where(d => d.KhoaHoc != null && d.KhoaHoc.MaMonHoc == maMonHoc.Value);
            }

            if (tuNgay.HasValue)
            {
                query = query.Where(d => d.NgayDangKy.Date >= tuNgay.Value.Date);
            }

            if (denNgay.HasValue)
            {
                query = query.Where(d => d.NgayDangKy.Date <= denNgay.Value.Date);
            }

            query = sapXep switch
            {
                "cu_nhat" => query.OrderBy(d => d.NgayDangKy),
                "hocvien_az" => query.OrderBy(d => d.HocVien!.HoTen),
                "congno_giam" => query.OrderByDescending(d => d.SoTienConLai),
                _ => query.OrderByDescending(d => d.NgayDangKy)
            };

            ViewBag.MonHocs = new SelectList(await _context.MonHocs.ToListAsync(), "MaMonHoc", "TenMonHoc", maMonHoc);

            var vm = new DangKyFilterViewModel
            {
                TimHocVien = timHocVien,
                TimKhoaHoc = timKhoaHoc,
                TrangThai = trangThai,
                MaMonHoc = maMonHoc,
                TuNgay = tuNgay,
                DenNgay = denNgay,
                SapXep = sapXep,
                DanhSachDangKy = await query.ToListAsync()
            };

            return View(vm);
        }

        // GET: /DangKyKhoaHoc/Details/5 (Chi tiết & Xử lý chuyển trạng thái dành cho Nhân viên)
        [RoleAuthorize("Admin", "NhanVien")]
        public async Task<IActionResult> Details(int id)
        {
            var dk = await _context.DangKyKhoaHocs
                .Include(d => d.HocVien)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.MonHoc)
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.DangKyKhoaHocs)
                .Include(d => d.KetQuaHocTap)
                .FirstOrDefaultAsync(d => d.MaDangKy == id);

            if (dk == null) return NotFound();
            return View(dk);
        }

        // POST: /DangKyKhoaHoc/ProcessStatus (Kiểm tra chặt chẽ luồng trạng thái State Machine)
        [RoleAuthorize("Admin", "NhanVien")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessStatus(int maDangKy, string trangThaiMoi, decimal soTienDongThem, string? ghiChu)
        {
            var dk = await _context.DangKyKhoaHocs
                .Include(d => d.KhoaHoc)
                    .ThenInclude(k => k!.DangKyKhoaHocs)
                .FirstOrDefaultAsync(d => d.MaDangKy == maDangKy);

            if (dk == null || dk.KhoaHoc == null) return NotFound();

            string trangThaiHienTai = dk.TrangThai;

            // KIỂM TRA LUỒNG TRẠNG THÁI (STATE MANAGEMENT)
            // Luồng chuẩn: Chờ xử lý -> Đang xử lý -> Hoàn thành (Hoặc sang Hủy / Từ chối)
            bool hopLe = false;

            if (trangThaiHienTai == "Chờ xử lý")
            {
                // Từ Chờ xử lý KHÔNG ĐƯỢC nhảy cóc sang Hoàn thành
                hopLe = (trangThaiMoi == "Đang xử lý" || trangThaiMoi == "Từ chối" || trangThaiMoi == "Hủy");
            }
            else if (trangThaiHienTai == "Đang xử lý")
            {
                hopLe = (trangThaiMoi == "Hoàn thành" || trangThaiMoi == "Từ chối" || trangThaiMoi == "Hủy" || trangThaiMoi == "Đang xử lý");
            }
            else if (trangThaiHienTai == "Hoàn thành")
            {
                // Cho phép đóng thêm học phí còn nợ khi đã Hoàn thành
                hopLe = (trangThaiMoi == "Hoàn thành");
            }

            if (!hopLe)
            {
                TempData["Error"] = $"Sai luồng trạng thái nghiệp vụ! Không được phép chuyển trực tiếp từ '{trangThaiHienTai}' sang '{trangThaiMoi}'. (Luồng bắt buộc: Chờ xử lý -> Đang xử lý -> Hoàn thành).";
                return RedirectToAction(nameof(Details), new { id = maDangKy });
            }

            // Kiểm tra số tiền đóng thêm
            if (soTienDongThem < 0)
            {
                TempData["Error"] = "Số tiền thu thêm không được âm!";
                return RedirectToAction(nameof(Details), new { id = maDangKy });
            }

            if (soTienDongThem > 0)
            {
                if (dk.SoTienDaDong + soTienDongThem > dk.KhoaHoc.HocPhi)
                {
                    TempData["Error"] = "Tổng số tiền đã đóng không được vượt quá học phí của khóa học!";
                    return RedirectToAction(nameof(Details), new { id = maDangKy });
                }

                dk.SoTienDaDong += soTienDongThem;
                dk.SoTienConLai = dk.KhoaHoc.HocPhi - dk.SoTienDaDong;
            }

            // Nếu Hủy hoặc Từ chối -> Xóa công nợ còn lại
            if (trangThaiMoi == "Hủy" || trangThaiMoi == "Từ chối")
            {
                dk.SoTienConLai = 0;
            }

            // Nếu chuyển sang Hoàn thành -> Ghi nhận NgayXacNhan
            if (trangThaiMoi == "Hoàn thành" && dk.NgayXacNhan == null)
            {
                dk.NgayXacNhan = DateTime.Now;
            }

            dk.TrangThai = trangThaiMoi;
            if (!string.IsNullOrWhiteSpace(ghiChu))
            {
                dk.GhiChu = ghiChu.Trim();
            }

            // Kiểm tra cảnh báo giới hạn sĩ số khóa học
            int soLuongHopLe = dk.KhoaHoc.DangKyKhoaHocs.Count(d => d.TrangThai != "Hủy" && d.TrangThai != "Từ chối");
            if (soLuongHopLe >= dk.KhoaHoc.SoLuongToiDa && dk.KhoaHoc.TrangThai == "Đang mở")
            {
                dk.KhoaHoc.TrangThai = "Đã đầy";
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã cập nhật hồ sơ đăng ký #{maDangKy} sang trạng thái '{trangThaiMoi}'.";
            return RedirectToAction(nameof(Details), new { id = maDangKy });
        }
    }
}