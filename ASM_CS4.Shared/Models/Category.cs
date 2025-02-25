

using System.ComponentModel.DataAnnotations;

namespace ASM_CS4.Models
{
    public class Category
    {
        [Key]
        public string MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; }

        // Quan hệ với bảng Products (1-N)
        public ICollection<Product> Products { get; set; }
    }
}
