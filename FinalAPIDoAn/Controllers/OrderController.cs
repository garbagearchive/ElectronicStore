using FinalAPIDoAn.MyModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

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
        public IActionResult GetAllOrder()
        {
            var order = _dbc.Orders.ToList();
            return Ok(new { data = order });
        }
        [HttpGet("Search")]
        public IActionResult GetOrderById(int id)
        {
            var order = _dbc.Orders.SingleOrDefault(o => o.OrderId == id);
            if (order == null) return NotFound(new { message = "Order not found." });

            return Ok(new { data = order });
        }
        [HttpPost("Add")]
        public IActionResult AddOrder([FromBody] OrderDto orderDto)
        {
            if (orderDto == null || orderDto.UserID <= 0 || orderDto.TotalAmount <= 0)
            {
                return BadRequest(new { message = "Invalid order data." });
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
        [HttpPut("Update")]
        public IActionResult UpdateOrder([FromBody] OrderDto orderDto)
        {
            if (orderDto == null || orderDto.UserID <= 0 || orderDto.TotalAmount <= 0)
            {
                return BadRequest(new { message = "Invalid order data." });
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
        [HttpDelete("Delete")]
        public IActionResult DeleteOrder(int orderid)
        {
            var order = _dbc.Orders.SingleOrDefault(o => o.OrderId == orderid);
            if (order == null) return NotFound(new { message = "Order not found." });
            _dbc.Orders.Remove(order);
            _dbc.SaveChanges();

            return Ok(new { message = "Order deleted successfully." });
        }
    }
    public class OrderDto
    {
        public required int OrderID { get; set; }
        public required int UserID { get; set; }
        public required DateTime OrderDate { get; set; }
        public required int TotalAmount { get; set; }
        public required string OrderStatus { get; set; }
        public required string PaymentStatus { get; set; }
        public required string ShippingStatus { get; set; }
    }
    }

//testing git

