using System.ComponentModel.DataAnnotations;

namespace ASM_CS4.Models
{
    public class CartDetail
    {
        [Key]
        public string MaGioHang { get; set; }
        public string MaSanPham { get; set; }
        public int SoLuongSanPhamMua { get; set; }
        public string MaKhachHang { get; set; }
        public decimal Gia { get; set; }
		public int MaChiTiet { get; set; }


		// Quan hệ với bảng Product (N-1)
		public Product Product { get; set; }

        // Quan hệ với bảng Cart (N-1)
        public Cart Cart { get; set; }

    }
}
