using FinalAPIDoAn.MyModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly KetNoiCSDL _dbc;

        public DiscountController(KetNoiCSDL dbc)
        {
            _dbc = dbc;
        }
        [HttpGet("List")]
        public IActionResult GetAllDiscount()
        {
            var discount = _dbc.Discounts.ToList();
            return Ok(new { data = discount });
        }
        [HttpGet("Search")]
        public IActionResult GetDiscountById(int id)
        {
            var discount = _dbc.Discounts.SingleOrDefault(o => o.DiscountId == id);
            if (discount == null) return NotFound(new { message = "Discount not found." });

            return Ok(new { data = discount });
        }
        [HttpPost("Add")]
        public IActionResult AddDiscount([FromBody] DiscountDto discountDto)
        {
            if (discountDto.DiscountID <= 0 || string.IsNullOrWhiteSpace(discountDto.DiscountCode) || discountDto.DiscountPercentage <= 0)
            {
                return BadRequest(new { message = "Invalid data." });
            }
            var discount = new Discount
            {
                DiscountId = discountDto.DiscountID,
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
        [HttpPut("Update")]
        public IActionResult UpdateDiscount([FromBody] DiscountDto discountDto)
        {
            if (discountDto.DiscountID <= 0 || string.IsNullOrWhiteSpace(discountDto.DiscountCode) || discountDto.DiscountPercentage <= 0)
            {
                return BadRequest(new { message = "Invalid data." });
            }
            var discount = new Discount
            {
                DiscountId = discountDto.DiscountID,
                DiscountCode = discountDto.DiscountCode,
                Description = discountDto.Description,
                DiscountPercentage = discountDto.DiscountPercentage,
                StartDate = discountDto.StartDate,
                EndDate = discountDto.EndDate,
                IsActive = discountDto.IsActive
            };
            _dbc.Discounts.Update(discount);
            _dbc.SaveChanges();
            return CreatedAtAction(nameof(GetDiscountById), new { id = discount.DiscountId }, discount);
        }

        [HttpDelete("Delete")]
        public IActionResult DeleteDiscount(int id)
        {
            var discount = _dbc.Discounts.SingleOrDefault(o => o.DiscountId == id);
            if (discount == null) return NotFound(new { message = "Discount not found." });
            _dbc.Discounts.Remove(discount);
            _dbc.SaveChanges();
            return Ok(new { message = "Discount deleted successfully." });
        }
        public class DiscountDto
        {
            public required int DiscountID { get; set; }
            public required string DiscountCode { get; set; }
            public required string Description { get; set; }
            public required int DiscountPercentage { get; set; }
            public required DateTime StartDate { get; set; }
            public required DateTime EndDate { get; set; }
            public required bool IsActive { get; set; }
        }
    }
       
}

