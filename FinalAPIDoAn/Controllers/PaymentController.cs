using FinalAPIDoAn.Data;
using FinalAPIDoAn.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly KetNoiCSDL _dbc;

        public PaymentController(KetNoiCSDL dbc)
        {
            _dbc = dbc;
        }

        [HttpGet("List")]
        public IActionResult GetAllPayments()
        {
            var payments = _dbc.Payments.ToList();
            return Ok(new { data = payments });
        }

        [HttpGet("Search/{id}")]
        public IActionResult GetPaymentById(int id)
        {
            var payment = _dbc.Payments.SingleOrDefault(p => p.PaymentId == id);
            if (payment == null)
            {
                return NotFound(new { message = "Payment not found." });
            }
            return Ok(new { data = payment });
        }

        [HttpPost("Add")]
        public IActionResult AddPayment([FromBody] PaymentDto paymentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra xem OrderID có tồn tại không
            if (!_dbc.Orders.Any(o => o.OrderId == paymentDto.OrderID))
            {
                return BadRequest(new { message = "OrderID does not exist." });
            }

            var payment = new Payment
            {
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

        [HttpPut("Update/{id}")]
        public IActionResult UpdatePayment(int id, [FromBody] PaymentDto paymentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var payment = _dbc.Payments.FirstOrDefault(p => p.PaymentId == id);
            if (payment == null)
            {
                return NotFound(new { message = "Payment not found." });
            }

            // Kiểm tra xem OrderID có tồn tại không
            if (!_dbc.Orders.Any(o => o.OrderId == paymentDto.OrderID))
            {
                return BadRequest(new { message = "OrderID does not exist." });
            }

            payment.OrderId = paymentDto.OrderID;
            payment.PaymentDate = paymentDto.PaymentDate;
            payment.PaymentMethod = paymentDto.PaymentMethod;
            payment.Amount = paymentDto.Amount;
            payment.PaymentStatus = paymentDto.PaymentStatus;

            _dbc.Payments.Update(payment);
            _dbc.SaveChanges();
            return Ok(new { data = payment });
        }

        [HttpDelete("Delete/{id}")]
        public IActionResult DeletePayment(int id)
        {
            var payment = _dbc.Payments.FirstOrDefault(p => p.PaymentId == id);
            if (payment == null)
            {
                return NotFound(new { message = "Payment not found." });
            }

            _dbc.Payments.Remove(payment);
            _dbc.SaveChanges();
            return Ok(new { message = "Payment deleted successfully." });
        }
    }

    public class PaymentDto
    {
        [Required]
        public int OrderID { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        public string? PaymentStatus { get; set; } = "Pending";
    }
}
