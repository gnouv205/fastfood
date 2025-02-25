using ASM_CS4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASM_CS4.Data
{
    public class ApplicationDbContext : IdentityDbContext<Customer, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<ChiTietDonDatHang> ChiTietDonDatHangs { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder); // Gọi phương thức của `IdentityDbContext`

			// Đặt Id làm khóa chính
			modelBuilder.Entity<Customer>().HasKey(c => c.Id);

			// Quan hệ giữa Product và Category
			modelBuilder.Entity<Product>()
				.HasOne(p => p.Category)
				.WithMany(c => c.Products)
				.HasForeignKey(p => p.MaDanhMuc)
				.OnDelete(DeleteBehavior.Restrict);

			// Quan hệ giữa Cart và Customer
			modelBuilder.Entity<Cart>()
				.HasOne(c => c.Customer)
				.WithMany(cu => cu.Carts)
				.HasForeignKey(c => c.MaKhachHang)
				.OnDelete(DeleteBehavior.Cascade);

			// 🛠️ Thêm quan hệ giữa Cart và Product (BỊ THIẾU TRƯỚC ĐÓ)
			modelBuilder.Entity<Cart>()
				.HasOne(c => c.Product)
				.WithMany(p => p.Carts)
				.HasForeignKey(c => c.MaSanPham)
				.OnDelete(DeleteBehavior.Restrict);

			// Quan hệ giữa CartDetail và Cart
			modelBuilder.Entity<CartDetail>()
				.HasOne(cd => cd.Cart)
				.WithMany(c => c.CartDetails)
				.HasForeignKey(cd => cd.MaGioHang)
				.OnDelete(DeleteBehavior.Cascade);

			// Quan hệ giữa CartDetail và Product
			modelBuilder.Entity<CartDetail>()
				.HasOne(cd => cd.Product)
				.WithMany(p => p.CartDetails)
				.HasForeignKey(cd => cd.MaSanPham)
				.OnDelete(DeleteBehavior.Restrict);
		}

	}
}
