using ASM_CS4.Data;
using ASM_CS4.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASM_CS4.Controllers
{
	public class CustomerController : Controller
	{
		private readonly ApplicationDbContext _context;
        public CustomerController(ApplicationDbContext context)
        {
			_context = context;
        }
		[HttpGet]
        public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult>? Login(Customer cusLogin)
		{
			if(!ModelState.IsValid)
			{
				var customer = _context.Customers.FirstOrDefault(c => c.Email == cusLogin.Email && c.MatKhau == cusLogin.MatKhau);

				if(customer != null)
				{
					HttpContext.Session.SetString("CustomerMa", customer.MaKhachHang);
					HttpContext.Session.SetString("CustomerName", customer.TenKhachHang);

                    return RedirectToAction("Index", "Home");
				}
			}
			//ModelState.AddModelError("Thông báo", "Email hoặc mật khẩu không chính xác");
			ViewBag.ErrorMessage = "Email hoặc mật khẩu không chính xác";
            return View(cusLogin);
		}


		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Register(Customer cusRegister)
		{
			if(!ModelState.IsValid)
			{
				var existingCustomer = _context.Customers.FirstOrDefault(c => c.Email == cusRegister.Email );
				if(existingCustomer != null)
				{
					ViewBag.Errormessage = "Email da ton tai";
					return View();
				}
				cusRegister.MaKhachHang = Guid.NewGuid().ToString();
				_context.Customers.Add(cusRegister);
				await _context.SaveChangesAsync();

				HttpContext.Session.SetString("CustomerMa", cusRegister.MaKhachHang);
				HttpContext.Session.SetString("CustomerName", cusRegister.TenKhachHang);
				return RedirectToAction("Index", "Home");
			}
			return View(cusRegister);
		}
	}
}
