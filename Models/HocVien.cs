// Họ và tên: Tạ Xuân Thưng
// Mã sinh viên: 23103100286
// Nội dung thực hiện: Module 3 - Quản lý Học viên & Hồ sơ cá nhân

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc.Models
{
    [Table("HocVien")]
    public class HocVien
    {
        [Key]
        [Display(Name = "Mã học viên")]
        public int MaHocVien { get; set; }

        [Required]
        [Display(Name = "Tài khoản liên kết")]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ tên học viên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime NgaySinh { get; set; } = new DateTime(2004, 1, 1);

        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        [StringLength(10)]
        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; } = "Nam"; // Nam, Nữ, Khác

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Địa chỉ tối đa 250 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [StringLength(100, ErrorMessage = "Trình độ tối đa 100 ký tự")]
        [Display(Name = "Trình độ học vấn")]
        public string? TrinhDo { get; set; } = "Sinh viên Đại học";

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo hồ sơ")]
        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        [Display(Name = "Trạng thái hồ sơ")]
        public bool TrangThai { get; set; } = true;

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        // Foreign Key 1 - 1 với TaiKhoan
        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan? TaiKhoan { get; set; }

        // Navigation Property: 1 HocVien - N DangKyKhoaHoc
        public virtual ICollection<DangKyKhoaHoc> DangKyKhoaHocs { get; set; } = new List<DangKyKhoaHoc>();
    }
}