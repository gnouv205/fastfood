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
        public int GetCartItemCount(string userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				return 0;
			}

			return _context.Carts
				   .Where(c => c.MaKhachHang == userId) 
				   .Sum(c => c.SoLuong);
		}
	}
}
