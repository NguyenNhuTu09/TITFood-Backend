using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TITFood_Backend.Interfaces;
using TITFood_Backend.Models;
using TITFood_Backend.Common;

namespace TITFood_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize] // User must be logged in to create a review
        public async Task<ActionResult<ReviewDto>> CreateReview([FromBody] CreateReviewDto createReviewDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized("Không thể xác định người dùng.");

            try
            {
                var createdReview = await _reviewService.CreateReviewAsync(createReviewDto, userId);
                if (createdReview == null) return BadRequest("Không thể tạo đánh giá.");
                return CreatedAtAction(nameof(GetReviewById), new { reviewId = createdReview.Id }, createdReview);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                 return Conflict(new { Message = ex.Message }); // e.g., user hasn't ordered yet
            }
        }

        [HttpGet("restaurant/{restaurantId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsForRestaurant(int restaurantId)
        {
            var reviews = await _reviewService.GetReviewsByRestaurantAsync(restaurantId);
            return Ok(reviews);
        }

        [HttpGet("{reviewId}")]
        public async Task<ActionResult<ReviewDto>> GetReviewById(int reviewId)
        {
            var review = await _reviewService.GetReviewByIdAsync(reviewId);
            if (review == null) return NotFound("Không tìm thấy đánh giá.");
            return Ok(review);
        }

        [HttpPut("{reviewId}")]
        [Authorize] // User must be logged in
        public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] UpdateReviewDto updateReviewDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _reviewService.UpdateReviewAsync(reviewId, updateReviewDto, userId);
            if (!success) return Forbid("Không thể cập nhật đánh giá này hoặc không tìm thấy.");
            return Ok(new { Message = "Đánh giá đã được cập nhật."});
        }

        [HttpDelete("{reviewId}")]
        [Authorize] // User must be logged in
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            bool isAdmin = User.IsInRole(AppRole.Admin);
            var success = await _reviewService.DeleteReviewAsync(reviewId, userId, isAdmin);

            if (!success) return Forbid("Không thể xóa đánh giá này hoặc không tìm thấy.");
            return NoContent();
        }
    }
}