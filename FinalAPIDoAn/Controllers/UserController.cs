using FinalAPIDoAn.MyModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly KetNoiCSDL _dbc;
        public UserController(KetNoiCSDL dbc)
        {
            _dbc = dbc;
        }

        [HttpGet("List")]
        public IActionResult GetAllUserDetails()
        {
            var user = _dbc.Users.ToList();
            return Ok(new { data = user });
        }
        [HttpGet("Search")]
        public IActionResult SearchName([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new { message = "Invalid search keyword" });
            }
            var results = _dbc.Users.Where(s => s.FullName.Contains(keyword));

            if (!results.Any())
            {
                return NotFound(new { message = "No user found matching the keyword." });
            }
            return Ok(new { data = results });
        }

        [HttpPost("Add")]
        public IActionResult AddUser([FromBody] UserDto userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.UserName) || string.IsNullOrWhiteSpace(userDto.PasswordHash) || string.IsNullOrWhiteSpace(userDto.FullName) || string.IsNullOrWhiteSpace(userDto.Email) || string.IsNullOrWhiteSpace(userDto.Phone) || string.IsNullOrWhiteSpace(userDto.Address) || string.IsNullOrWhiteSpace(userDto.Role))
                return BadRequest(new { message = "Invalid user data." });
            var user = new User
            {
                UserId = userDto.UserID,
                PasswordHash = userDto.PasswordHash,
                FullName = userDto.PasswordHash,
                Email = userDto.Email,
                Phone = userDto.Phone,
                Address = userDto.Address,
                Role = userDto.Role ?? "Buyer"
            };
            _dbc.Users.Add(user);
            _dbc.SaveChanges();
            return CreatedAtAction(nameof(SearchName), new { id = user.UserId }, user);
        }
        [HttpPut("Update")]
        public IActionResult Update([FromBody] UserDto userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.UserName) || string.IsNullOrWhiteSpace(userDto.PasswordHash) || string.IsNullOrWhiteSpace(userDto.FullName) || string.IsNullOrWhiteSpace(userDto.Email) || string.IsNullOrWhiteSpace(userDto.Phone) || string.IsNullOrWhiteSpace(userDto.Address) || string.IsNullOrWhiteSpace(userDto.Role))
                return BadRequest(new { message = "Invalid user data." });
            var user = new User
            {
                UserId = userDto.UserID,
                PasswordHash = userDto.PasswordHash,
                FullName = userDto.PasswordHash,
                Email = userDto.Email,
                Phone = userDto.Phone,
                Address = userDto.Address,
                Role = userDto.Role ?? "Buyer"
            };
            _dbc.Users.Update(user);
            _dbc.SaveChanges();
            return CreatedAtAction(nameof(SearchName), new { id = user.UserId }, user);
        }
        [HttpDelete("Delete")]
        public IActionResult DeleteUser(int userid)
        {
            var user = _dbc.Users.SingleOrDefault(o => o.UserId == userid);
            if (user == null) return NotFound(new { message = "User not found." });
            _dbc.Users.Remove(user);
            _dbc.SaveChanges();
            return Ok(new { message = "User deleted successfully." });
        }

        public class UserDto
        {
            public required int UserID { get; set; }
            public required string UserName { get; set; }
            public required string PasswordHash { get; set; }
            public required string FullName { get; set; }
            public required string Email { get; set; }
            public required string Phone { get; set; }
            public required string Address { get; set; }
            public required string Role { get; set; }
        }
    }
}

