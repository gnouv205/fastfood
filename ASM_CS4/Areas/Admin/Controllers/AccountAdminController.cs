using ASM_CS4.Data;
using ASM_CS4.Models;
using ASM_CS4.ViewModels;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_CS4.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AccountAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Customer> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Customer> _signInManager;
        public AccountAdminController(ApplicationDbContext context, UserManager<Customer> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Customer> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        #region Lấy danh sách tài khoản người dùng
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var customers = await _context.Customers.ToListAsync();
            var customerList = new List<CustomerViewModel>();

            foreach (var customer in customers)
            {
                var user = await _userManager.FindByEmailAsync(customer.Email); 
                if (user == null)
                {
                    Console.WriteLine($"⚠ Không tìm thấy User với email: {customer.Email}");
                    continue;
                }

                var roles = await _userManager.GetRolesAsync(user);

                customerList.Add(new CustomerViewModel
                {
                    MaKhachHang = customer.Id,
                    AspNetUserId = user.Id, 
                    Email = customer.Email,
                    Role = roles.FirstOrDefault() ?? "Không có role"
                });
            }

            return View(customerList);
        }
        #endregion
        #region Quản lí Roles
        [HttpGet]
        public async Task<IActionResult> ManageRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }
        #endregion
        #region Create
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(RoleViewModel roleViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(roleViewModel);
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleViewModel.RoleName);
            if (!roleExists)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleViewModel.RoleName));
                if (result.Succeeded)
                {
                    TempData["Success"] = "Tạo vai trò thành công";
                }
                else
                {
                    TempData["Error"] = "Tạo vai trò không thành công";
                }
            }
            else
            {
                TempData["Error"] = "Vai trò này đã tồn tại";
            }
            return RedirectToAction("ManagerRoles", "AccountAdmin");
        }
        #endregion
        #region Edit
        [HttpGet]
        public async Task<IActionResult> EditRole(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Lỗi: Không nhận được UserId!";
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Người dùng không tồn tại!";
                return RedirectToAction("Index");
            }

            var model = new CustomerViewModel
            {
                MaKhachHang = user.Id, // ID trong bảng AspNetUsers
                Email = user.Email,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "Không có role",
                AspNetUserId = user.Id // Thêm dòng này để đảm bảo userId có dữ liệu
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(string userId, string newRole)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "Lỗi: Không nhận được UserId!";
                Console.WriteLine("Debug: userId bị null hoặc rỗng");
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Người dùng không tồn tại!";
                return RedirectToAction("Index");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, newRole);

            TempData["Success"] = "Cập nhật vai trò thành công!";
            return RedirectToAction("Index");
        }



        #endregion
        #region Delete
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                TempData["Error"] = "Vai trò không tồn tại!";
                return RedirectToAction("ManageRoles");
            }

            // Kiểm tra xem có người dùng nào đang sử dụng role này không
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            if (usersInRole.Any())
            {
                TempData["Error"] = $"Không thể xóa. Có {usersInRole.Count} người dùng đang sử dụng vai trò này!";
                return RedirectToAction("ManageRoles");
            }

            // Nếu không có ai sử dụng, tiến hành xóa
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["Success"] = "Xóa vai trò thành công!";
            }
            else
            {
                TempData["Error"] = "Không thể xóa vai trò này!";
            }

            return RedirectToAction("ManageRoles");
        }


        #endregion
        #region Kiểm tra số user trong roles 
        [HttpGet]
        public async Task<IActionResult> UsersInRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                TempData["Error"] = "Vai trò không tồn tại!";
                return RedirectToAction("ManageRoles");
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);

            var model = new UsersInRoleViewModel
            {
                RoleId = roleId,
                RoleName = role.Name,
                Users = usersInRole.Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Email = u.Email
                }).ToList()
            };

            return View(model);
        }

        #endregion
    }

}
