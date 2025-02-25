using ASM_CS4.Data;
using ASM_CS4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace ASM_CS4.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class ProductAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:44304/api/ProductApi";

        public ProductAdminController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, HttpClient httpClient)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index(string searchProduct = "")
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}?searchProduct={searchProduct}");
            if (!response.IsSuccessStatusCode) return View(new List<Product>());

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<Product>>(jsonResponse);

            ViewBag.SearchProduct = searchProduct;
            return View(products);
        }
        // 🔹 Hiển thị chi tiết sản phẩm
        public async Task<IActionResult> Details(string id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<Product>(jsonResponse);

            return View(product);
        }

        // 🔹 Hiển thị form thêm sản phẩm
        public async Task<IActionResult> Create()
        {
            ViewBag.DanhMuc = await GetDanhMucSelectList();
            return View();
        }

        // 🔹 Xử lý thêm sản phẩm
        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile uploadedFile)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                // 🏷 Tạo đường dẫn thư mục lưu ảnh
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");

                // ✅ Đảm bảo thư mục tồn tại
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // 🏷 Tạo tên file duy nhất để tránh trùng lặp
                var uniqueFileName = $"{Guid.NewGuid()}_{uploadedFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // ✅ Lưu file vào server
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                // ✅ Cập nhật đường dẫn ảnh trong database
                product.HinhSanPham = "/images/products/" + uniqueFileName;
            }
            else
            {
                product.HinhSanPham = "/images/products/default.jpg"; // Ảnh mặc định nếu không chọn file
            }

            // ✅ Gửi dữ liệu sản phẩm đến API
            var jsonContent = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrl, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                return View(product);
            }

            return RedirectToAction(nameof(Index));
        }


        // 🔹 Hiển thị form cập nhật sản phẩm
        public async Task<IActionResult> Edit(string maSanPham)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{maSanPham}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<Product>(jsonResponse);

            ViewBag.DanhMucs = await GetDanhMucSelectList();
            return View(product);
        }

        // 🔹 Xử lý cập nhật sản phẩm
        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] string maSanPham, [FromForm] Product product)
        {
            if (string.IsNullOrEmpty(maSanPham))
            {
                ModelState.AddModelError("", "Mã sản phẩm không được để trống.");
                return View(product);
            }

            // Đóng gói đúng dữ liệu gửi API
            var productDto = new
            {
                MaSanPham = product.MaSanPham,
                TenSanPham = product.TenSanPham,
                MoTaSanPham = product.MoTaSanPham,
                SoLuongSanPham = product.SoLuongSanPham,
                GiaSanPham = product.GiaSanPham,
                TrangThai = product.TrangThai,
                MaDanhMuc = string.IsNullOrEmpty(product.MaDanhMuc) ? null : product.MaDanhMuc
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(productDto), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_apiUrl}/{maSanPham}", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Cập nhật sản phẩm thất bại.");
                return View(product);
            }

            return RedirectToAction(nameof(Index));
        }



        // 🔹 Xác nhận xóa sản phẩm

        public async Task<IActionResult> Delete(string maSanPham)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{maSanPham}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var product = JsonConvert.DeserializeObject<Product>(jsonResponse);

            return View(product);
        }
        // 🔹 Xử lý xóa sản phẩm
        [HttpPost]
        [ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(string maSanPham)
        {
            if (string.IsNullOrEmpty(maSanPham))
            {
                return BadRequest(new { message = "Mã sản phẩm không hợp lệ." });
            }

            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{maSanPham}");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(new { message = "Lỗi khi xóa sản phẩm: " + error });
            }

            return RedirectToAction(nameof(Index));
        }


        private async Task<List<SelectListItem>> GetDanhMucSelectList()
        {
            var danhMucList = await _context.Categories
                .Select(dm => new SelectListItem
                {
                    Value = dm.MaDanhMuc.ToString(),
                    Text = dm.TenDanhMuc
                })
                .ToListAsync();

            return danhMucList;
        }



    }
}
