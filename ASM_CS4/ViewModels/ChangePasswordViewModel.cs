using System.ComponentModel.DataAnnotations;

namespace ASM_CS4.ViewModels
{
	public class ChangePasswordViewModel
	{
		[Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại.")]
		[DataType(DataType.Password)]
		[Display(Name = "Mật khẩu hiện tại")]
		public string CurrentPassword { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
		[DataType(DataType.Password)]
		[Display(Name = "Mật khẩu mới")]
		[MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
		public string NewPassword { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập lại mật khẩu mới.")]
		[DataType(DataType.Password)]
		[Display(Name = "Nhập lại mật khẩu mới")]
		[Compare("NewPassword", ErrorMessage = "Mật khẩu mới không khớp.")]
		public string ConfirmNewPassword { get; set; }
	}
}
