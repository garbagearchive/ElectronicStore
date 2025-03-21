using FinalAPIDoAn.Data;
using FinalAPIDoAn.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/discounts")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly KetNoiCSDL _dbc;

        public DiscountController(KetNoiCSDL dbc)
        {
            _dbc = dbc;
        }

        [HttpGet("List")]
        public IActionResult GetAllDiscounts()
        {
            var discounts = _dbc.Discounts.ToList();
            return Ok(new { data = discounts });
        }

        [HttpGet("Search/{id}")]
        public IActionResult GetDiscountById(int id)
        {
            var discount = _dbc.Discounts.SingleOrDefault(d => d.DiscountId == id);
            if (discount == null)
            {
                return NotFound(new { message = "Discount not found." });
            }
            return Ok(new { data = discount });
        }

        [HttpPost("Add")]
        public IActionResult AddDiscount([FromBody] DiscountDto discountDto)
        {
            if (!ModelState.IsValid || discountDto.StartDate >= discountDto.EndDate)
            {
                return BadRequest(new { message = "Invalid data. EndDate must be after StartDate." });
            }

            var discount = new Discount
            {
                DiscountCode = discountDto.DiscountCode,
                Description = discountDto.Description,
                DiscountPercentage = discountDto.DiscountPercentage,
                StartDate = discountDto.StartDate,
                EndDate = discountDto.EndDate,
                IsActive = discountDto.IsActive
            };

            _dbc.Discounts.Add(discount);
            _dbc.SaveChanges();

            return CreatedAtAction(nameof(GetDiscountById), new { id = discount.DiscountId }, discount);
        }

        [HttpPut("Update/{id}")]
        public IActionResult UpdateDiscount(int id, [FromBody] DiscountDto discountDto)
        {
            if (!ModelState.IsValid || discountDto.StartDate >= discountDto.EndDate)
            {
                return BadRequest(new { message = "Invalid data. EndDate must be after StartDate." });
            }

            var discount = _dbc.Discounts.FirstOrDefault(d => d.DiscountId == id);
            if (discount == null)
            {
                return NotFound(new { message = "Discount not found." });
            }

            discount.DiscountCode = discountDto.DiscountCode;
            discount.Description = discountDto.Description;
            discount.DiscountPercentage = discountDto.DiscountPercentage;
            discount.StartDate = discountDto.StartDate;
            discount.EndDate = discountDto.EndDate;
            discount.IsActive = discountDto.IsActive;

            _dbc.Discounts.Update(discount);
            _dbc.SaveChanges();

            return Ok(new { data = discount });
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteDiscount(int id)
        {
            var discount = _dbc.Discounts.FirstOrDefault(d => d.DiscountId == id);
            if (discount == null)
            {
                return NotFound(new { message = "Discount not found." });
            }

            _dbc.Discounts.Remove(discount);
            _dbc.SaveChanges();
            return Ok(new { message = "Discount deleted successfully." });
        }
    }

    public class DiscountDto
    {
        [Required]
        public string DiscountCode { get; set; }

        public string Description { get; set; }

        [Range(1, 100, ErrorMessage = "Discount percentage must be between 1 and 100.")]
        public int DiscountPercentage { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
