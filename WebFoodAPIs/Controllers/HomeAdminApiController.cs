using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebFoodAPIs.Controllers
{
    [Route("api/admin/homeadmin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class HomeAdminApiController : ControllerBase
    {
        [HttpGet("index")]
        public IActionResult Index()
        {
            var userInfo = new
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                Roles = User.Claims.Where(c => c.Type == "role").Select(c => c.Value)
            };

            return Ok(userInfo); // Trả về JSON thay vì View
        }
    }
}
