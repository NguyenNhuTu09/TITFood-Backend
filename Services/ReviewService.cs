using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TITFood_Backend.Data;
using TITFood_Backend.Entities;
using TITFood_Backend.Interfaces;
using TITFood_Backend.Models;

namespace TITFood_Backend.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IRestaurantService _restaurantService; // To update restaurant rating

        public ReviewService(ApplicationDbContext context, IMapper mapper, IRestaurantService restaurantService)
        {
            _context = context;
            _mapper = mapper;
            _restaurantService = restaurantService;
        }

        public async Task<ReviewDto?> CreateReviewAsync(CreateReviewDto createReviewDto, string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new ArgumentException("Người dùng không tồn tại.");

            var restaurant = await _context.Restaurants.FindAsync(createReviewDto.RestaurantId);
            if (restaurant == null) throw new ArgumentException("Nhà hàng không tồn tại.");

            // Optional: Check if user has ordered from this restaurant before allowing review
            // var hasOrdered = await _context.Orders.AnyAsync(o => o.UserId == userId && o.RestaurantId == createReviewDto.RestaurantId && o.Status == OrderStatus.Delivered);
            // if (!hasOrdered) throw new InvalidOperationException("Bạn cần đặt hàng và nhận hàng từ nhà hàng này trước khi đánh giá.");


            var review = _mapper.Map<Review>(createReviewDto);
            review.UserId = userId;
            review.ReviewDate = DateTime.UtcNow;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            await UpdateRestaurantRatingAsync(createReviewDto.RestaurantId);
            
            // Eager load related data for the DTO
            var createdReview = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Restaurant)
                .FirstOrDefaultAsync(r => r.Id == review.Id);

            return _mapper.Map<ReviewDto>(createdReview);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByRestaurantAsync(int restaurantId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.RestaurantId == restaurantId)
                .Include(r => r.User) // Eager load user info
                .Include(r => r.Restaurant)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto?> GetReviewByIdAsync(int reviewId)
        {
             var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Restaurant)
                .FirstOrDefaultAsync(r => r.Id == reviewId);
            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<bool> UpdateReviewAsync(int reviewId, UpdateReviewDto updateReviewDto, string userId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return false;
            if (review.UserId != userId) return false; // User can only update their own review

            _mapper.Map(updateReviewDto, review);
            review.ReviewDate = DateTime.UtcNow; // Update review date
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();

            await UpdateRestaurantRatingAsync(review.RestaurantId);
            return true;
        }

        public async Task<bool> DeleteReviewAsync(int reviewId, string userId, bool isAdmin)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null) return false;

            if (!isAdmin && review.UserId != userId) return false; // User can only delete their own review, admin can delete any

            int restaurantId = review.RestaurantId; // Store before deleting
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            await UpdateRestaurantRatingAsync(restaurantId);
            return true;
        }

        private async Task UpdateRestaurantRatingAsync(int restaurantId)
        {
            var restaurant = await _context.Restaurants.Include(r => r.Reviews).FirstOrDefaultAsync(r => r.Id == restaurantId);
            if (restaurant != null)
            {
                if (restaurant.Reviews.Any())
                {
                    restaurant.Rating = Math.Round(restaurant.Reviews.Average(r => r.Rating), 1);
                }
                else
                {
                    restaurant.Rating = 0;
                }
                _context.Restaurants.Update(restaurant);
                await _context.SaveChangesAsync();
            }
        }
    }
}