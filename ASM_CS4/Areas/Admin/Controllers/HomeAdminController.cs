using ASM_CS4.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ASM_CS4.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    [AdminAuthorize] // Sử dụng bộ lọc đã cập nhật
    public class HomeAdminController : Controller
    {
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
