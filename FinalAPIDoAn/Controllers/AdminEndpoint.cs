using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        // Chỉ người dùng có vai trò Admin mới có thể truy cập endpoint này.
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAdminData()
        {
            // Xử lý logic ở đây.
            return Ok(new { message = "Dữ liệu chỉ dành cho Admin." });
        }
    }
}
