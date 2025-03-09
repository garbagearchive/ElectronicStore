using FinalAPIDoAn.MyModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/user")]
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
            var users = _dbc.Users.ToList();
            return Ok(new { data = users });
        }

        [HttpGet("Search")]
        public IActionResult SearchName([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Invalid search keyword" });

            var results = _dbc.Users.Where(s => s.FullName.Contains(keyword)).ToList();

            if (!results.Any())
                return NotFound(new { message = "No user found matching the keyword." });

            return Ok(new { data = results });
        }

        [HttpPost("Add")]
        public IActionResult AddUser([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = userDto.PasswordHash,
                FullName = userDto.FullName,
                Email = userDto.Email,
                Phone = userDto.Phone,
                Address = userDto.Address,
                Role = userDto.Role ?? "Buyer"
            };

            _dbc.Users.Add(user);
            _dbc.SaveChanges();

            return CreatedAtAction(nameof(SearchName), new { keyword = user.FullName }, user);
        }

        [HttpPut("Update/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _dbc.Users.SingleOrDefault(o => o.UserId == id);
            if (user == null)
                return NotFound(new { message = "User not found." });

            user.Username = userDto.Username;
            user.PasswordHash = userDto.PasswordHash;
            user.FullName = userDto.FullName;
            user.Email = userDto.Email;
            user.Phone = userDto.Phone;
            user.Address = userDto.Address;
            user.Role = userDto.Role ?? "Buyer";

            _dbc.Users.Update(user);
            _dbc.SaveChanges();

            return Ok(new { message = "User updated successfully.", data = user });
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _dbc.Users.SingleOrDefault(o => o.UserId == id);
            if (user == null)
                return NotFound(new { message = "User not found." });

            _dbc.Users.Remove(user);
            _dbc.SaveChanges();

            return Ok(new { message = "User deleted successfully." });
        }
    }

    public class UserDto
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        [Required]
        public required string FullName { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Phone { get; set; }

        [Required]
        public required string Address { get; set; }

        public string? Role { get; set; }
    }
}
