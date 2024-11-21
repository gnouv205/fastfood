using System.ComponentModel.DataAnnotations;

namespace ASM_CS4.Models
{
    public class Customer
    {
        public string MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string DienThoai { get; set; }
		[Required(ErrorMessage = "Tên đăng nhập không được để trống.")]
		[EmailAddress(ErrorMessage = "Email không hợp lệ.")]
		public string Email { get; set; }
        public string DiaChi { get; set; }
		[Required(ErrorMessage = "Mật khẩu không được để trống.")]
		[StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.", MinimumLength = 6)]
		public string MatKhau { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Quan hệ với bảng Carts (1-N)
        public ICollection<Cart> Carts { get; set; }
    }
}
