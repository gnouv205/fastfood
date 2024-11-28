using Microsoft.EntityFrameworkCore;

namespace ASM_CS4.Data
{
	public class GetCountCart
	{
		private readonly ApplicationDbContext _context;
        public GetCountCart(ApplicationDbContext context)
        {
            _context = context;
        }
        public int GetCartItemCount(string customerMa)
		{
			if (string.IsNullOrEmpty(customerMa))
			{
				return 0;
			}

			// Lấy tổng số lượng sản phẩm trong giỏ hàng của khách hàng
			return _context.Carts
						   .Where(c => c.MaKhachHang == customerMa)
						   .Sum(c => c.SoLuong);
		}
	}
}
