// Họ và tên: Nguyễn Duy Trung
// Mã sinh viên: 23103100256
// Nội dung thực hiện: Khởi tạo Seed Data đầy đủ cho toàn hệ thống

using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Models;

namespace QuanLyKhoaHoc.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.Migrate();

            if (context.TaiKhoans.Any())
            {
                return; // Đã có dữ liệu
            }

            // 1. TẠO TÀI KHOẢN ADMIN & NHÂN VIÊN
            var taiKhoans = new List<TaiKhoan>
            {
                new TaiKhoan { TenDangNhap = "admin", MatKhau = "Admin@123", HoTen = "Quản Trị Viên Hệ Thống", Email = "admin@edu.vn", VaiTro = "Admin", TrangThai = true },
                new TaiKhoan { TenDangNhap = "admin02", MatKhau = "Admin@123", HoTen = "Nguyễn Quản Trị", Email = "admin02@edu.vn", VaiTro = "Admin", TrangThai = true },
                new TaiKhoan { TenDangNhap = "staff01", MatKhau = "Staff@123", HoTen = "Trần Thị Thanh (NV Đào tạo)", Email = "staff01@edu.vn", VaiTro = "NhanVien", TrangThai = true },
                new TaiKhoan { TenDangNhap = "staff02", MatKhau = "Staff@123", HoTen = "Lê Hoàng Nam (NV Học vụ)", Email = "staff02@edu.vn", VaiTro = "NhanVien", TrangThai = true },
                new TaiKhoan { TenDangNhap = "staff03", MatKhau = "Staff@123", HoTen = "Phạm Minh Tuấn (NV Tư vấn)", Email = "staff03@edu.vn", VaiTro = "NhanVien", TrangThai = true }
            };

            // Tạo 30 tài khoản Học viên
            string[] ho = { "Nguyễn", "Trần", "Lê", "Phạm", "Hoàng", "Huỳnh", "Phan", "Vũ", "Võ", "Đặng" };
            string[] dem = { "Văn", "Thị", "Minh", "Hoàng", "Ngọc", "Quốc", "Gia", "Thanh", "Đức", "Phương" };
            string[] ten = { "An", "Bình", "Châu", "Dũng", "Em", "Giang", "Hải", "Khánh", "Linh", "My", "Nam", "Oanh", "Phúc", "Quân", "Tâm" };

            for (int i = 1; i <= 30; i++)
            {
                string hoTen = $"{ho[i % ho.Length]} {dem[(i * 2) % dem.Length]} {ten[i % ten.Length]}";
                taiKhoans.Add(new TaiKhoan
                {
                    TenDangNhap = $"student{i:D2}",
                    MatKhau = "Student@123",
                    HoTen = hoTen,
                    Email = $"student{i:D2}@sv.edu.vn",
                    VaiTro = "HocVien",
                    TrangThai = i != 30 // student30 bị khóa để test case tài khoản bị khóa
                });
            }

            context.TaiKhoans.AddRange(taiKhoans);
            context.SaveChanges();

            // 2. TẠO 30 HỒ SƠ HỌC VIÊN
            var tkHocViens = context.TaiKhoans.Where(t => t.VaiTro == "HocVien").OrderBy(t => t.MaTaiKhoan).ToList();
            var hocViens = new List<HocVien>();
            string[] diaChis = { "Quận 1, TP.HCM", "Quận 3, TP.HCM", "Cầu Giấy, Hà Nội", "Hải Châu, Đà Nẵng", "Ninh Kiều, Cần Thơ", "Thủ Đức, TP.HCM" };
            string[] trinhDos = { "Sinh viên Năm 1", "Sinh viên Năm 2", "Sinh viên Năm 3", "Sinh viên Năm 4", "Đã tốt nghiệp ĐH" };

            int idx = 1;
            foreach (var tk in tkHocViens)
            {
                hocViens.Add(new HocVien
                {
                    MaTaiKhoan = tk.MaTaiKhoan,
                    HoTen = tk.HoTen,
                    NgaySinh = new DateTime(2002 + (idx % 4), (idx % 12) + 1, (idx % 25) + 1),
                    GioiTinh = idx % 2 == 0 ? "Nữ" : "Nam",
                    SoDienThoai = $"09{idx:D2}1234{idx:D2}",
                    Email = tk.Email,
                    DiaChi = diaChis[idx % diaChis.Length],
                    TrinhDo = trinhDos[idx % trinhDos.Length],
                    NgayDangKy = DateTime.Now.AddDays(-60 + idx),
                    TrangThai = tk.TrangThai,
                    GhiChu = idx == 30 ? "Tài khoản học viên đang tạm khóa" : "Hồ sơ hợp lệ"
                });
                idx++;
            }
            context.HocViens.AddRange(hocViens);
            context.SaveChanges();

            // 3. TẠO 5 MÔN HỌC
            var monHocs = new List<MonHoc>
            {
                new MonHoc { TenMonHoc = "Lập trình Web với ASP.NET Core MVC", SoTinChi = 4, HocPhi = 3500000, MoTa = "Xây dựng ứng dụng web hoàn chỉnh với .NET 10, EF Core 10, SQL Server và Bootstrap 5.", TrangThai = true },
                new MonHoc { TenMonHoc = "Cơ sở dữ liệu & Hệ quản trị SQL Server", SoTinChi = 3, HocPhi = 2500000, MoTa = "Thiết kế ERD, chuẩn hóa dữ liệu, truy vấn T-SQL nâng cao và tối ưu hiệu năng.", TrangThai = true },
                new MonHoc { TenMonHoc = "Cấu trúc dữ liệu và Giải thuật với C#", SoTinChi = 4, HocPhi = 3000000, MoTa = "Nắm vững mảng, danh sách liên kết, cây nhị phân, đồ thị và thuật toán sắp xếp/tìm kiếm.", TrangThai = true },
                new MonHoc { TenMonHoc = "Phân tích và Thiết kế Hệ thống Thông tin", SoTinChi = 3, HocPhi = 2800000, MoTa = "Sử dụng UML, thiết kế Use Case, Sequence Diagram, Class Diagram và kiến trúc phần mềm.", TrangThai = true },
                new MonHoc { TenMonHoc = "Kiểm thử phần mềm (Software Testing)", SoTinChi = 3, HocPhi = 3200000, MoTa = "Viết Test Case, kiểm thử hộp đen, hộp trắng, kiểm thử tích hợp và quản lý lỗi.", TrangThai = true }
            };
            context.MonHocs.AddRange(monHocs);
            context.SaveChanges();

            // 4. TẠO 6 GIẢNG VIÊN
            var giangViens = new List<GiangVien>
            {
                new GiangVien { HoTen = "TS. Nguyễn Đình Công", Email = "congnd@edu.vn", SoDienThoai = "0901112221", ChuyenMon = "Công nghệ phần mềm & .NET Architecture", TrangThai = true },
                new GiangVien { HoTen = "ThS. Trần Minh Hoàng", Email = "hoangtm@edu.vn", SoDienThoai = "0901112222", ChuyenMon = "Hệ thống thông tin & Cơ sở dữ liệu", TrangThai = true },
                new GiangVien { HoTen = "ThS. Lê Thu Thảo", Email = "thaolt@edu.vn", SoDienThoai = "0901112223", ChuyenMon = "Khoa học máy tính & Giải thuật", TrangThai = true },
                new GiangVien { HoTen = "TS. Phạm Quốc Bảo", Email = "baopq@edu.vn", SoDienThoai = "0901112224", ChuyenMon = "Phân tích nghiệp vụ & Kiến trúc hệ thống", TrangThai = true },
                new GiangVien { HoTen = "ThS. Võ Hoàng Yến", Email = "yenvh@edu.vn", SoDienThoai = "0901112225", ChuyenMon = "Đảm bảo chất lượng & Kiểm thử phần mềm", TrangThai = true },
                new GiangVien { HoTen = "ThS. Đặng Văn Khoa", Email = "khoadv@edu.vn", SoDienThoai = "0901112226", ChuyenMon = "Phát triển ứng dụng Web Fullstack", TrangThai = true }
            };
            context.GiangViens.AddRange(giangViens);
            context.SaveChanges();

            // 5. TẠO 12 KHÓA HỌC
            var mh = context.MonHocs.ToList();
            var gv = context.GiangViens.ToList();

            var khoaHocs = new List<KhoaHoc>
            {
                new KhoaHoc { TenKhoaHoc = "ASP.NET Core 10 MVC - Khóa K01 (Tối 2-4-6)", MaMonHoc = mh[0].MaMonHoc, MaGiangVien = gv[0].MaGiangVien, NgayBatDau = DateTime.Today.AddDays(-45), NgayKetThuc = DateTime.Today.AddDays(-5), SoLuongToiDa = 25, HocPhi = 3500000, HinhThuc = "Offline", TrangThai = "Đã kết thúc", MoTa = "Lớp học trực tiếp tại phòng Lab 302." },
                new KhoaHoc { TenKhoaHoc = "ASP.NET Core 10 MVC - Khóa K02 (Tối 3-5-7)", MaMonHoc = mh[0].MaMonHoc, MaGiangVien = gv[5].MaGiangVien, NgayBatDau = DateTime.Today.AddDays(-15), NgayKetThuc = DateTime.Today.AddDays(45), SoLuongToiDa = 30, HocPhi = 3500000, HinhThuc = "Hybrid", TrangThai = "Đang học", MoTa = "Kết hợp học trực tiếp và qua Zoom." },
                new KhoaHoc { TenKhoaHoc = "ASP.NET Core 10 MVC - Khóa K03 (Chuyên sâu)", MaMonHoc = mh[0].MaMonHoc, MaGiangVien = gv[0].MaGiangVien, NgayBatDau = DateTime.Today.AddDays(10), NgayKetThuc = DateTime.Today.AddDays(70), SoLuongToiDa = 25, HocPhi = 3800000, HinhThuc = "Offline", TrangThai = "Đang mở", MoTa = "Làm đồ án thực tế ngay trong khóa học." },
                new KhoaHoc { TenKhoaHoc = "SQL Server Thực Chiến - Khóa K01", MaMonHoc = mh[1].MaMonHoc, MaGiangVien = gv[1].MaGiangVien, NgayBatDau = DateTime.Today.AddDays(-50), NgayKetThuc = DateTime.Today.AddDays(-10), SoLuongToiDa = 20, HocPhi = 2500000, HinhThuc = "Online", TrangThai = "Đã kết thúc", MoTa = "Thực hành tối ưu truy vấn SQL Server." },
                new KhoaHoc { TenKhoaHoc = "SQL Server Thực Chiến - Khóa K02", MaMonHoc = mh[1].MaMonHoc, MaGiangVien = gv[1].MaGiangVien, NgayBatDau = DateTime.Today.AddDays(7), NgayKetThuc = DateTime.Today.AddDays(60), SoLuongToiDa = 20, HocPhi = 2500000, HinhThuc = "Online", TrangThai = "Đang mở", MoTa = "Khóa học trực tuyến buổi tối." },
                new KhoaHoc { TenKhoaHoc = "Cấu trúc dữ liệu & Giải thuật C# - K01", MaMonHoc = mh[2].MaMonHoc, MaGiangVien = gv[2].MaGiangVien, NgayBatDau = DateTime.Today.AddDays(5), NgayKetThuc = DateTime.Today.AddDays(65), SoLuongToiDa = 5, HocPhi = 3000000, HinhThuc = "Offline", TrangThai = "Đã đầy", MoTa = "Lớp giới hạn 5 học viên kèm sát 1-1 (Dùng để test khóa học Đã đầy)." },
                new KhoaHoc { TenKhoaHoc = "Cấu trúc dữ liệu & Giải thuật C# - K02", MaMonHoc = mh[2].MaMonHoc, MaGiangVien = gv[2].MaGiangVien, NgayBatDau = DateTime.Today.AddDays(14), NgayKetThuc = DateTime.Today.AddDays(75), SoLuongToiDa = 30, HocPhi = 3000000, HinhThuc = "Hybrid", TrangThai = "Đang mở", MoTa = "Luyện giải thuật phỏng vấn doanh nghiệp." },
                new KhoaHoc { TenKhoaHoc = "Phân tích thiết kế UML & Hệ thống - K01", MaMonHoc = mh[3].MaMonHoc, MaGiangVien = gv[3].MaGiangVien, NgayBatDau = DateTime.Today.AddDays(12), NgayKetThuc = DateTime.Today.AddDays(65), SoLuongToiDa = 25, HocPhi = 2800000, HinhThuc = "Online", TrangThai = "Đang mở", MoTa = "Hướng dẫn viết tài liệu SRS và vẽ sơ đồ chuẩn." },
                new KhoaHoc { TenKhoaHoc = "Phân tích thiết kế Hệ thống - K02 (Sắp khai giảng)", MaMonHoc = mh[3].MaMonHoc, MaGiangVien = gv[3].MaGiangVien, NgayBatDau = DateTime.Today.AddDays(30), NgayKetThuc = DateTime.Today.AddDays(90), SoLuongToiDa = 25, HocPhi = 2800000, HinhThuc = "Offline", TrangThai = "Chưa mở", MoTa = "Dự kiến mở đăng ký vào tháng tới." },
                new KhoaHoc { TenKhoaHoc = "Software Testing Toàn diện - Khóa K01", MaMonHoc = mh[4].MaMonHoc, MaGiangVien = gv[4].MaGiangVien, NgayBatDau = DateTime.Today.AddDays(-20), NgayKetThuc = DateTime.Today.AddDays(35), SoLuongToiDa = 25, HocPhi = 3200000, HinhThuc = "Online", TrangThai = "Đang học", MoTa = "Thực hành viết Testcase & Bug Report." },
                new KhoaHoc { TenKhoaHoc = "Software Testing Toàn diện - Khóa K02", MaMonHoc = mh[4].MaMonHoc, MaGiangVien = gv[4].MaGiangVien, NgayBatDau = DateTime.Today.AddDays(9), NgayKetThuc = DateTime.Today.AddDays(69), SoLuongToiDa = 25, HocPhi = 3200000, HinhThuc = "Offline", TrangThai = "Đang mở", MoTa = "Đào tạo Tester từ cơ bản đến nâng cao." },
                new KhoaHoc { TenKhoaHoc = "Khóa học Lập trình Web Cấp tốc (Lớp hủy)", MaMonHoc = mh[0].MaMonHoc, MaGiangVien = gv[5].MaGiangVien, NgayBatDau = DateTime.Today.AddDays(20), NgayKetThuc = DateTime.Today.AddDays(50), SoLuongToiDa = 20, HocPhi = 3500000, HinhThuc = "Offline", TrangThai = "Đã hủy", MoTa = "Đã hủy do thay đổi lịch công tác giảng viên." }
            };
            context.KhoaHocs.AddRange(khoaHocs);
            context.SaveChanges();

            // 6. TẠO 45 ĐĂNG KÝ KHÓA HỌC
            var dsHocVien = context.HocViens.OrderBy(h => h.MaHocVien).ToList();
            var dsKhoaHoc = context.KhoaHocs.OrderBy(k => k.MaKhoaHoc).ToList();
            var dangKys = new List<DangKyKhoaHoc>();

            // 6.1: 10 đăng ký đã Hoàn thành ở Khóa 1 (ASP.NET K01 - Đã kết thúc)
            for (int i = 0; i < 10; i++)
            {
                dangKys.Add(new DangKyKhoaHoc
                {
                    MaHocVien = dsHocVien[i].MaHocVien,
                    MaKhoaHoc = dsKhoaHoc[0].MaKhoaHoc,
                    NgayDangKy = DateTime.Now.AddDays(-50 + i),
                    TrangThai = "Hoàn thành",
                    SoTienDaDong = dsKhoaHoc[0].HocPhi,
                    SoTienConLai = 0,
                    NgayXacNhan = DateTime.Now.AddDays(-48 + i),
                    GhiChu = "Đã hoàn tất học phí và xác nhận vào lớp"
                });
            }

            // 6.2: 8 đăng ký Hoàn thành ở Khóa 4 (SQL K01 - Đã kết thúc)
            for (int i = 5; i < 13; i++)
            {
                dangKys.Add(new DangKyKhoaHoc
                {
                    MaHocVien = dsHocVien[i].MaHocVien,
                    MaKhoaHoc = dsKhoaHoc[3].MaKhoaHoc,
                    NgayDangKy = DateTime.Now.AddDays(-55 + i),
                    TrangThai = "Hoàn thành",
                    SoTienDaDong = dsKhoaHoc[3].HocPhi,
                    SoTienConLai = 0,
                    NgayXacNhan = DateTime.Now.AddDays(-52 + i),
                    GhiChu = "Đủ điều kiện dự thi"
                });
            }

            // 6.3: 5 đăng ký lấp đầy Khóa 6 (CTDL K01 - Sĩ số tối đa = 5 -> Đã đầy)
            for (int i = 0; i < 5; i++)
            {
                dangKys.Add(new DangKyKhoaHoc
                {
                    MaHocVien = dsHocVien[i + 10].MaHocVien,
                    MaKhoaHoc = dsKhoaHoc[5].MaKhoaHoc,
                    NgayDangKy = DateTime.Now.AddDays(-10 + i),
                    TrangThai = "Hoàn thành",
                    SoTienDaDong = 2000000,
                    SoTienConLai = 1000000,
                    NgayXacNhan = DateTime.Now.AddDays(-8 + i),
                    GhiChu = "Đóng trước 2 triệu, còn nợ 1 triệu"
                });
            }

            // 6.4: 8 đăng ký ở trạng thái "Đang xử lý" ở Khóa 3 (ASP.NET K03 - Đang mở)
            for (int i = 0; i < 8; i++)
            {
                dangKys.Add(new DangKyKhoaHoc
                {
                    MaHocVien = dsHocVien[i + 15].MaHocVien,
                    MaKhoaHoc = dsKhoaHoc[2].MaKhoaHoc,
                    NgayDangKy = DateTime.Now.AddDays(-7 + (i % 5)),
                    TrangThai = "Đang xử lý",
                    SoTienDaDong = 1500000,
                    SoTienConLai = dsKhoaHoc[2].HocPhi - 1500000,
                    GhiChu = "Đang chờ kế toán đối soát chuyển khoản"
                });
            }

            // 6.5: 10 đăng ký ở trạng thái "Chờ xử lý" ở Khóa 5 (SQL K02) & Khóa 7 (CTDL K02)
            for (int i = 0; i < 10; i++)
            {
                var kh = (i % 2 == 0) ? dsKhoaHoc[4] : dsKhoaHoc[6];
                dangKys.Add(new DangKyKhoaHoc
                {
                    MaHocVien = dsHocVien[i + 18].MaHocVien,
                    MaKhoaHoc = kh.MaKhoaHoc,
                    NgayDangKy = DateTime.Now.AddDays(-3 + (i % 3)),
                    TrangThai = "Chờ xử lý",
                    SoTienDaDong = 0,
                    SoTienConLai = kh.HocPhi,
                    GhiChu = "Học viên đăng ký trực tuyến trên website"
                });
            }

            // 6.6: 4 đăng ký ở trạng thái "Hủy" và "Từ chối"
            dangKys.Add(new DangKyKhoaHoc
            {
                MaHocVien = dsHocVien[0].MaHocVien,
                MaKhoaHoc = dsKhoaHoc[7].MaKhoaHoc,
                NgayDangKy = DateTime.Now.AddDays(-12),
                TrangThai = "Hủy",
                SoTienDaDong = 0,
                SoTienConLai = 0,
                GhiChu = "Học viên xin hủy do trùng lịch học ở trường"
            });
            dangKys.Add(new DangKyKhoaHoc
            {
                MaHocVien = dsHocVien[1].MaHocVien,
                MaKhoaHoc = dsKhoaHoc[7].MaKhoaHoc,
                NgayDangKy = DateTime.Now.AddDays(-11),
                TrangThai = "Từ chối",
                SoTienDaDong = 0,
                SoTienConLai = 0,
                GhiChu = "Từ chối do quá hạn đóng học phí giữ chỗ"
            });
            dangKys.Add(new DangKyKhoaHoc
            {
                MaHocVien = dsHocVien[2].MaHocVien,
                MaKhoaHoc = dsKhoaHoc[10].MaKhoaHoc,
                NgayDangKy = DateTime.Now.AddDays(-2),
                TrangThai = "Chờ xử lý",
                SoTienDaDong = 0,
                SoTienConLai = dsKhoaHoc[10].HocPhi,
                GhiChu = "Đăng ký mới chờ duyệt"
            });
            dangKys.Add(new DangKyKhoaHoc
            {
                MaHocVien = dsHocVien[3].MaHocVien,
                MaKhoaHoc = dsKhoaHoc[10].MaKhoaHoc,
                NgayDangKy = DateTime.Now.AddDays(-1),
                TrangThai = "Hoàn thành",
                SoTienDaDong = dsKhoaHoc[10].HocPhi,
                SoTienConLai = 0,
                NgayXacNhan = DateTime.Now,
                GhiChu = "Đã đóng đủ 100% học phí"
            });

            context.DangKyKhoaHocs.AddRange(dangKys);
            context.SaveChanges();

            // 7. TẠO 18 KẾT QUẢ HỌC TẬP (Cho 18 đăng ký "Hoàn thành" ở mục 6.1 và 6.2)
            var dkHoanThanh = context.DangKyKhoaHocs
                .Where(d => d.TrangThai == "Hoàn thành")
                .OrderBy(d => d.MaDangKy)
                .Take(18)
                .ToList();

            var ketQuas = new List<KetQuaHocTap>();
            double[] diemCC = { 9.0, 8.5, 10.0, 7.5, 8.0, 6.0, 9.5, 5.0, 8.0, 4.0, 9.0, 8.5, 7.0, 6.5, 9.0, 3.5, 8.0, 10.0 };
            double[] diemGK = { 8.5, 7.5, 9.0, 6.5, 7.0, 5.0, 8.5, 4.5, 7.5, 3.5, 8.0, 8.0, 6.5, 5.5, 9.0, 4.0, 7.0, 9.5 };
            double[] diemCK = { 9.0, 8.0, 9.5, 7.0, 6.5, 4.0, 9.0, 4.0, 8.0, 3.0, 8.5, 7.5, 6.0, 5.0, 8.5, 3.0, 7.5, 9.0 };

            for (int i = 0; i < dkHoanThanh.Count; i++)
            {
                var kq = new KetQuaHocTap
                {
                    MaDangKy = dkHoanThanh[i].MaDangKy,
                    DiemChuyenCan = diemCC[i],
                    DiemGiuaKy = diemGK[i],
                    DiemCuoiKy = diemCK[i]
                };
                kq.TinhKetQuaTuDong();
                ketQuas.Add(kq);
            }

            context.KetQuaHocTaps.AddRange(ketQuas);
            context.SaveChanges();
        }
    }
}