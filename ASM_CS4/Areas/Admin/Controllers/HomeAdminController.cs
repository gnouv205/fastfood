
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASM_CS4.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    [Authorize(Roles = "Admin")]

    public class HomeAdminController : Controller
    {
        [Route("index")]
        public IActionResult Index()
        {
            // Kiem tra User.Identity da co chua
            //Console.WriteLine($"🟢 User đã đăng nhập? {User.Identity.IsAuthenticated}");
            //Console.WriteLine($"🔍 Các roles của User: {string.Join(", ", User.Claims.Where(c => c.Type == "role").Select(c => c.Value))}");

            return View();
        }

    }
}
