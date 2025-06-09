using BookStore.Core.Entities;
using BookStore.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Data.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(BookStoreContext context) : base(context) { }

        public async Task<IEnumerable<Review>> GetReviewsByBookAsync(int bookId)
        {
            return await _context.Reviews
                .Where(r => r.BookId == bookId)
                .Include(r => r.User)
                .Include(r => r.Book)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserAsync(int userId)
        {
            return await _entities
                .Where(r => r.UserId == userId)
                .Include(r => r.Book)
                .ToListAsync();
        }
    }
}
