using FinalAPIDoAn.Data;
using FinalAPIDoAn.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly KetNoiCSDL _dbc;

        public CategoryController(KetNoiCSDL dbc)
        {
            _dbc = dbc;
        }

        [HttpGet("List")]
        public IActionResult GetAllCategories()
        {
            var categories = _dbc.Categories.ToList();
            return Ok(new { data = categories });
        }

        [HttpGet("Search")]
        public IActionResult SearchCategory([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new { message = "Invalid search keyword." });
            }

            var results = _dbc.Categories
                .Where(s => s.CategoryName.Contains(keyword) || s.Description.Contains(keyword))
                .ToList();

            if (!results.Any())
            {
                return NotFound(new { message = "No categories found matching the keyword." });
            }

            return Ok(new { data = results });
        }

        [HttpPost("Add")]
        public IActionResult AddCategory([FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null || string.IsNullOrWhiteSpace(categoryDto.CategoryName))
                return BadRequest(new { message = "Category name is required." });

            var category = new Category
            {
                CategoryName = categoryDto.CategoryName,
                Description = categoryDto.Description
            };

            _dbc.Categories.Add(category);
            _dbc.SaveChanges();

            return CreatedAtAction(nameof(GetAllCategories), new { id = category.CategoryId }, category);
        }

        [HttpPut("Update/{id}")]
        public IActionResult UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null || string.IsNullOrWhiteSpace(categoryDto.CategoryName))
                return BadRequest(new { message = "Invalid category data." });

            var category = _dbc.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
                return NotFound(new { message = "Category not found." });

            category.CategoryName = categoryDto.CategoryName;
            category.Description = categoryDto.Description;

            _dbc.Categories.Update(category);
            _dbc.SaveChanges();

            return Ok(new { data = category });
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteCategory(int id)
        {
            var category = _dbc.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null)
                return NotFound(new { message = "Category not found." });

            _dbc.Categories.Remove(category);
            _dbc.SaveChanges();

            return Ok(new { message = "Category deleted successfully." });
        }
    }

    public class CategoryDto
    {
        [Required(ErrorMessage = "Category name is required.")]
        [MaxLength(255, ErrorMessage = "Category name must be at most 255 characters.")]
        public string CategoryName { get; set; }

        [MaxLength(1000, ErrorMessage = "Description must be at most 1000 characters.")]
        public string Description { get; set; }
    }
}
