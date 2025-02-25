using System.ComponentModel.DataAnnotations;

namespace ASM_CS4.ViewModels
{
	public class LoginViewModel
	{
        [Required(ErrorMessage = "Vui lòng nhập Email hoặc Username.")]
        public string UserIdentifier { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập password")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Display(Name = "Remember me?")]
		public bool RememberMe { get; set; }
	}
}
