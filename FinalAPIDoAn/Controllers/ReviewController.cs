using FinalAPIDoAn.Data;
using FinalAPIDoAn.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly KetNoiCSDL _dbc;

        public ReviewController(KetNoiCSDL dbc)
        {
            _dbc = dbc;
        }

        [HttpGet("List")]
        public IActionResult GetAllReviews()
        {
            var reviews = _dbc.ProductReviews.ToList();
            return Ok(new { data = reviews });
        }

        [HttpGet("Search/{reviewId}")]
        public IActionResult GetReviewById(int reviewId)
        {
            var review = _dbc.ProductReviews.SingleOrDefault(o => o.ReviewId == reviewId);
            if (review == null)
                return NotFound(new { message = "Review not found." });

            return Ok(new { data = review });
        }

        [HttpPost("Add")]
        public IActionResult AddReview([FromBody] ReviewDto reviewDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Kiểm tra nếu ProductID và UserID có tồn tại
            if (!_dbc.Products.Any(p => p.ProductId == reviewDto.ProductId))
                return BadRequest(new { message = "ProductID does not exist." });

            if (!_dbc.Users.Any(u => u.UserId == reviewDto.UserId))
                return BadRequest(new { message = "UserID does not exist." });

            var review = new ProductReview
            {
                ProductId = reviewDto.ProductId,
                UserId = reviewDto.UserId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment
            };

            _dbc.ProductReviews.Add(review);
            _dbc.SaveChanges();
            return CreatedAtAction(nameof(GetReviewById), new { reviewId = review.ReviewId }, review);
        }

        [HttpDelete("Delete/{reviewId}")]
        public IActionResult DeleteReview(int reviewId)
        {
            var review = _dbc.ProductReviews.SingleOrDefault(o => o.ReviewId == reviewId);
            if (review == null)
                return NotFound(new { message = "Review not found." });

            _dbc.ProductReviews.Remove(review);
            _dbc.SaveChanges();
            return Ok(new { message = "Review deleted successfully." });
        }
    }

    public class ReviewDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required]
        public string Comment { get; set; }
    }
}
