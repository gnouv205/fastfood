using ASM_CS4.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace ASM_CS4.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}
		public DbSet<Admin> Admins { get; set; }
		public DbSet<Customer> Customers { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<Cart> Carts { get; set; }
		public DbSet<CartDetail> CartDetails { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Cấu hình các khóa chính
			modelBuilder.Entity<Admin>().HasKey(a => a.MaAdmin);
			modelBuilder.Entity<Customer>().HasKey(c => c.MaKhachHang);
			modelBuilder.Entity<Category>().HasKey(c => c.MaDanhMuc);
			modelBuilder.Entity<Product>().HasKey(p => p.MaSanPham);
			modelBuilder.Entity<Cart>().HasKey(c => c.MaGioHang);
			modelBuilder.Entity<CartDetail>().HasKey(cd => cd.MaChiTiet);

			// Quan hệ giữa Product và Category (1-N)
			modelBuilder.Entity<Product>()
				.HasOne(p => p.Category)
				.WithMany(c => c.Products)
				.HasForeignKey(p => p.MaDanhMuc)
				.OnDelete(DeleteBehavior.Restrict); // Khi xóa Category không xóa Product

			// Quan hệ giữa Cart và Customer (N-1)
			modelBuilder.Entity<Cart>()
				.HasOne(c => c.Customer)
				.WithMany(cu => cu.Carts)
				.HasForeignKey(c => c.MaKhachHang)
				.OnDelete(DeleteBehavior.Cascade); // Xóa giỏ hàng khi xóa khách hàng

			// Quan hệ giữa Cart và Product (1-N) qua CartDetail
			modelBuilder.Entity<CartDetail>()
				.HasOne(cd => cd.Cart)
				.WithMany(c => c.CartDetails)
				.HasForeignKey(cd => cd.MaGioHang)
				.OnDelete(DeleteBehavior.Cascade); // Xóa chi tiết giỏ hàng khi xóa giỏ hàng

			// CartDetails với Product
			modelBuilder.Entity<CartDetail>()
				.HasOne(cd => cd.Product)
				.WithMany(p => p.CartDetails)
				.HasForeignKey(cd => cd.MaSanPham)
				.OnDelete(DeleteBehavior.Restrict); // Khi xóa sản phẩm không xóa chi tiết giỏ hàng

			modelBuilder.Entity<Cart>()
				.HasOne(c => c.Product)
				.WithMany()
				.HasForeignKey(c => c.MaSanPham);
		}
	}
}
