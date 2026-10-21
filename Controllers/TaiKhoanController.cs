// Họ và tên: Nguyễn Duy Trung
// Mã sinh viên: 23103100256
// Nội dung thực hiện: Module 1 - Quản lý Tài khoản, Đăng nhập, Đăng xuất, Phân quyền

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Data;
using QuanLyKhoaHoc.Filters;
using QuanLyKhoaHoc.Models;
using QuanLyKhoaHoc.ViewModels;

namespace QuanLyKhoaHoc.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /TaiKhoan/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("MaTaiKhoan") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /TaiKhoan/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.HocVien)
                .FirstOrDefaultAsync(t => t.TenDangNhap == model.TenDangNhap && t.MatKhau == model.MatKhau);

            if (taiKhoan == null)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác!");
                return View(model);
            }

            if (!taiKhoan.TrangThai)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ Quản trị viên!");
                return View(model);
            }

            // Lưu thông tin vào Session theo đúng quy định đề bài
            HttpContext.Session.SetInt32("MaTaiKhoan", taiKhoan.MaTaiKhoan);
            HttpContext.Session.SetString("TenDangNhap", taiKhoan.TenDangNhap);
            HttpContext.Session.SetString("HoTen", taiKhoan.HoTen);
            HttpContext.Session.SetString("VaiTro", taiKhoan.VaiTro);

            if (taiKhoan.HocVien != null)
            {
                HttpContext.Session.SetInt32("MaHocVien", taiKhoan.HocVien.MaHocVien);
            }

            TempData["Success"] = $"Xin chào {taiKhoan.HoTen}! Đăng nhập thành công.";

            if (taiKhoan.VaiTro == "Admin" || taiKhoan.VaiTro == "NhanVien")
            {
                return RedirectToAction("Index", "ThongKe");
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: /TaiKhoan/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Bạn đã đăng xuất khỏi hệ thống.";
            return RedirectToAction("Login");
        }

        // GET: /TaiKhoan/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // POST: /TaiKhoan/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == model.TenDangNhap))
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập này đã tồn tại, vui lòng chọn tên khác.");
            }

            if (await _context.TaiKhoans.AnyAsync(t => t.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng trong hệ thống.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = model.TenDangNhap.Trim(),
                MatKhau = model.MatKhau,
                HoTen = model.HoTen.Trim(),
                Email = model.Email.Trim(),
                VaiTro = "HocVien",
                TrangThai = true
            };

            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            var hocVien = new HocVien
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan,
                HoTen = model.HoTen.Trim(),
                Email = model.Email.Trim(),
                SoDienThoai = model.SoDienThoai.Trim(),
                NgaySinh = model.NgaySinh,
                GioiTinh = model.GioiTinh,
                DiaChi = model.DiaChi,
                TrinhDo = "Chưa cập nhật",
                NgayDangKy = DateTime.Now,
                TrangThai = true
            };

            _context.HocViens.Add(hocVien);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đăng ký tài khoản học viên thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        // GET: /TaiKhoan/Profile
        [RoleAuthorize]
        public async Task<IActionResult> Profile()
        {
            int maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan")!.Value;
            var taiKhoan = await _context.TaiKhoans.FindAsync(maTaiKhoan);
            if (taiKhoan == null) return NotFound();
            return View(taiKhoan);
        }

        // POST: /TaiKhoan/Profile (Cập nhật thông tin tài khoản & mật khẩu của chính mình)
        [HttpPost]
        [RoleAuthorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(string hoTen, string email, string? matKhauMoi)
        {
            int maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan")!.Value;
            var taiKhoan = await _context.TaiKhoans.Include(t => t.HocVien).FirstOrDefaultAsync(t => t.MaTaiKhoan == maTaiKhoan);
            if (taiKhoan == null) return NotFound();

            if (string.IsNullOrWhiteSpace(hoTen) || string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Họ tên và Email không được để trống.";
                return View(taiKhoan);
            }

            taiKhoan.HoTen = hoTen.Trim();
            taiKhoan.Email = email.Trim();

            if (!string.IsNullOrWhiteSpace(matKhauMoi))
            {
                if (matKhauMoi.Length < 6)
                {
                    TempData["Error"] = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                    return View(taiKhoan);
                }
                taiKhoan.MatKhau = matKhauMoi;
            }

            if (taiKhoan.HocVien != null)
            {
                taiKhoan.HocVien.HoTen = taiKhoan.HoTen;
                taiKhoan.HocVien.Email = taiKhoan.Email;
            }

            await _context.SaveChangesAsync();
            HttpContext.Session.SetString("HoTen", taiKhoan.HoTen);
            TempData["Success"] = "Cập nhật thông tin tài khoản thành công!";
            return RedirectToAction("Profile");
        }

        // GET: /TaiKhoan/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }

        // ============================================================
        // CHỨC NĂNG QUẢN LÝ TÀI KHOẢN DÀNH CHO ADMIN
        // ============================================================

        [RoleAuthorize("Admin")]
        public async Task<IActionResult> Index(string? search, string? vaiTro)
        {
            var query = _context.TaiKhoans.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t => t.TenDangNhap.Contains(search) || t.HoTen.Contains(search) || t.Email.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(vaiTro))
            {
                query = query.Where(t => t.VaiTro == vaiTro);
            }

            ViewBag.Search = search;
            ViewBag.VaiTro = vaiTro;

            var list = await query.OrderBy(t => t.VaiTro).ThenBy(t => t.TenDangNhap).ToListAsync();
            return View(list);
        }

        [RoleAuthorize("Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new TaiKhoan());
        }

        [RoleAuthorize("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaiKhoan model)
        {
            if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == model.TenDangNhap))
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại trong hệ thống.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _context.TaiKhoans.Add(model);
            await _context.SaveChangesAsync();

            // Nếu tạo tài khoản có vai trò Học viên thì tự động khởi tạo hồ sơ Học viên đi kèm
            if (model.VaiTro == "HocVien")
            {
                var hv = new HocVien
                {
                    MaTaiKhoan = model.MaTaiKhoan,
                    HoTen = model.HoTen,
                    Email = model.Email,
                    SoDienThoai = "0900000000",
                    NgaySinh = new DateTime(2004, 1, 1),
                    GioiTinh = "Khác",
                    TrangThai = model.TrangThai
                };
                _context.HocViens.Add(hv);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = $"Đã tạo tài khoản {model.TenDangNhap} thành công!";
            return RedirectToAction(nameof(Index));
        }

        [RoleAuthorize("Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var tk = await _context.TaiKhoans.FindAsync(id);
            if (tk == null) return NotFound();
            return View(tk);
        }

        [RoleAuthorize("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaiKhoan model)
        {
            if (id != model.MaTaiKhoan) return NotFound();

            if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == model.TenDangNhap && t.MaTaiKhoan != id))
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã bị trùng với tài khoản khác.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _context.Update(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật tài khoản thành công!";
            return RedirectToAction(nameof(Index));
        }

        [RoleAuthorize("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            int currentAdminId = HttpContext.Session.GetInt32("MaTaiKhoan")!.Value;
            if (id == currentAdminId)
            {
                TempData["Error"] = "Bạn không thể tự khóa tài khoản của chính mình!";
                return RedirectToAction(nameof(Index));
            }

            var tk = await _context.TaiKhoans.FindAsync(id);
            if (tk == null) return NotFound();

            tk.TrangThai = !tk.TrangThai;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã {(tk.TrangThai ? "mở khóa" : "khóa")} tài khoản {tk.TenDangNhap}.";
            return RedirectToAction(nameof(Index));
        }
    }
}




