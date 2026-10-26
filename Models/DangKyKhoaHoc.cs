// Họ và tên: Tạ Xuân Thưng & Ngô Duy Thái
// Mã sinh viên: 23103100286, 23103100247
// Nội dung thực hiện: Module 3 & 4 - Đăng ký khóa học và Tiếp nhận xử lý đăng ký

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc.Models
{
    [Table("DangKyKhoaHoc")]
    public class DangKyKhoaHoc
    {
        [Key]
        [Display(Name = "Mã đăng ký")]
        public int MaDangKy { get; set; }

        [Required]
        [Display(Name = "Học viên")]
        public int MaHocVien { get; set; }

        [Required]
        [Display(Name = "Khóa học")]
        public int MaKhoaHoc { get; set; }

        [Display(Name = "Ngày đăng ký")]
        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Chờ xử lý";
        // Luồng: Chờ xử lý -> Đang xử lý -> Hoàn thành (Hoặc: Hủy, Từ chối)

        [Range(0, 100000000, ErrorMessage = "Số tiền đã đóng không được âm")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Số tiền đã đóng (VNĐ)")]
        public decimal SoTienDaDong { get; set; } = 0;

        [Range(0, 100000000, ErrorMessage = "Số tiền còn lại không được âm")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Công nợ còn lại (VNĐ)")]
        public decimal SoTienConLai { get; set; } = 0;

        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        [Display(Name = "Ghi chú xử lý")]
        public string? GhiChu { get; set; }

        [Display(Name = "Ngày xác nhận")]
        public DateTime? NgayXacNhan { get; set; }

        // Foreign Keys & Navigation Properties
        [ForeignKey("MaHocVien")]
        public virtual HocVien? HocVien { get; set; }

        [ForeignKey("MaKhoaHoc")]
        public virtual KhoaHoc? KhoaHoc { get; set; }

        // 1 DangKyKhoaHoc - 0..1 KetQuaHocTap
        public virtual KetQuaHocTap? KetQuaHocTap { get; set; }
    }
}