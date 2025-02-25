using ASM_CS4.Data;
using ASM_CS4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
[Area("Admin")]
public class ProductApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    #region GET
    [HttpGet]
    public async Task<IActionResult> GetProducts(string searchProduct = "")
    {
        var productsQuery = _context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(searchProduct))
        {
            productsQuery = productsQuery.Where(p => p.TenSanPham.Contains(searchProduct));
        }

        var productsList = await productsQuery.OrderBy(p => p.MaSanPham).ToListAsync();

        return Ok(productsList); 
    }
    [HttpGet("{maSanPham}")]
    public async Task<IActionResult> GetProductById(string maSanPham)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.MaSanPham == maSanPham); // ✅ Trả về 1 sản phẩm duy nhất

        if (product == null) return NotFound();

        return Ok(product); // ✅ Trả về object `{}` thay vì danh sách `[]`
    }


    #endregion
    #region POST
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] Product product)
    {
        if (product == null)
        {
            return BadRequest(new { message = "Dữ liệu sản phẩm không hợp lệ." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Nếu HinhSanPham là null hoặc rỗng, gán giá trị mặc định
        if (string.IsNullOrEmpty(product.HinhSanPham))
        {
            product.HinhSanPham = "/LayoutUser/img/product/product-1.jpg"; // Thay bằng ảnh mặc định nếu cần
        }

        try
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { maSanPham = product.MaSanPham }, product);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi server khi tạo sản phẩm.", error = ex.Message });
        }
    }

    #endregion
    #region PUT
    [HttpPut("{maSanPham}")]
    public async Task<IActionResult> UpdateProduct(string maSanPham, [FromBody] Product updatedProduct)
    {
        if (maSanPham != updatedProduct.MaSanPham)
        {
            return BadRequest(new { message = "ID sản phẩm không khớp." });
        }

        var existingProduct = await _context.Products.FindAsync(maSanPham);
        if (existingProduct == null)
        {
            return NotFound(new { message = "Sản phẩm không tồn tại." });
        }

        existingProduct.TenSanPham = updatedProduct.TenSanPham;
        existingProduct.GiaSanPham = updatedProduct.GiaSanPham;
        existingProduct.SoLuongSanPham = updatedProduct.SoLuongSanPham;
        existingProduct.MoTaSanPham = updatedProduct.MoTaSanPham;
        existingProduct.TrangThai = updatedProduct.TrangThai;
        existingProduct.MaDanhMuc = updatedProduct.MaDanhMuc;

        _context.Products.Update(existingProduct);
        await _context.SaveChangesAsync();

        return Ok(existingProduct);
    }
    #endregion
    #region DELETE
    [HttpDelete("{maSanPham}")]
    public async Task<IActionResult> DeleteProduct(string maSanPham)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.MaSanPham == maSanPham);
        if (product == null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent(); 
    }

    #endregion
}