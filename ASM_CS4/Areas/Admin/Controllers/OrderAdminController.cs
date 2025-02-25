using ASM_CS4.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_CS4.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class OrderAdminController : Controller
	{
		private readonly ApplicationDbContext _context;

		public OrderAdminController(ApplicationDbContext context)
		{
			_context = context;
		}

		// Hiển thị danh sách đơn hàng chờ xác nhận
		public async Task<IActionResult> AdminConfirmOrder()
		{
			var pendingOrders = await _context.ChiTietDonDatHangs
				.Where(o => o.TrangThai == "Chờ xác nhận")
				.Include(o => o.Product)
				.Include(o => o.Customer)
				.ToListAsync();

			return View(pendingOrders);
		}

		// Xác nhận đơn hàng và cập nhật trạng thái thành "Đang giao hàng"
		[HttpPost]
		public async Task<IActionResult> ApproveOrder(string maChiTiet)
		{
			var order = await _context.ChiTietDonDatHangs.FirstOrDefaultAsync(o => o.MaChiTiet == maChiTiet);
			if (order != null)
			{
				// Cập nhật trạng thái thành "Đang giao hàng"
				order.TrangThai = "Đang giao hàng";
				_context.Update(order);
				await _context.SaveChangesAsync();
			}

			// Quay lại trang danh sách đơn hàng chờ xác nhận
			return RedirectToAction("AdminConfirmOrder");
		}
        public async Task<IActionResult> SalesStatistics()
        {
            // Lấy danh sách các đơn hàng đã được xác nhận và tính tổng số lượng sản phẩm đã bán.
            var salesStatistics = await _context.ChiTietDonDatHangs
                .Where(o => o.TrangThai == "Đã xác nhận")
                .GroupBy(o => o.MaSanPham)
                .Select(g => new
                {
                    ProductName = g.FirstOrDefault().Product.TenSanPham,
                    QuantitySold = g.Sum(x => x.SoLuong),
                    TotalRevenue = g.Sum(x => x.Gia)
                })
                .ToListAsync();

            return View(salesStatistics);
        }

    }
}
