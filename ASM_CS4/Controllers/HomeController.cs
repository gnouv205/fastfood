using ASM_CS4.Data;
using ASM_CS4.Models;
using ASM_CS4.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
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

        public IActionResult TestSession()
        {
            string customerMa = HttpContext.Session.GetString("CustomerMa");
            string userName = HttpContext.Session.GetString("UserName");

            Console.WriteLine($"🔍 Kiểm tra session ở trang khác: CustomerMa = {customerMa}, UserName = {userName}");

            return Content($"CustomerMa: {customerMa}, UserName: {userName}");
        }

        public IActionResult Index(int? page, string searchProduct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = HttpContext.Session.GetString("UserName");

            var findProduct = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchProduct))
            {
                findProduct = findProduct.Where(p => p.TenSanPham.Contains(searchProduct));
            }

            ViewBag.CustomerName = string.IsNullOrEmpty(userName) ? "Khách" : userName;

            int pageSize = 8;
            int pageNumber = page ?? 1;

            var products = findProduct
                                .OrderBy(p => p.MaSanPham)
                                .ToPagedList(pageNumber, pageSize);

            if (!products.Any())
            {
                ViewBag.Message = "Không có sản phẩm nào";
            }

            ViewBag.CartItemCount = !string.IsNullOrEmpty(userId) ? getCountCart.GetCartItemCount(userId) : 0;

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

