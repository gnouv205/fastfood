using ASM_CS4.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace ASM_CS4.Models
{
	public class Customer : IdentityUser
	{
		[Required]
		public string MaKhachHang { get; set; } = "KH" + Guid.NewGuid().ToString("N").Substring(0, 8);

		[Required]
		public string TenKhachHang { get; set; }

		[Required]
		[EmailAddress]
		public override string Email { get; set; }

		public string? DiaChi { get; set; }

		public DateTime CreatedDate { get; set; } = DateTime.Now;

		public ICollection<Cart> Carts { get; set; }
	}
}
