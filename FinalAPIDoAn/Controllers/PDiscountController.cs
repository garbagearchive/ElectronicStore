using FinalAPIDoAn.Data;
using FinalAPIDoAn.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/product-discounts")]
    [ApiController]
    public class PDiscountController : ControllerBase
    {
        private readonly KetNoiCSDL _dbc;

        public PDiscountController(KetNoiCSDL dbc)
        {
            _dbc = dbc;
        }

        [HttpGet("List")]
        public IActionResult GetAllPDiscounts()
        {
            var pdiscount = _dbc.ProductDiscounts
        .Join(
            _dbc.Discounts,
            productDiscount => productDiscount.DiscountId,
            discount => discount.DiscountId,
            (productDiscount, discount) => new { ProductDiscount = productDiscount, Discount = discount }
        )
        .Join(
            _dbc.Products,
            combined => combined.ProductDiscount.ProductId,
            product => product.ProductId,
            (combined, product) => new
            {
                DiscountId = combined.Discount.DiscountId,
                DiscountCode = combined.Discount.DiscountCode, // Corrected: Get DiscountCode from Discounts table
                ProductId = product.ProductId,
                ProductName = product.ProductName
            }
        )
        .ToList();

            return Ok(new { data = pdiscount });
        }

        [HttpGet("Search/{productId}/{discountId}")]
        public IActionResult GetPDiscountById(int productId, int discountId)
        {
            var pdiscount = _dbc.ProductDiscounts
                .SingleOrDefault(o => o.ProductId == productId && o.DiscountId == discountId);

            if (pdiscount == null)
                return NotFound(new { message = "Discount not found." });

            return Ok(new { data = pdiscount });
        }

        [HttpPost("Add")]
        public IActionResult AddPDiscount([FromBody] PDiscountDto pdiscountDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kiểm tra nếu sản phẩm & giảm giá có tồn tại
            if (!_dbc.Products.Any(p => p.ProductId == pdiscountDto.ProductID))
                return BadRequest(new { message = "ProductID does not exist." });

            if (!_dbc.Discounts.Any(d => d.DiscountId == pdiscountDto.DiscountID))
                return BadRequest(new { message = "DiscountID does not exist." });

            // Kiểm tra nếu giảm giá đã tồn tại cho sản phẩm này
            if (_dbc.ProductDiscounts.Any(pd => pd.ProductId == pdiscountDto.ProductID && pd.DiscountId == pdiscountDto.DiscountID))
                return Conflict(new { message = "This discount is already applied to the product." });

            var pdiscount = new ProductDiscount
            {
                DiscountId = pdiscountDto.DiscountID,
                ProductId = pdiscountDto.ProductID
            };

            _dbc.ProductDiscounts.Add(pdiscount);
            _dbc.SaveChanges();
            return CreatedAtAction(nameof(GetPDiscountById), new { productId = pdiscount.ProductId, discountId = pdiscount.DiscountId }, pdiscount);
        }

        [HttpPut("Update/{productId}/{discountId}")]
        public IActionResult UpdatePDiscount(int productId, int discountId, [FromBody] PDiscountDto pdiscountDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingPDiscount = _dbc.ProductDiscounts
                .FirstOrDefault(pd => pd.ProductId == productId && pd.DiscountId == discountId);

            if (existingPDiscount == null)
                return NotFound(new { message = "Discount not found for this product." });

            existingPDiscount.DiscountId = pdiscountDto.DiscountID;
            existingPDiscount.ProductId = pdiscountDto.ProductID;

            _dbc.ProductDiscounts.Update(existingPDiscount);
            _dbc.SaveChanges();
            return Ok(new { data = existingPDiscount });
        }

        [HttpDelete("Delete/{productId}/{discountId}")]
        public IActionResult DeletePDiscount(int productId, int discountId)
        {
            var pdiscount = _dbc.ProductDiscounts
                .SingleOrDefault(pd => pd.ProductId == productId && pd.DiscountId == discountId);

            if (pdiscount == null)
                return NotFound(new { message = "Discount not found for this product." });

            _dbc.ProductDiscounts.Remove(pdiscount);
            _dbc.SaveChanges();
            return Ok(new { message = "Discount removed from product successfully." });
        }
    }

    public class PDiscountDto
    {
        [Required]
        public int DiscountID { get; set; }

        [Required]
        public int ProductID { get; set; }
    }
}
