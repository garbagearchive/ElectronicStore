using FinalAPIDoAn.Data;
using FinalAPIDoAn.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FinalAPIDoAn.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly KetNoiCSDL _dbc;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(KetNoiCSDL dbc, ILogger<ReviewController> logger)
        {
            _dbc = dbc;
            _logger = logger;
        }

        // GET: api/reviews/List
        [HttpGet("List")]
        public async Task<IActionResult> GetAllReviews()
        {
            try
            {
                var reviews = await _dbc.ProductReviews
                    .Include(r => r.ReviewImages)
                    .Select(r => new ReviewResponseDto
                    {
                        ReviewId = r.ReviewId,
                        ProductId = r.ProductId,
                        UserId = r.UserId,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        ReviewDate = r.ReviewDate ?? DateTime.UtcNow,
                        Images = r.ReviewImages.Select(i => new ReviewImageDto
                        {
                            ImageId = i.ImageId,
                            ImageUrl = i.ImageUrl,
                            CreatedAt = i.CreatedAt
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(new { data = reviews });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all reviews");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/reviews/Get/5
        [HttpGet("Get/{reviewId}")]
        public async Task<IActionResult> GetReviewById(int reviewId)
        {
            try
            {
                var review = await _dbc.ProductReviews
                    .Include(r => r.ReviewImages)
                    .Where(r => r.ReviewId == reviewId)
                    .Select(r => new ReviewResponseDto
                    {
                        ReviewId = r.ReviewId,
                        ProductId = r.ProductId,
                        UserId = r.UserId,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        ReviewDate = r.ReviewDate ?? DateTime.UtcNow,
                        Images = r.ReviewImages.Select(i => new ReviewImageDto
                        {
                            ImageId = i.ImageId,
                            ImageUrl = i.ImageUrl,
                            CreatedAt = i.CreatedAt
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (review == null)
                    return NotFound(new { message = "Review not found." });

                return Ok(new { data = review });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting review with ID {reviewId}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/reviews/Add
        [HttpPost("Add")]
        public async Task<IActionResult> AddReview([FromBody] ReviewDto reviewDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var transaction = await _dbc.Database.BeginTransactionAsync();

            try
            {
                // Explicitly convert nullable ProductId to non-nullable
                var productId = reviewDto.ProductId ?? throw new ArgumentException("ProductId is required");
                var userId = reviewDto.UserId ?? throw new ArgumentException("UserId is required");

                if (!await _dbc.Products.AnyAsync(p => p.ProductId == productId))
                    return BadRequest(new { message = "ProductID does not exist." });

                if (!await _dbc.Users.AnyAsync(u => u.UserId == userId))
                    return BadRequest(new { message = "UserID does not exist." });

                var review = new ProductReview
                {
                    ProductId = productId,
                    UserId = userId,
                    Rating = reviewDto.Rating ?? 1, // Default to 1 if null
                    Comment = reviewDto.Comment,
                    ReviewDate = DateTime.UtcNow
                };

                await _dbc.ProductReviews.AddAsync(review);
                await _dbc.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetReviewById),
                    new { reviewId = review.ReviewId },
                    new { data = review });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error adding review");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/reviews/Update/5
        [HttpPut("Update/{reviewId}")]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] ReviewDto reviewDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var transaction = await _dbc.Database.BeginTransactionAsync();

            try
            {
                var review = await _dbc.ProductReviews.FindAsync(reviewId);
                if (review == null)
                    return NotFound(new { message = "Review not found." });

                // Explicit conversion with null checks
                if (reviewDto.ProductId.HasValue)
                {
                    if (!await _dbc.Products.AnyAsync(p => p.ProductId == reviewDto.ProductId))
                        return BadRequest(new { message = "ProductID does not exist." });
                    review.ProductId = reviewDto.ProductId.Value;
                }

                if (reviewDto.UserId.HasValue)
                {
                    if (!await _dbc.Users.AnyAsync(u => u.UserId == reviewDto.UserId))
                        return BadRequest(new { message = "UserID does not exist." });
                    review.UserId = reviewDto.UserId.Value;
                }

                if (reviewDto.Rating.HasValue)
                    review.Rating = reviewDto.Rating.Value;

                if (!string.IsNullOrEmpty(reviewDto.Comment))
                    review.Comment = reviewDto.Comment;

                _dbc.ProductReviews.Update(review);
                await _dbc.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { data = review });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error updating review {reviewId}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/reviews/Delete/5
        [HttpDelete("Delete/{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            using var transaction = await _dbc.Database.BeginTransactionAsync();

            try
            {
                var review = await _dbc.ProductReviews
                    .Include(r => r.ReviewImages)
                    .FirstOrDefaultAsync(r => r.ReviewId == reviewId);

                if (review == null)
                    return NotFound(new { message = "Review not found." });

                _dbc.ReviewImages.RemoveRange(review.ReviewImages);
                _dbc.ProductReviews.Remove(review);

                await _dbc.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Review deleted successfully." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error deleting review {reviewId}");
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class ReviewDto
    {
        public int? ProductId { get; set; }

        public int? UserId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? Rating { get; set; }

        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
        public string Comment { get; set; }
    }

    public class ReviewResponseDto
    {
        public int? ReviewId { get; set; }
        public int? ProductId { get; set; }
        public int? UserId { get; set; }
        public int? Rating { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public List<ReviewImageDto> Images { get; set; } = new List<ReviewImageDto>();
    }

    public class ReviewImageDto
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}