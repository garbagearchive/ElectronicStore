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

        // ----- CRUD CHO DISCOUNTS -----

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
                return NotFound(new { message = "Discount not found." });

            return Ok(new { data = discount });
        }

        [HttpPost("Add")]
        public IActionResult AddDiscount([FromBody] DiscountDto discountDto)
        {
            if (!ModelState.IsValid || discountDto.StartDate >= discountDto.EndDate)
                return BadRequest(new { message = "Invalid data. EndDate must be after StartDate." });

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
                return BadRequest(new { message = "Invalid data. EndDate must be after StartDate." });

            var discount = _dbc.Discounts.FirstOrDefault(d => d.DiscountId == id);
            if (discount == null)
                return NotFound(new { message = "Discount not found." });

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
                return NotFound(new { message = "Discount not found." });

            _dbc.Discounts.Remove(discount);
            _dbc.SaveChanges();
            return Ok(new { message = "Discount deleted successfully." });
        }

        // ----- CRUD CHO PRODUCT DISCOUNTS -----

        [HttpGet("product-discounts/List")]
        public IActionResult GetAllPDiscounts()
        {
            var pdiscount = _dbc.ProductDiscounts
                .Join(
                    _dbc.Discounts,
                    pd => pd.DiscountId,
                    d => d.DiscountId,
                    (pd, d) => new { ProductDiscount = pd, Discount = d }
                )
                .Join(
                    _dbc.Products,
                    combined => combined.ProductDiscount.ProductId,
                    p => p.ProductId,
                    (combined, p) => new
                    {
                        DiscountId = combined.Discount.DiscountId,
                        DiscountCode = combined.Discount.DiscountCode,
                        ProductId = p.ProductId,
                        ProductName = p.ProductName
                    }
                )
                .ToList();

            return Ok(new { data = pdiscount });
        }

        [HttpGet("product-discounts/Search/{productId}/{discountId}")]
        public IActionResult GetPDiscountById(int productId, int discountId)
        {
            var pdiscount = _dbc.ProductDiscounts
                .SingleOrDefault(o => o.ProductId == productId && o.DiscountId == discountId);

            if (pdiscount == null)
                return NotFound(new { message = "Discount not found." });

            return Ok(new { data = pdiscount });
        }

        [HttpPost("product-discounts/Add")]
        public IActionResult AddPDiscount([FromBody] PDiscountDto pdiscountDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_dbc.Products.Any(p => p.ProductId == pdiscountDto.ProductID))
                return BadRequest(new { message = "ProductID does not exist." });

            if (!_dbc.Discounts.Any(d => d.DiscountId == pdiscountDto.DiscountID))
                return BadRequest(new { message = "DiscountID does not exist." });

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

        [HttpPut("product-discounts/Update/{productId}/{discountId}")]
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

        [HttpDelete("product-discounts/Delete/{productId}/{discountId}")]
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

    public class DiscountDto
    {
        [Required]
        public required string DiscountCode { get; set; }

        public required string Description { get; set; }

        [Range(1, 100, ErrorMessage = "Discount percentage must be between 1 and 100.")]
        public int DiscountPercentage { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
