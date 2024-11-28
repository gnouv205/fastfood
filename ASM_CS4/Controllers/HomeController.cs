using ASM_CS4.Data;
using ASM_CS4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;
using X.PagedList.Extensions;
using X.PagedList.Mvc.Core;

namespace ASM_CS4.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly GetCountCart getCountCart;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
            getCountCart = new GetCountCart(context);
        }

		public IActionResult Index(int? page)
		{
			// Lấy thông tin tên khách hàng từ Session
            var user = HttpContext.Session.GetString("CustomerName");
            var userMa = HttpContext.Session.GetString("customerMa");
			ViewBag.CustomerName = user;

			// Cấu hình phân trang
			int pageSize = 8; // Số sản phẩm mỗi trang
			int pageNumber = page ?? 1; // Trang hiện tại (mặc định là trang 1)

			// Lấy danh sách sản phẩm có phân trang
			var products = _context.Products
										 .OrderBy(p => p.MaSanPham)
										 .ToPagedList(pageNumber, pageSize);

			ViewBag.CartItemCount = getCountCart.GetCartItemCount(userMa);

			// Trả về View với Model là danh sách sản phẩm đã phân trang
			return View(products);
		}

		public async Task<IActionResult> Details(string maSP)
        {
            // Lấy chi tiết sản phẩm theo ID
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.MaSanPham == maSP);

            if (product == null)
            {
                return NotFound();
            }

            return View(product); // Trả về View chứa chi tiết sản phẩm
        }

    }
}
