namespace ASM_CS4.Models
{
    public class Product
    {
        public string MaSanPham { get; set; } = string.Empty;
        public string HinhSanPham { get; set; } = string.Empty;
        public string TenSanPham { get; set; } = string.Empty;
        public decimal GiaSanPham { get; set; }
        public int SoLuongSanPham { get; set; }
        public string MoTaSanPham { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public string MaDanhMuc { get; set; } = string.Empty;

        // Quan hệ với bảng Category (N-1)
        public Category? Category { get; set; } 

        // Quan hệ với bảng CartDetails (1-N)
        public ICollection<CartDetail>? CartDetails { get; set; }
    }
}
