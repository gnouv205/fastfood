using ASM_CS4.Data;
using ASM_CS4.Filters;
using ASM_CS4.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASM_CS4.Areas.Admin.Controllers
{
	[Area("Admin")]
    [AdminAuthorize]
    public class AccountAdminController : Controller
	{
		private readonly ApplicationDbContext _context;
		public AccountAdminController(ApplicationDbContext context)
		{
			_context = context;
        }
		public async Task<IActionResult> Index()
		{
			var getListAccount = await _context.Admins.ToListAsync();
			return View(getListAccount);
		}
        #region Create
        [HttpGet]
		public IActionResult Create()
		{ 
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(UserAdmin admin)
		{
			if(ModelState.IsValid)
			{
				_context.Admins.Add(admin);
				await _context.SaveChangesAsync();	
				return RedirectToAction("Index");
			}
			return View(admin);
		}
		#endregion

		#region Edit
		[HttpGet]
		public IActionResult Edit(string maAd)
		{
			var getMaSP = _context.Admins.Find(maAd);
			return View(getMaSP);
		}
		[HttpPost]
		public async Task<IActionResult> Edit(UserAdmin admin)
		{
			if(ModelState.IsValid)
			{
				_context.Admins.Update(admin);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(admin);
		}
		#endregion
		#region Delete
		[HttpGet]	
		public IActionResult Delete(string maAd)
		{
			var getMaAd = _context.Admins.Find(maAd);
			return View(getMaAd);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(string maAd, UserAdmin admin)
		{
			var getMaAd = await _context.Admins.FindAsync(maAd);
			if(ModelState.IsValid)
			{
				_context.Admins.Remove(admin);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(admin);
		}
        #endregion
    }
}
