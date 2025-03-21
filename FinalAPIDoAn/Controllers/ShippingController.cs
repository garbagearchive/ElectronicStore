using FinalAPIDoAn.Data;
using FinalAPIDoAn.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/shipping")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        private readonly KetNoiCSDL _dbc;

        public ShippingController(KetNoiCSDL dbc)
        {
            _dbc = dbc;
        }

        [HttpGet("List")]
        public IActionResult GetAllShipping()
        {
            var shipping = _dbc.Shippings.ToList();
            return Ok(new { data = shipping });
        }

        [HttpGet("Search/{id}")]
        public IActionResult GetShippingById(int id)
        {
            var shipping = _dbc.Shippings.SingleOrDefault(o => o.ShippingId == id);
            if (shipping == null)
                return NotFound(new { message = "Shipping not found." });

            return Ok(new { data = shipping });
        }

        [HttpPost("Add")]
        public IActionResult AddShipping([FromBody] ShippingDto shippingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_dbc.Orders.Any(o => o.OrderId == shippingDto.OrderId))
                return BadRequest(new { message = "OrderID does not exist." });

            var shipping = new Shipping
            {
                OrderId = shippingDto.OrderId,
                ShippingAddress = shippingDto.ShippingAddress,
                ShippingMethod = shippingDto.ShippingMethod,
                TrackingNumber = shippingDto.TrackingNumber,
                EstimatedDeliveryDate = shippingDto.EstimatedDeliveryDate,
                ShippingStatus = shippingDto.ShippingStatus ?? "Delivering"
            };

            _dbc.Shippings.Add(shipping);
            _dbc.SaveChanges();

            return CreatedAtAction(nameof(GetShippingById), new { id = shipping.ShippingId }, shipping);
        }

        [HttpPut("Update/{id}")]
        public IActionResult UpdateShipping(int id, [FromBody] ShippingDto shippingDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var shipping = _dbc.Shippings.SingleOrDefault(o => o.ShippingId == id);
            if (shipping == null)
                return NotFound(new { message = "Shipping not found." });

            shipping.OrderId = shippingDto.OrderId;
            shipping.ShippingAddress = shippingDto.ShippingAddress;
            shipping.ShippingMethod = shippingDto.ShippingMethod;
            shipping.TrackingNumber = shippingDto.TrackingNumber;
            shipping.EstimatedDeliveryDate = shippingDto.EstimatedDeliveryDate;
            shipping.ShippingStatus = shippingDto.ShippingStatus ?? "Delivering";

            _dbc.Shippings.Update(shipping);
            _dbc.SaveChanges();

            return Ok(new { message = "Shipping updated successfully.", data = shipping });
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteShipping(int id)
        {
            var shipping = _dbc.Shippings.SingleOrDefault(o => o.ShippingId == id);
            if (shipping == null)
                return NotFound(new { message = "Shipping not found." });

            _dbc.Shippings.Remove(shipping);
            _dbc.SaveChanges();

            return Ok(new { message = "Shipping deleted successfully." });
        }
    }

    public class ShippingDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public string ShippingAddress { get; set; }

        [Required]
        public string ShippingMethod { get; set; }

        [Required]
        public string TrackingNumber { get; set; }

        [Required]
        public DateOnly EstimatedDeliveryDate { get; set; }

        public string? ShippingStatus { get; set; }
    }
}
