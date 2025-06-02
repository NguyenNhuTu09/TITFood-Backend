using System.Collections.Generic;
using System.Threading.Tasks;
using TITFood_Backend.Models;
using Microsoft.AspNetCore.Identity; // For IdentityResult

namespace TITFood_Backend.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDto?> CreateReviewAsync(CreateReviewDto createReviewDto, string userId);
        Task<IEnumerable<ReviewDto>> GetReviewsByRestaurantAsync(int restaurantId);
        Task<ReviewDto?> GetReviewByIdAsync(int reviewId);
        Task<bool> UpdateReviewAsync(int reviewId, UpdateReviewDto updateReviewDto, string userId);
        Task<bool> DeleteReviewAsync(int reviewId, string userId, bool isAdmin);
    }
}