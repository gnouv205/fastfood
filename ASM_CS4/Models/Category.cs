

namespace ASM_CS4.Models
{
    public class Category
    {
        public string MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; }

        // Quan hệ với bảng Products (1-N)
        public ICollection<Product> Products { get; set; }
    }
}
