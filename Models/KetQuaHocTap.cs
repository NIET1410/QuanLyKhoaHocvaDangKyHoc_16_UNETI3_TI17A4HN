// Họ và tên: Nguyễn Minh Tiến
// Mã sinh viên: 23103100298
// Nội dung thực hiện: Module 5 - Quản lý Kết quả học tập, Dashboard & Thống kê

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKhoaHoc.Models
{
    [Table("KetQuaHocTap")]
    public class KetQuaHocTap
    {
        [Key]
        [Display(Name = "Mã kết quả")]
        public int MaKetQua { get; set; }

        [Required]
        [Display(Name = "Mã đăng ký")]
        public int MaDangKy { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập điểm chuyên cần")]
        [Range(0.0, 10.0, ErrorMessage = "Điểm chuyên cần phải từ 0 đến 10")]
        [Display(Name = "Điểm chuyên cần (10%)")]
        public double DiemChuyenCan { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập điểm giữa kỳ")]
        [Range(0.0, 10.0, ErrorMessage = "Điểm giữa kỳ phải từ 0 đến 10")]
        [Display(Name = "Điểm giữa kỳ (30%)")]
        public double DiemGiuaKy { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập điểm cuối kỳ")]
        [Range(0.0, 10.0, ErrorMessage = "Điểm cuối kỳ phải từ 0 đến 10")]
        [Display(Name = "Điểm cuối kỳ (60%)")]
        public double DiemCuoiKy { get; set; }

        [Display(Name = "Điểm tổng kết")]
        public double DiemTongKet { get; set; }

        [StringLength(30)]
        [Display(Name = "Xếp loại")]
        public string XepLoai { get; set; } = string.Empty; // Xuất sắc, Giỏi, Khá, Trung bình, Yếu

        [StringLength(30)]
        [Display(Name = "Kết quả")]
        public string KetQua { get; set; } = string.Empty; // Đạt, Không đạt

        [Display(Name = "Ngày cập nhật")]
        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        // Foreign Key 1 - 0..1 với DangKyKhoaHoc
        [ForeignKey("MaDangKy")]
        public virtual DangKyKhoaHoc? DangKyKhoaHoc { get; set; }

        // Hàm hỗ trợ tự động tính điểm tổng kết, xếp loại và kết quả
        public void TinhKetQuaTuDong()
        {
            DiemTongKet = Math.Round(DiemChuyenCan * 0.1 + DiemGiuaKy * 0.3 + DiemCuoiKy * 0.6, 2);

            if (DiemTongKet >= 8.5) XepLoai = "Xuất sắc";
            else if (DiemTongKet >= 7.0) XepLoai = "Giỏi";
            else if (DiemTongKet >= 5.5) XepLoai = "Khá";
            else if (DiemTongKet >= 4.0) XepLoai = "Trung bình";
            else XepLoai = "Yếu";

            KetQua = DiemTongKet >= 5.0 ? "Đạt" : "Không đạt";
            NgayCapNhat = DateTime.Now;
        }
    }
}