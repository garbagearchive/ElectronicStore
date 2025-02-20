using FinalAPIDoAn.MyModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/[controller]")]
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
            var orderDetail = _dbc.OrderDetails.ToList();
            return Ok(new { data = orderDetail });
        }
        [HttpGet("Search")]
        public IActionResult GetOrderDetailById(int id)
        {
            var order = _dbc.OrderDetails.SingleOrDefault(o => o.OrderDetailId == id);
            if (order == null) return NotFound(new { message = "Order detail not found." });

            return Ok(new { data = order });
        }
        [HttpPost("Add")]
        public IActionResult AddOrderDetail([FromBody] OrderDetailDto orderDetailDto)
        {
            if (orderDetailDto == null || orderDetailDto.ProductID <= 0 || orderDetailDto.OrderID <= 0 || orderDetailDto.Quantity <= 0 || orderDetailDto.Price <= 0)
            {
                return BadRequest(new { message = "Invalid order detail data." });
            }
            var orderDetail = new OrderDetail
            {
                OrderDetailId = orderDetailDto.OrderDetailID,
                OrderId = orderDetailDto.OrderID,
                ProductId = orderDetailDto.ProductID,
                Quantity = orderDetailDto.Quantity,
                Price = orderDetailDto.Price
            };
            _dbc.OrderDetails.Add(orderDetail);
            _dbc.SaveChanges();
            return CreatedAtAction(nameof(GetOrderDetailById), new { id = orderDetail.OrderDetailId }, orderDetail);
        }
        [HttpPut("Update")]
        public IActionResult UpdateOrderDetail([FromBody] OrderDetailDto orderDetailDto)
        {
            if (orderDetailDto == null || orderDetailDto.ProductID <= 0 || orderDetailDto.OrderID <= 0 || orderDetailDto.Quantity <= 0 || orderDetailDto.Price <= 0)
            {
                return BadRequest(new { message = "Invalid order detail data." });
            }
            var orderDetail = new OrderDetail
            {
                OrderDetailId = orderDetailDto.OrderDetailID,
                OrderId = orderDetailDto.OrderID,
                ProductId = orderDetailDto.ProductID,
                Quantity = orderDetailDto.Quantity,
                Price = orderDetailDto.Price
            };
            _dbc.OrderDetails.Update(orderDetail);
            _dbc.SaveChanges();
            return CreatedAtAction(nameof(GetOrderDetailById), new { id = orderDetail.OrderDetailId }, orderDetail);
        }
        [HttpDelete("Delete")]
        public IActionResult DeleteOrderDetail(int orderdetailid)
        {
            var orderDetail = _dbc.OrderDetails.SingleOrDefault(o => o.OrderDetailId == orderdetailid);
            if (orderDetail == null) return NotFound(new { message = "Order detail not found." });
            _dbc.OrderDetails.Remove(orderDetail);
            _dbc.SaveChanges();
            return Ok(new { message = "Order detail deleted successfully." });
        }


    }
    public class OrderDetailDto
    {
        public required int OrderDetailID { get; set; }
        public required int OrderID { get; set; }
        public required int ProductID { get; set; }
        public required int Quantity { get; set; }
        public required int Price { get; set; }
    }
}
