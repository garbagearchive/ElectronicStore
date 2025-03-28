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

        // ------------------ ĐƠN HÀNG ------------------ //

        [HttpGet]
        public IActionResult GetAllOrders()
        {
            var orders = _dbc.Orders.ToList();
            return Ok(new { data = orders });
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            var order = _dbc.Orders.SingleOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound(new { message = "Order not found." });
            }
            return Ok(new { data = order });
        }

        [HttpPost]
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

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
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

        // ------------------ CHI TIẾT ĐƠN HÀNG ------------------ //

        [HttpGet("{orderId}/details")]
        public IActionResult GetOrderDetails(int orderId)
        {
            var orderDetails = _dbc.OrderDetails.Where(od => od.OrderId == orderId).ToList();
            if (!orderDetails.Any())
            {
                return NotFound(new { message = "No order details found for this order." });
            }
            return Ok(new { data = orderDetails });
        }

        [HttpGet("details/{id}")]
        public IActionResult GetOrderDetailById(int id)
        {
            var orderDetail = _dbc.OrderDetails.SingleOrDefault(od => od.OrderDetailId == id);
            if (orderDetail == null)
            {
                return NotFound(new { message = "Order detail not found." });
            }
            return Ok(new { data = orderDetail });
        }

        [HttpPost("{orderId}/details")]
        public IActionResult AddOrderDetail(int orderId, [FromBody] OrderDetailDto orderDetailDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_dbc.Orders.Any(o => o.OrderId == orderId) ||
                !_dbc.Products.Any(p => p.ProductId == orderDetailDto.ProductID))
            {
                return BadRequest(new { message = "OrderID or ProductID does not exist." });
            }

            var orderDetail = new OrderDetail
            {
                OrderId = orderId,
                ProductId = orderDetailDto.ProductID,
                Quantity = orderDetailDto.Quantity,
                Price = orderDetailDto.Price
            };

            _dbc.OrderDetails.Add(orderDetail);
            _dbc.SaveChanges();
            return CreatedAtAction(nameof(GetOrderDetailById), new { id = orderDetail.OrderDetailId }, orderDetail);
        }

        [HttpPut("details/{id}")]
        public IActionResult UpdateOrderDetail(int id, [FromBody] OrderDetailDto orderDetailDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderDetail = _dbc.OrderDetails.FirstOrDefault(od => od.OrderDetailId == id);
            if (orderDetail == null)
            {
                return NotFound(new { message = "Order detail not found." });
            }

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

        [HttpDelete("details/{id}")]
        public IActionResult DeleteOrderDetail(int id)
        {
            var orderDetail = _dbc.OrderDetails.FirstOrDefault(od => od.OrderDetailId == id);
            if (orderDetail == null)
            {
                return NotFound(new { message = "Order detail not found." });
            }

            _dbc.OrderDetails.Remove(orderDetail);
            _dbc.SaveChanges();
            return Ok(new { message = "Order detail deleted successfully." });
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
