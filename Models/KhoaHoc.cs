// Họ và tên: Phan Anh Tú
// Mã sinh viên: 23103100258
// Nội dung thực hiện: Module 2 - Quản lý Khóa học

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc.Models
{
    [Table("KhoaHoc")]
    public class KhoaHoc
    {
        [Key]
        [Display(Name = "Mã khóa học")]
        public int MaKhoaHoc { get; set; }

        [Required(ErrorMessage = "Tên khóa học không được để trống")]
        [StringLength(200, ErrorMessage = "Tên khóa học tối đa 200 ký tự")]
        [Display(Name = "Tên khóa học")]
        public string TenKhoaHoc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn môn học")]
        [Display(Name = "Môn học")]
        public int MaMonHoc { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giảng viên")]
        [Display(Name = "Giảng viên")]
        public int MaGiangVien { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime NgayBatDau { get; set; } = DateTime.Today.AddDays(7);

        [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày kết thúc")]
        public DateTime NgayKetThuc { get; set; } = DateTime.Today.AddMonths(2);

        [Required(ErrorMessage = "Số lượng tối đa không được để trống")]
        [Range(1, 500, ErrorMessage = "Số lượng tối đa phải từ 1 đến 500 học viên")]
        [Display(Name = "Sĩ số tối đa")]
        public int SoLuongToiDa { get; set; } = 30;

        [Required(ErrorMessage = "Học phí không được để trống")]
        [Range(0, 100000000, ErrorMessage = "Học phí không được âm")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Học phí (VNĐ)")]
        public decimal HocPhi { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hình thức học")]
        [StringLength(30)]
        [Display(Name = "Hình thức")]
        public string HinhThuc { get; set; } = "Offline"; // Online, Offline, Hybrid

        [Required(ErrorMessage = "Vui lòng chọn trạng thái khóa học")]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Đang mở";
        // Chưa mở, Đang mở, Đã đầy, Đang học, Đã kết thúc, Đã hủy

        [StringLength(1500, ErrorMessage = "Mô tả tối đa 1500 ký tự")]
        [Display(Name = "Mô tả chi tiết")]
        public string? MoTa { get; set; }

        // Foreign Keys & Navigation Properties
        [ForeignKey("MaMonHoc")]
        public virtual MonHoc? MonHoc { get; set; }

        [ForeignKey("MaGiangVien")]
        public virtual GiangVien? GiangVien { get; set; }

        public virtual ICollection<DangKyKhoaHoc> DangKyKhoaHocs { get; set; } = new List<DangKyKhoaHoc>();

        // Thuộc tính tính toán nhanh (không lưu vào DB) để lấy số lượng đã đăng ký hợp lệ (không tính Đã hủy / Từ chối)
        [NotMapped]
        [Display(Name = "Đã đăng ký")]
        public int SoLuongDaDangKy => DangKyKhoaHocs?.Count(d => d.TrangThai != "Hủy" && d.TrangThai != "Từ chối") ?? 0;

        [NotMapped]
        [Display(Name = "Số chỗ còn lại")]
        public int SoChoConLai => Math.Max(0, SoLuongToiDa - SoLuongDaDangKy);
    }
}