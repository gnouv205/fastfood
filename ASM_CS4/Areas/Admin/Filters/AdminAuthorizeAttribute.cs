using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASM_CS4.Filters
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session.GetString("MaAdmin"); // Sử dụng MaAdmin thay vì AdminId
            if (string.IsNullOrEmpty(session))
            {
                context.Result = new RedirectToActionResult("Index", "LoginAdmin", new { area = "Admin" });
            }
            base.OnActionExecuting(context);
        }
    }
}
