using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ASM_CS4.Models
{
    public class ChiTietDonDatHang
    {
        [Key]
        public string MaChiTiet { get; set; }        // Khóa chính

        [Required(ErrorMessage = "Đơn đặt hàng là bắt buộc.")]
        public string MaKhachHang { get; set; }     // Khóa ngoại, liên kết đến đơn đặt hàng

        [Required(ErrorMessage = "Sản phẩm là bắt buộc.")]
        public string MaSanPham { get; set; }        // Khóa ngoại, liên kết đến sản phẩm

        [Required(ErrorMessage = "Số lượng là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int SoLuong { get; set; }          // Số lượng sản phẩm đặt

        [Required(ErrorMessage = "Giá là bắt buộc.")]
        public decimal Gia { get; set; }          // Giá sản phẩm

        [Required(ErrorMessage = "Ngày giao hàng là bắt buộc.")]
        public DateTime? NgayGiao { get; set; } = DateTime.Now;     // Ngày giao

        [Required(ErrorMessage = "Ngày nhan hàng là bắt buộc.")]
        public DateTime? NgayNhan { get; set; } = DateTime.Now;     //Ngay nhan

        [Required(ErrorMessage = "Ngày thanh toan hàng là bắt buộc.")]
        public DateTime? NgayThanhToan { get; set; } = DateTime.Now;     // Ngày đặt hàng

        public string TrangThai { get; set; } = "Đang xử lý";

        [ForeignKey("MaKhachHang")]
        public virtual Customer? Customer { get; set; } // Điều hướng đến KhachHang

        [ForeignKey("MaSanPham")]
        public virtual Product? Product { get; set; } // Điều hướng đến SanPham

    }
}
