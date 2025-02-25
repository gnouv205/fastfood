using System.ComponentModel.DataAnnotations;

namespace ASM_CS4.ViewModels
{
	public class RoleViewModel
	{
        [Required(ErrorMessage = "Tên vai trò không được để trống!")]
        public string RoleName { get; set; }
    }
}
