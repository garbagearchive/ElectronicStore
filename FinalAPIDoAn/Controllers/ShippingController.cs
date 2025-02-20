using FinalAPIDoAn.MyModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/[controller]")]
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
            return Ok(new { data = shipping});
        }
        [HttpGet("Search")]
        public IActionResult GetShippingById(int id)
        {
            var shipping = _dbc.Shippings.SingleOrDefault(o => o.ShippingId== id);
            if (shipping == null) return NotFound(new { message = "Order not found." });
            return Ok(new { data = shipping });
        }
        [HttpPost("Add")]
        public IActionResult AddShipping([FromBody] ShippingDto shippingDto)
        {
            if (shippingDto.OrderID <=0 || string.IsNullOrWhiteSpace(shippingDto.ShippingAddress) || string.IsNullOrWhiteSpace(shippingDto.ShippingMethod) || string.IsNullOrWhiteSpace(shippingDto.ShippingStatus) || string.IsNullOrWhiteSpace(shippingDto.TrackingNumber))
            {
                return BadRequest(new { message = "Invalid shipping data." });
            }

            var shipping = new Shipping
            {
                OrderId = shippingDto.OrderID,
                ShippingAddress = shippingDto.ShippingAddress,
                ShippingMethod = shippingDto.ShippingMethod,
                TrackingNumber = shippingDto.TrackingNumber,
                EstimatedDeliveryDate = shippingDto.EstimateDelivertyDate,
                ShippingStatus = shippingDto.ShippingStatus ?? "Delivering"
            };

            _dbc.Shippings.Add(shipping);
            _dbc.SaveChanges();

            return CreatedAtAction(nameof(GetShippingById), new { id = shipping.ShippingId}, shipping);
        }
        [HttpPut("Update")]
        public IActionResult UpdateShipping([FromBody] ShippingDto shippingDto)
        {
            if (shippingDto.OrderID <= 0 || string.IsNullOrWhiteSpace(shippingDto.ShippingAddress) || string.IsNullOrWhiteSpace(shippingDto.ShippingMethod) || string.IsNullOrWhiteSpace(shippingDto.ShippingStatus) || string.IsNullOrWhiteSpace(shippingDto.TrackingNumber))
            {
                return BadRequest(new { message = "Invalid shipping data." });
            }

            var shipping = new Shipping
            {
                OrderId = shippingDto.OrderID,
                ShippingAddress = shippingDto.ShippingAddress,
                ShippingMethod = shippingDto.ShippingMethod,
                TrackingNumber = shippingDto.TrackingNumber,
                EstimatedDeliveryDate = shippingDto.EstimateDelivertyDate,
                ShippingStatus = shippingDto.ShippingStatus ?? "Delivering"
            };

            _dbc.Shippings.Update(shipping);
            _dbc.SaveChanges();

            return CreatedAtAction(nameof(GetShippingById), new { id = shipping.ShippingId }, shipping);
        }
        [HttpDelete("Delete")]
        public IActionResult DeleteShipping (int shippingid)
        {
            var shipping = _dbc.Shippings.SingleOrDefault(o => o.ShippingId == shippingid);
            if (shipping == null) return NotFound(new { message = "Shipping not found." });
            _dbc.Shippings.Remove(shipping);
            _dbc.SaveChanges();
            return Ok(new { message = "Shipping deleted successfully." });
        }
    }
    public class ShippingDto
    {
        public required int ShippingID { get; set; }
        public required int OrderID { get; set; }
        public required string ShippingAddress { get; set; }
        public required string ShippingMethod { get; set; }
        public required string TrackingNumber { get; set; }
        public required DateOnly EstimateDelivertyDate { get; set; }
        public required string ShippingStatus { get; set; }


    }
}
