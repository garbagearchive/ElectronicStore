using FinalAPIDoAn.MyModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly KetNoiCSDL _dbc;
        public ReviewController(KetNoiCSDL dbc)
        {
            _dbc = dbc;
        }
        [HttpGet("List")]
        public IActionResult GetReview()
        { var review = _dbc.ProductReviews.ToList();
            return Ok(new { data = review });
        }
        [HttpGet("Search")]
        public IActionResult GetReviewById(int reviewid) {
            {
                var review = _dbc.ProductReviews.SingleOrDefault(o => o.ReviewId == reviewid);
                if (review == null) return NotFound(new { message = "Order not found." });
                return Ok(new { data = review });
            }

        }
        [HttpPost("Add")]
        public IActionResult AddReview([FromBody] ReviewDto reviewDto)
        {
            if (reviewDto.ReviewID <= 0 || reviewDto.ProductID <= 0 || reviewDto.UserID <= 0 ||reviewDto.Rating <=0 ||reviewDto.Rating >5 || string.IsNullOrWhiteSpace(reviewDto.Comment))
            {
                return BadRequest(new { message = "Invalid review data" });
            }
            var review = new ProductReview
            {
                ReviewId = reviewDto.ReviewID,
                ProductId = reviewDto.ProductID,
                UserId = reviewDto.ReviewID,
                Rating = reviewDto.ReviewID,
                Comment = reviewDto.Comment
            };
            _dbc.ProductReviews.Add(review);
            _dbc.SaveChanges();
            return CreatedAtAction(nameof(GetReviewById), new { id = review.ReviewId }, review);
        }
        [HttpDelete("Delete")]
        public IActionResult DeleteReview(int reviewid)
        {
            var review = _dbc.ProductReviews.SingleOrDefault(o => o.ReviewId == reviewid);
            if (review == null) return NotFound(new { message = "Order not found." });
            _dbc.ProductReviews.Remove(review);
            _dbc.SaveChanges();
            return Ok(new { message = "Review deleted successfully." });
        }
        public class ReviewDto
        {
            public required int ReviewID { get; set; }
            public required int ProductID { get; set; }
            public required int UserID { get; set; }
            public required int Rating { get; set; }
            public required string Comment { get; set; }
        }
    }
}
