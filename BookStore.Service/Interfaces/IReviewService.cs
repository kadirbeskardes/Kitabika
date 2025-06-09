using BookStore.Service.DTOs;

namespace BookStore.Service.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetReviewsByBookAsync(int bookId);
        Task<IEnumerable<ReviewDto>> GetReviewsByUserAsync(int userId);
        Task<ReviewDto> GetReviewByIdAsync(int id);
        Task<ReviewDto> AddReviewAsync(CreateReviewDto createReviewDto, int userId);
        Task UpdateReviewAsync(int id, UpdateReviewDto updateReviewDto, int userId);
        Task DeleteReviewAsync(int id, int userId);
    }
}
