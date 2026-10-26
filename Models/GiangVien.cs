// Họ và tên: Nguyễn Duy Trung
// Mã sinh viên: 23103100256
// Nội dung thực hiện: Module 1 - Quản lý Giảng viên (Dữ liệu nền)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc.Models
{
    [Table("GiangVien")]
    public class GiangVien
    {
        [Key]
        [Display(Name = "Mã giảng viên")]
        public int MaGiangVien { get; set; }

        [Required(ErrorMessage = "Họ tên giảng viên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Chuyên môn không được để trống")]
        [StringLength(150, ErrorMessage = "Chuyên môn tối đa 150 ký tự")]
        [Display(Name = "Chuyên môn")]
        public string ChuyenMon { get; set; } = string.Empty;

        [Display(Name = "Đang giảng dạy")]
        public bool TrangThai { get; set; } = true;

        // Navigation Property: 1 GiangVien - N KhoaHoc
        public virtual ICollection<KhoaHoc> KhoaHocs { get; set; } = new List<KhoaHoc>();
    }
}