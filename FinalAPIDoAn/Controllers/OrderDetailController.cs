using FinalAPIDoAn.Data;
using FinalAPIDoAn.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/orderdetails")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly KetNoiCSDL _dbc;

        public OrderDetailController(KetNoiCSDL dbc)
        {
            _dbc = dbc;
        }

        [HttpGet("List")]
        public IActionResult GetAllOrderDetails()
        {
            var orderDetails = _dbc.OrderDetails.ToList();
            return Ok(new { data = orderDetails });
        }

        [HttpGet("Search/{id}")]
        public IActionResult GetOrderDetailById(int id)
        {
            var orderDetail = _dbc.OrderDetails.SingleOrDefault(o => o.OrderDetailId == id);
            if (orderDetail == null)
            {
                return NotFound(new { message = "Order detail not found." });
            }
            return Ok(new { data = orderDetail });
        }

        [HttpPost("Add")]
        public IActionResult AddOrderDetail([FromBody] OrderDetailDto orderDetailDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra xem OrderID và ProductID có tồn tại không
            if (!_dbc.Orders.Any(o => o.OrderId == orderDetailDto.OrderID) ||
                !_dbc.Products.Any(p => p.ProductId == orderDetailDto.ProductID))
            {
                return BadRequest(new { message = "OrderID or ProductID does not exist." });
            }

            var orderDetail = new OrderDetail
            {
                OrderId = orderDetailDto.OrderID,
                ProductId = orderDetailDto.ProductID,
                Quantity = orderDetailDto.Quantity,
                Price = orderDetailDto.Price
            };

            _dbc.OrderDetails.Add(orderDetail);
            _dbc.SaveChanges();
            return CreatedAtAction(nameof(GetOrderDetailById), new { id = orderDetail.OrderDetailId }, orderDetail);
        }

        [HttpPut("Update/{id}")]
        public IActionResult UpdateOrderDetail(int id, [FromBody] OrderDetailDto orderDetailDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderDetail = _dbc.OrderDetails.FirstOrDefault(o => o.OrderDetailId == id);
            if (orderDetail == null)
            {
                return NotFound(new { message = "Order detail not found." });
            }

            // Kiểm tra xem OrderID và ProductID có tồn tại không
            if (!_dbc.Orders.Any(o => o.OrderId == orderDetailDto.OrderID) ||
                !_dbc.Products.Any(p => p.ProductId == orderDetailDto.ProductID))
            {
                return BadRequest(new { message = "OrderID or ProductID does not exist." });
            }

            orderDetail.OrderId = orderDetailDto.OrderID;
            orderDetail.ProductId = orderDetailDto.ProductID;
            orderDetail.Quantity = orderDetailDto.Quantity;
            orderDetail.Price = orderDetailDto.Price;

            _dbc.OrderDetails.Update(orderDetail);
            _dbc.SaveChanges();
            return Ok(new { data = orderDetail });
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteOrderDetail(int id)
        {
            var orderDetail = _dbc.OrderDetails.FirstOrDefault(o => o.OrderDetailId == id);
            if (orderDetail == null)
            {
                return NotFound(new { message = "Order detail not found." });
            }

            _dbc.OrderDetails.Remove(orderDetail);
            _dbc.SaveChanges();
            return Ok(new { message = "Order detail deleted successfully." });
        }
    }

    public class OrderDetailDto
    {
        [Required]
        public int OrderID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }
    }
}
