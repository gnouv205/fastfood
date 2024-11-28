using ASM_CS4.Data;
using ASM_CS4.Filters;
using ASM_CS4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ASM_CS4.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class ProductAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductAdminController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

		// Hiển thị danh sách sản phẩm
		// Hiển thị danh sách sản phẩm hoặc kết quả tìm kiếm
		public async Task<IActionResult> Index(string searchQuery)
		{
			var products = _context.Products.AsQueryable();

			if (!string.IsNullOrEmpty(searchQuery))
			{
				products = products.Where(p => p.TenSanPham.Contains(searchQuery)); // Tìm kiếm theo tên sản phẩm
			}

			return View(await products.ToListAsync());
		}

		#region Create
		[HttpGet]
        public async Task<IActionResult> Create()
        {
            // Lấy danh sách các danh mục từ cơ sở dữ liệu
            ViewBag.DanhMuc = new SelectList(await _context.Categories.ToListAsync(), "MaDanhMuc", "TenDanhMuc");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile uploadedFile)
        {
            if (ModelState.IsValid)
            {
                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    var fileName = Path.GetFileName(uploadedFile.FileName);
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/LayoutUser/img/product", fileName);

                    using (var fileStream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }

                    product.HinhSanPham = "/LayoutUser/img/product/" + fileName;
                }

                _context.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Trả lại danh sách danh mục nếu ModelState không hợp lệ
            ViewBag.DanhMuc = new SelectList(await _context.Categories.ToListAsync(), "MaDanhMuc", "TenDanhMuc");
            return View(product);
        }

        #endregion

        #region Edit
        [HttpGet]
        public async Task<IActionResult> Edit(string? maSP)
        {
            if (maSP == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(maSP);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product, IFormFile? uploadedFile)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = await _context.Products.FindAsync(product.MaSanPham);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                existingProduct.TenSanPham = product.TenSanPham;
                existingProduct.GiaSanPham = product.GiaSanPham;
                existingProduct.SoLuongSanPham = product.SoLuongSanPham;
                existingProduct.MoTaSanPham = product.MoTaSanPham;
                existingProduct.TrangThai = product.TrangThai;
                existingProduct.MaDanhMuc = product.MaDanhMuc;

                // Chỉ cập nhật ảnh nếu người dùng tải ảnh mới
                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    // Xóa ảnh cũ nếu tồn tại
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, existingProduct.HinhSanPham.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                    // Lưu ảnh mới
                    var fileName = Path.GetFileName(uploadedFile.FileName);
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "LayoutUser/img", fileName);
                    using (var fileStream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }

                    // Cập nhật đường dẫn ảnh mới
                    existingProduct.HinhSanPham = "/LayoutUser/img/product/" + fileName;
                }

                // Cập nhật vào cơ sở dữ liệu
                _context.Products.Update(existingProduct);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Sản phẩm đã được cập nhật!";
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        #endregion

        #region Delete
        [HttpGet]
        public IActionResult Delete(string maSP)
        {
            var product = _context.Products.Find(maSP);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string maSP)
        {
            var product = _context.Products.Find(maSP);
            if (product != null)
            {
                // Xóa file ảnh
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.HinhSanPham.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                // Xóa sản phẩm trong database
                _context.Products.Remove(product);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Sản phẩm đã được xóa!";
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }
        #endregion
    }
}
