using FinalAPIDoAn.MyModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PDiscountController : ControllerBase
    {
        private readonly KetNoiCSDL _dbc;

        public PDiscountController(KetNoiCSDL dbc)
        {
            _dbc = dbc;
        }
        [HttpGet("List")]
        public IActionResult GetAllPDiscount()
        {
            var pdiscount = _dbc.ProductDiscounts.ToList();
            return Ok(new { data = pdiscount });
        }
        [HttpGet("Search")]
        public IActionResult GetPDiscountById(int id)
        {
            var pdiscount = _dbc.ProductDiscounts.SingleOrDefault(o => o.ProductId == id);
            if (pdiscount == null) return NotFound(new { message = "Discount not found." });

            return Ok(new { data = pdiscount });
        }
        [HttpPost("Add")]
        public IActionResult AddPDiscount([FromBody] PDiscountDto pdiscountDto)
        {
            if (pdiscountDto.ProductID <= 0|| pdiscountDto.DiscountID <= 0)
            {
                return BadRequest(new { message = "Invalid data." });
            }
            var pdiscount = new ProductDiscount
            {
                DiscountId = pdiscountDto.DiscountID,
                ProductId = pdiscountDto.ProductID
            };
            _dbc.ProductDiscounts.Add(pdiscount);
            _dbc.SaveChanges();
            return CreatedAtAction(nameof(GetPDiscountById), new { id = pdiscount.DiscountId }, pdiscount);
        }
        [HttpPut("Update")]
        public IActionResult UpdatePDiscountt([FromBody] PDiscountDto pdiscountDto)
        {
            if (pdiscountDto.ProductID <= 0 || pdiscountDto.DiscountID <= 0)
            {
                return BadRequest(new { message = "Invalid data." });
            }
            var pdiscount = new ProductDiscount
            {
                DiscountId = pdiscountDto.DiscountID,
                ProductId = pdiscountDto.ProductID
            };
            _dbc.ProductDiscounts.Update(pdiscount);
            _dbc.SaveChanges();
            return CreatedAtAction(nameof(GetPDiscountById), new { id = pdiscount.DiscountId }, pdiscount);
        }
        [HttpDelete("Delete")]
        public IActionResult DeletePDiscount(int id)
        {
            var pdiscount = _dbc.ProductDiscounts.SingleOrDefault(o => o.DiscountId == id);
            if (pdiscount == null) return NotFound(new { message = "Discount not found." });
            _dbc.ProductDiscounts.Remove(pdiscount);
            _dbc.SaveChanges();
            return Ok(new { message = "Discount deleted successfully." });
        }
        public class PDiscountDto
        {
            public required int DiscountID { get; set; }
            public required int ProductID { get; set; }
        }
    }
}
