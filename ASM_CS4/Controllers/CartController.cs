using ASM_CS4.Data;
using ASM_CS4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;

namespace ASM_CS4.Controllers
{
	public class CartController : Controller
	{
		private readonly ApplicationDbContext _context;
		private GetCountCart getCountCart;
		public CartController(ApplicationDbContext context)
		{
			_context = context;
			getCountCart=new GetCountCart(context); 
		}
		[HttpGet]
		public IActionResult Cart()
		{
			var customerMa = HttpContext.Session.GetString("CustomerMa");

			if (string.IsNullOrEmpty(customerMa))
			{
				return RedirectToAction("Login", "Customer");
			}

			// Tiến hành xử lý giỏ hàng
			var cartItems = _context.Carts
				.Where(c => c.MaKhachHang == customerMa)
				.Include(c => c.Product)
				.ToList();

			// Đặt giá trị cho ViewBag.CartItemCount
			ViewBag.CartItemCount = getCountCart.GetCartItemCount(customerMa);

			return View(cartItems);
		}


		[HttpPost]
		public async Task<IActionResult> AddToCart(string proId, int quantity = 1)
		{
			var customerMa = HttpContext.Session.GetString("CustomerMa");
			var product = _context.Products.FirstOrDefault(p => p.MaSanPham == proId);

			if (string.IsNullOrEmpty(customerMa))
			{
				return RedirectToAction("Login", "Customer");
			}

			if (string.IsNullOrEmpty(proId))
			{
				ViewBag.ErrorMessage = "Mã sản phẩm không hợp lệ.";
				return RedirectToAction("Index", "Home");
			}

			// Kiểm tra sản phẩm có tồn tại trong bảng Products không
			if (product == null)
			{
				ViewBag.ErrorMessage = "Sản phẩm không tồn tại.";
				return RedirectToAction("Index", "Home");
			}

			// Lấy giỏ hàng của khách hàng hoặc tạo mới nếu chưa có
			var cart = _context.Carts.FirstOrDefault(c => c.MaKhachHang == customerMa && c.MaSanPham == proId);
			if (cart == null)
			{
				cart = new Cart()
				{
					MaGioHang = Guid.NewGuid().ToString(),
					MaKhachHang = customerMa,
					SoLuong = quantity,
					TongTien = product.GiaSanPham * quantity,
					CreatedDate = DateTime.Now,
					MaSanPham = proId
				};
				_context.Carts.Add(cart);	
			}
			else
			{
                // Nếu giỏ hàng đã tồn tại, tăng số lượng và cập nhật tổng tiền
                cart.SoLuong += quantity;
                cart.TongTien = product.GiaSanPham * cart.SoLuong;
            }


			await _context.SaveChangesAsync();
            

            return RedirectToAction("Index", "Home");
		}

		public IActionResult Delete(string maSP)
		{
			var customerMa = HttpContext.Session.GetString("CustomerMa");

			if (string.IsNullOrEmpty(customerMa))
			{
				return RedirectToAction("Login", "Customer");
			}

			// Lấy thông tin sản phẩm từ bảng Products (để hiển thị)
			var product = _context.Products.FirstOrDefault(p => p.MaSanPham == maSP);

			if (product == null)
			{
				return RedirectToAction("Cart");
			}

			return View(product); // Trả về view Delete (trang xác nhận xóa)
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirm(string maSP)
		{
			var customerMa = HttpContext.Session.GetString("CustomerMa");

			if (string.IsNullOrEmpty(customerMa))
			{
				return RedirectToAction("Login", "Customer");
			}

			// Tìm bản ghi trong bảng Carts
			var cartItem = _context.Carts.FirstOrDefault(c => c.MaKhachHang == customerMa && c.MaSanPham == maSP);

			if (cartItem != null)
			{
				_context.Carts.Remove(cartItem); // Xóa bản ghi khỏi bảng Carts
				await _context.SaveChangesAsync(); // Lưu thay đổi vào CSDL
			}

			return RedirectToAction("Cart"); // Quay lại trang giỏ hàng
		}

	}

}
