using ASM_CS4.Data;
using ASM_CS4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASM_CS4.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private GetCountCart getCountCart;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
            getCountCart = new GetCountCart(context);
        }

		[HttpGet]
		public IActionResult Index()
		{
			var customerMa = User.FindFirstValue(ClaimTypes.NameIdentifier); // ✅ Lấy ID của User từ Identity
			var customerName = HttpContext.Session.GetString("UserName");

			Console.WriteLine($"Cart - User ID (MaKhachHang): {customerMa}");
			Console.WriteLine($"Cart - UserName: {customerName}");

			if (string.IsNullOrEmpty(customerMa))
			{
				return RedirectToAction("Login", "Account");
			}

			ViewBag.CustomerName = customerName ?? "Người dùng"; // ✅ Nếu null thì hiển thị mặc định

			var cartItems = _context.Carts
				.Where(c => c.MaKhachHang == customerMa)
				.Include(c => c.Product)
				.ToList();

			// Kiểm tra nếu `getCountCart` có thể gây lỗi null
			ViewBag.CartItemCount = getCountCart?.GetCartItemCount(customerMa) ?? 0;

			return View(cartItems);
		}

		[HttpPost]
		public async Task<IActionResult> AddToCart(string proId, int quantity = 1)
		{
			var customerMa = User.FindFirstValue(ClaimTypes.NameIdentifier); // ✅ Lấy ID từ Identity

			if (string.IsNullOrEmpty(customerMa))
			{
				return RedirectToAction("Login", "Account");
			}

			var product = await _context.Products.FindAsync(proId);
			if (product == null)
			{
				TempData["Error"] = "Sản phẩm không tồn tại!";
				return RedirectToAction("Index", "Products");
			}

			var cart = _context.Carts.FirstOrDefault(c => c.MaKhachHang == customerMa && c.MaSanPham == proId);
			if (cart == null)
			{
				cart = new Cart()
				{
					MaGioHang = Guid.NewGuid().ToString(),
					MaKhachHang = customerMa, // ✅ Giữ nguyên MãKhachHang nhưng lấy từ Identity
					MaSanPham = proId,
					SoLuong = quantity,
					TongTien = product.GiaSanPham * quantity,
					CreatedDate = DateTime.Now
				};
				_context.Carts.Add(cart);
			}
			else
			{
				cart.SoLuong += quantity;
				cart.TongTien = product.GiaSanPham * cart.SoLuong;
			}

			await _context.SaveChangesAsync();
			return RedirectToAction("Index", "Home");
		}

		public IActionResult Delete(string maSP)
        {
			var customerMa = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (string.IsNullOrEmpty(customerMa))
            {
                return RedirectToAction("Login", "Account");
            }

            var product = _context.Products.FirstOrDefault(p => p.MaSanPham == maSP);

            if (product == null)
            {
                return RedirectToAction("Cart");
            }

            return View(product);
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirm(string maSP)
		{
			if (string.IsNullOrEmpty(maSP))
			{
				return BadRequest("Mã sản phẩm không hợp lệ!");
			}

			var customerMa = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (string.IsNullOrEmpty(customerMa))
			{
				return RedirectToAction("Login", "Account");
			}

			var cartItem = await _context.Carts
				.FirstOrDefaultAsync(c => c.MaKhachHang == customerMa && c.MaSanPham == maSP);

			if (cartItem == null)
			{
				return NotFound("Sản phẩm không tồn tại trong giỏ hàng!");
			}

			_context.Carts.Remove(cartItem);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index", "Cart");
		}


		[HttpGet]
        public async Task<IActionResult> Checkout()
        {
			var customerMa = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(customerMa))
            {
                return RedirectToAction("Login", " Account");
            }

            var cartItems = await _context.Carts
                .Where(c => c.MaKhachHang == customerMa)
                .Include(c => c.Product)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index", "Cart");
            }

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmCheckout()
        {
			var customerMa = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(customerMa))
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy các sản phẩm từ giỏ hàng
            var cartItems = await _context.Carts
                .Where(c => c.MaKhachHang == customerMa)
                .Include(c => c.Product)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index", "Cart");
            }

            // Tạo các bản ghi trong bảng ChiTietDonDatHang
            foreach (var item in cartItems)
            {
                var chiTietDonHang = new ChiTietDonDatHang
                {
                    MaChiTiet = Guid.NewGuid().ToString(),
                    MaKhachHang = customerMa,
                    MaSanPham = item.MaSanPham,
                    SoLuong = item.SoLuong,
                    Gia = item.Product.GiaSanPham * item.SoLuong,
                    NgayGiao = DateTime.Now.AddDays(3), // Ví dụ: ngày giao hàng sau 3 ngày
                    NgayNhan = DateTime.Now.AddDays(7), // Ví dụ: ngày nhận hàng sau 7 ngày
                    NgayThanhToan = DateTime.Now,
                    TrangThai = "Chờ xác nhận"
                };
                _context.ChiTietDonDatHangs.Add(chiTietDonHang);
            }

            // Xóa giỏ hàng sau khi đặt
            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đơn hàng của bạn đã được đặt thành công!";
            return RedirectToAction("OrderPending");
        }


        [HttpGet]
        public IActionResult OrderPending()
        {
            var customerMa = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerMa))
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy thông tin đơn hàng vừa đặt để hiển thị
            var orders = _context.ChiTietDonDatHangs
                .Where(o => o.MaKhachHang == customerMa && o.TrangThai == "Chờ xác nhận")
                .Include(o => o.Product)
                .ToList();

            return View(orders);
        }

    }
}
