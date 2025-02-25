using System.ComponentModel.DataAnnotations;

namespace ASM_CS4.ViewModels
{
	public class CustomerViewModel
	{
        public string MaKhachHang { get; set; }
        public string AspNetUserId { get; set; }


        [Required]
		public string Email { get; set; }

		public string PhoneNumber { get; set; }

		public string Role { get; set; } // Thêm thuộc tính Role
	}
}


