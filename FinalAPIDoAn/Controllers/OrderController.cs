using FinalAPIDoAn.Data;
using FinalAPIDoAn.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly KetNoiCSDL _dbc;

        public OrderController(KetNoiCSDL dbc)
        {
            _dbc = dbc;
        }

        [HttpGet("List")]
        public IActionResult GetAllOrders()
        {
            var orders = _dbc.Orders.ToList();
            return Ok(new { data = orders });
        }

        [HttpGet("Search/{id}")]
        public IActionResult GetOrderById(int id)
        {
            var order = _dbc.Orders.SingleOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound(new { message = "Order not found." });
            }
            return Ok(new { data = order });
        }

        [HttpPost("Add")]
        public IActionResult AddOrder([FromBody] OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = new Order
            {
                UserId = orderDto.UserID,
                OrderDate = orderDto.OrderDate == default ? DateTime.UtcNow : orderDto.OrderDate,
                TotalAmount = orderDto.TotalAmount,
                OrderStatus = orderDto.OrderStatus ?? "Pending",
                PaymentStatus = orderDto.PaymentStatus ?? "Unpaid",
                ShippingStatus = orderDto.ShippingStatus ?? "Not Shipped"
            };

            _dbc.Orders.Add(order);
            _dbc.SaveChanges();

            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }

        [HttpPut("Update/{id}")]
        public IActionResult UpdateOrder(int id, [FromBody] OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = _dbc.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound(new { message = "Order not found." });
            }

            order.UserId = orderDto.UserID;
            order.OrderDate = orderDto.OrderDate == default ? DateTime.UtcNow : orderDto.OrderDate;
            order.TotalAmount = orderDto.TotalAmount;
            order.OrderStatus = orderDto.OrderStatus ?? "Pending";
            order.PaymentStatus = orderDto.PaymentStatus ?? "Unpaid";
            order.ShippingStatus = orderDto.ShippingStatus ?? "Not Shipped";

            _dbc.Orders.Update(order);
            _dbc.SaveChanges();

            return Ok(new { data = order });
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult DeleteOrder(int id)
        {
            var order = _dbc.Orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound(new { message = "Order not found." });
            }

            _dbc.Orders.Remove(order);
            _dbc.SaveChanges();
            return Ok(new { message = "Order deleted successfully." });
        }
    }

    public class OrderDto
    {
        [Required]
        public int UserID { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0.")]
        public decimal TotalAmount { get; set; }

        public string OrderStatus { get; set; } = "Pending";
        public string PaymentStatus { get; set; } = "Unpaid";
        public string ShippingStatus { get; set; } = "Not Shipped";
    }
}
