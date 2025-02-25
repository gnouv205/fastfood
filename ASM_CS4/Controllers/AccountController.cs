using ASM_CS4.Models;
using ASM_CS4.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ASM_CS4.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Customer> _signInManager;
        private readonly UserManager<Customer> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;

        public AccountController(SignInManager<Customer> signInManager, UserManager<Customer> userManager, RoleManager<IdentityRole> roleManager, IEmailSender emailSender)
        {
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._roleManager = roleManager;
			this._emailSender = emailSender;
        }


        #region Đăng nhập
        [HttpGet]
		public IActionResult Login()
        {
            return View();
        }

		[HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel loginViewModel)
		{
			//Console.WriteLine($"Bắt đầu đăng nhập với: {loginViewModel.UserIdentifier}");

			var user = await _userManager.FindByEmailAsync(loginViewModel.UserIdentifier)
					 ?? await _userManager.FindByNameAsync(loginViewModel.UserIdentifier);

			if (user == null)
			{
				Console.WriteLine("Không tìm thấy tài khoản.");
				ModelState.AddModelError("", "Tài khoản không tồn tại.");
				return View(loginViewModel);
			}

			//Console.WriteLine($"Tìm thấy tài khoản: {user.Email} ({user.UserName})");

			if (!await _userManager.CheckPasswordAsync(user, loginViewModel.Password))
			{
				//Console.WriteLine("Sai mật khẩu.");
				ModelState.AddModelError("", "Sai mật khẩu.");
				return View(loginViewModel);
			}

			//Console.WriteLine($"Đăng nhập với UserName: {user.UserName}, Email: {user.Email}");

			await _signInManager.SignInAsync(user, isPersistent: loginViewModel.RememberMe);

			//Console.WriteLine($"Đăng nhập thành công: {user.Email}");

			// Lưu session
			HttpContext.Session.SetString("CustomerMa", user.MaKhachHang ?? Guid.NewGuid().ToString());
			HttpContext.Session.SetString("UserName", user.TenKhachHang ?? "Guest");

			// Lưu Role vào Session
			var roles = await _userManager.GetRolesAsync(user);
			HttpContext.Session.SetString("UserRole", roles.FirstOrDefault() ?? "User");

			//Console.WriteLine($"Đã lưu session: CustomerMa = {HttpContext.Session.GetString("CustomerMa")}");
			//Console.WriteLine($"Đã lưu session: UserName = {HttpContext.Session.GetString("UserName")}");
			//Console.WriteLine($"Roles của User: {string.Join(", ", roles)}");

			if (roles.Contains("Admin"))
			{
				return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
			}

			return RedirectToAction("Index", "Home");
		}
		#endregion

		#region Đăng kí
		[AllowAnonymous]
		public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var existingUser = await _userManager.FindByEmailAsync(model.Email);
			if (existingUser != null)
			{
				ModelState.AddModelError("", "Email đã tồn tại.");
				return View(model);
			}

			var newUser = new Customer
			{
				TenKhachHang = model.Name,
				Email = model.Email,
                DiaChi = model.DiaChi,
				UserName = model.Email,
				MaKhachHang = "KH" + Guid.NewGuid().ToString("N").Substring(0, 8)
				
			};

			var result = await _userManager.CreateAsync(newUser, model.Password);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return View(model);
			}


			var roleExists = await _roleManager.RoleExistsAsync("User");
			if (!roleExists)
			{
				await _roleManager.CreateAsync(new IdentityRole("User"));
			}

			var roleResult = await _userManager.AddToRoleAsync(newUser, "User");
			if (!roleResult.Succeeded)
			{
				await _userManager.DeleteAsync(newUser);
				foreach (var error in roleResult.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				ModelState.AddModelError("", "Không thể thêm vai trò. Tài khoản đã bị xóa.");
				return View(model);
			}

			return RedirectToAction("Login", "Account");
		}
        #endregion

        #region Đăng xuất
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear(); // 

            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Đổi mật khẩu
        [Authorize]
		[HttpGet]
		public IActionResult ChangePassword()
		{
			return View();
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return RedirectToAction("Login");
			}

			var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return View(model);
			}

			// Đăng xuất và yêu cầu đăng nhập lại
			await _signInManager.SignOutAsync();
			TempData["SuccessMessage"] = "Đổi mật khẩu thành công. Vui lòng đăng nhập lại.";

			return RedirectToAction("Login");
		}
        #endregion

        #region Quên mật khẩu
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ViewBag.Message = "Nếu email hợp lệ, bạn sẽ nhận được một liên kết đặt lại mật khẩu.";
                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Account", new { token, email = model.Email }, Request.Scheme);

            await _emailSender.SendEmailAsync(model.Email, "Đặt lại mật khẩu",
                $"Nhấp vào <a href='{resetLink}'>đây</a> để đặt lại mật khẩu.");

            ViewBag.Message = "Nếu email hợp lệ, bạn sẽ nhận được một liên kết đặt lại mật khẩu.";
            return View();
        }

        #endregion

        #region ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return BadRequest("Liên kết không hợp lệ.");
            }

            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }

            return RedirectToAction("Login");
        }

        #endregion
    }
}
