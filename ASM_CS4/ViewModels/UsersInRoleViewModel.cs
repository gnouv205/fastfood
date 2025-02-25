namespace ASM_CS4.ViewModels
{
    public class UsersInRoleViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<UserViewModel> Users { get; set; }
    }
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
    }
}
