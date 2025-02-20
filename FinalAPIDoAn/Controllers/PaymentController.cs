using FinalAPIDoAn.MyModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client.AppConfig;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly KetNoiCSDL _dbc;
        public PaymentController(KetNoiCSDL dbc)
        {
            _dbc = dbc;
        }
        [HttpGet("List")]
        public IActionResult GetAllPayment()
        {
            var payment = _dbc.Payments.ToList();
            return Ok(new { data = payment });
        }
        [HttpGet("Search")]
        public IActionResult GetPaymentById(int paymentid)
        {
            var payment = _dbc.Payments.SingleOrDefault(o => o.PaymentId == paymentid);
            if (payment == null) return NotFound(new { message = "Order not found." });

            return Ok(new { data = payment});
        }
        [HttpPost("Add")]
        public IActionResult AddPayment([FromBody] PaymentDto paymentDto)
        {
            if (paymentDto == null || paymentDto.PaymentID <= 0 || paymentDto.OrderID <= 0 || paymentDto.PaymentDate == default || string.IsNullOrWhiteSpace(paymentDto.PaymentMethod) || paymentDto.Amount <= 0 || string.IsNullOrWhiteSpace(paymentDto.PaymentStatus))
            {
                return BadRequest(new { message = "Invalid payment data." });
            }
            var payment = new Payment
            {
                PaymentId = paymentDto.PaymentID,
                OrderId = paymentDto.OrderID,
                PaymentDate = paymentDto.PaymentDate,
                PaymentMethod = paymentDto.PaymentMethod,
                Amount = paymentDto.Amount,
                PaymentStatus = paymentDto.PaymentStatus ?? "Pending"
            };
            _dbc.Payments.Add(payment);
            _dbc.SaveChanges();
            return CreatedAtAction(nameof(GetPaymentById), new { id = payment.PaymentId }, payment);
        }
        [HttpPut("Update")]
        public IActionResult UpdatePayment([FromBody] PaymentDto paymentDto)
        {
            if (paymentDto == null || paymentDto.PaymentID <= 0 || paymentDto.OrderID <= 0 || paymentDto.PaymentDate == default || string.IsNullOrWhiteSpace(paymentDto.PaymentMethod) || paymentDto.Amount <= 0 || string.IsNullOrWhiteSpace(paymentDto.PaymentStatus))
            {
                return BadRequest(new { message = "Invalid payment data." });
            }
            var payment = new Payment
            {
                PaymentId = paymentDto.PaymentID,
                OrderId = paymentDto.OrderID,
                PaymentDate = paymentDto.PaymentDate,
                PaymentMethod = paymentDto.PaymentMethod,
                Amount = paymentDto.Amount,
                PaymentStatus = paymentDto.PaymentStatus
            };
            _dbc.Payments.Update(payment);
            _dbc.SaveChanges();
            return CreatedAtAction(nameof(GetPaymentById), new { id = payment.PaymentId }, payment);
            
        }
        [HttpDelete("Delete")]
        public IActionResult DeletePayment(int paymentid)
        {
            var payment = _dbc.Payments.SingleOrDefault(o => o.PaymentId== paymentid);
            if (payment == null) return NotFound(new { message = "Order detail not found." });
            _dbc.Payments.Remove(payment);
            _dbc.SaveChanges();
            return Ok(new { message = "Payment deleted successfully." });
        }
    }
    public class PaymentDto
    {
        public required int PaymentID { get; set; }
        public required int OrderID { get; set; }
        public required DateTime PaymentDate { get; set; }
        public required string PaymentMethod { get; set; }
        public required int Amount { get; set; }
        public required string PaymentStatus { get; set; }
    }
}
