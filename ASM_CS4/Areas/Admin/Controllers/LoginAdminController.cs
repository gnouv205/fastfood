using ASM_CS4.Data;
using Microsoft.AspNetCore.Mvc;

namespace ASM_CS4.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginAdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string Email, string MatKhau)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.Email == Email && a.MatKhau == MatKhau);

            if (admin != null)
            {
                HttpContext.Session.SetString("MaAdmin", admin.MaAdmin); // Lưu MaAdmin vào session
                HttpContext.Session.SetString("AdminName", admin.HoTen);

                return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
            }

            ViewBag.Error = "Email hoặc mật khẩu không đúng!";
            return View("Index");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
