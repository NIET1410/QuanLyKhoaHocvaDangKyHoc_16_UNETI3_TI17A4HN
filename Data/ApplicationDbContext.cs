// Họ và tên: Nguyễn Duy Trung
// Mã sinh viên: 23103100256
// Nội dung thực hiện: Thiết kế ApplicationDbContext và cấu hình quan hệ Database

using Microsoft.EntityFrameworkCore;
using QuanLyKhoaHoc.Models;

namespace QuanLyKhoaHoc.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<MonHoc> MonHocs { get; set; }
        public DbSet<GiangVien> GiangViens { get; set; }
        public DbSet<KhoaHoc> KhoaHocs { get; set; }
        public DbSet<HocVien> HocViens { get; set; }
        public DbSet<DangKyKhoaHoc> DangKyKhoaHocs { get; set; }
        public DbSet<KetQuaHocTap> KetQuaHocTaps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. TaiKhoan: TenDangNhap không được trùng
            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.TenDangNhap)
                .IsUnique();

            // 2. TaiKhoan 1 - 1 HocVien
            modelBuilder.Entity<TaiKhoan>()
                .HasOne(t => t.HocVien)
                .WithOne(h => h.TaiKhoan)
                .HasForeignKey<HocVien>(h => h.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. MonHoc 1 - N KhoaHoc (Không cho xóa MonHoc nếu đã có KhoaHoc)
            modelBuilder.Entity<MonHoc>()
                .HasMany(m => m.KhoaHocs)
                .WithOne(k => k.MonHoc)
                .HasForeignKey(k => k.MaMonHoc)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. GiangVien 1 - N KhoaHoc (Không cho xóa GiangVien nếu đã có KhoaHoc)
            modelBuilder.Entity<GiangVien>()
                .HasMany(g => g.KhoaHocs)
                .WithOne(k => k.GiangVien)
                .HasForeignKey(k => k.MaGiangVien)
                .OnDelete(DeleteBehavior.Restrict);

            // 5. HocVien 1 - N DangKyKhoaHoc
            modelBuilder.Entity<HocVien>()
                .HasMany(h => h.DangKyKhoaHocs)
                .WithOne(d => d.HocVien)
                .HasForeignKey(d => d.MaHocVien)
                .OnDelete(DeleteBehavior.Restrict);

            // 6. KhoaHoc 1 - N DangKyKhoaHoc
            modelBuilder.Entity<KhoaHoc>()
                .HasMany(k => k.DangKyKhoaHocs)
                .WithOne(d => d.KhoaHoc)
                .HasForeignKey(d => d.MaKhoaHoc)
                .OnDelete(DeleteBehavior.Restrict);

            // 7. DangKyKhoaHoc 1 - 0..1 KetQuaHocTap
            modelBuilder.Entity<DangKyKhoaHoc>()
                .HasOne(d => d.KetQuaHocTap)
                .WithOne(kq => kq.DangKyKhoaHoc)
                .HasForeignKey<KetQuaHocTap>(kq => kq.MaDangKy)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}