using BookStore.Core.Entities;

namespace BookStore.Core.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByBookAsync(int bookId);
        Task<IEnumerable<Review>> GetReviewsByUserAsync(int userId);
    }
}
