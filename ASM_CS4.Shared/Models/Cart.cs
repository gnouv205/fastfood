using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASM_CS4.Models
{
	public class Cart
	{
		[Key] // Xác định MaGioHang là khóa chính
		public string MaGioHang { get; set; } = Guid.NewGuid().ToString();

		[Required] // MaKhachHang không được để trống
		public string MaKhachHang { get; set; }

		public int SoLuong { get; set; }

		public decimal TongTien { get; set; }

		public DateTime CreatedDate { get; set; } = DateTime.Now;
		[Required]
		public string MaSanPham { get; set; }

		// Quan hệ với bảng Customer (N-1)
		public Customer Customer { get; set; }

		// Quan hệ với bảng Product (N-1)
		public Product Product { get; set; }

		// Quan hệ với bảng CartDetails (1-N)
		public ICollection<CartDetail> CartDetails { get; set; }
	}
}
