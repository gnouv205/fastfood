namespace ASM_CS4.Models
{
    public class CartDetail
    {
        public int MaChiTiet { get; set; }
        public string MaGioHang { get; set; }
        public string MaSanPham { get; set; }
        public int SoLuongSanPhamMua { get; set; }
        public string MaKhachHang { get; set; }
        public decimal Gia { get; set; }

        // Quan hệ với bảng Product (N-1)
        public Product Product { get; set; }

        // Quan hệ với bảng Cart (N-1)
        public Cart Cart { get; set; }

    }
}
