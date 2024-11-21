namespace ASM_CS4.Models
{
    public class Cart
    {
        public string MaGioHang { get; set; }
        public string MaKhachHang { get; set; }
        public int SoLuong { get; set; }

        public decimal TongTien { get; set; }
        public DateTime CreatedDate { get; set; }
        public string MaSanPham { get; set; }

        // Quan hệ với bảng Customer (N-1)
        public Customer Customer { get; set; }

        public Product Product { get; set; }

        // Quan hệ với bảng CartDetails (1-N)
        public ICollection<CartDetail> CartDetails { get; set; }
    }
}
