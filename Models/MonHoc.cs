// Họ và tên: Nguyễn Duy Trung
// Mã sinh viên: 23103100256
// Nội dung thực hiện: Module 1 - Quản lý Môn học

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc.Models
{
    [Table("MonHoc")]
    public class MonHoc
    {
        [Key]
        [Display(Name = "Mã môn học")]
        public int MaMonHoc { get; set; }

        [Required(ErrorMessage = "Tên môn học không được để trống")]
        [StringLength(150, ErrorMessage = "Tên môn học tối đa 150 ký tự")]
        [Display(Name = "Tên môn học")]
        public string TenMonHoc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số tín chỉ không được để trống")]
        [Range(1, 15, ErrorMessage = "Số tín chỉ phải từ 1 đến 15")]
        [Display(Name = "Số tín chỉ")]
        public int SoTinChi { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự")]
        [Display(Name = "Mô tả môn học")]
        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Học phí niêm yết không được để trống")]
        [Range(0, 100000000, ErrorMessage = "Học phí phải từ 0 VNĐ trở lên")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Học phí chuẩn (VNĐ)")]
        public decimal HocPhi { get; set; }

        [Display(Name = "Đang áp dụng")]
        public bool TrangThai { get; set; } = true;

        // Navigation Property: 1 MonHoc - N KhoaHoc
        public virtual ICollection<KhoaHoc> KhoaHocs { get; set; } = new List<KhoaHoc>();
    }
}