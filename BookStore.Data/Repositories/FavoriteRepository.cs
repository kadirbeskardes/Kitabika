using BookStore.Core.Entities;
using BookStore.Core.Interfaces;
using BookStore.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Data.Repositories
{
    public class FavoriteRepository : Repository<Favorite>, IFavoriteRepository
    {
        private readonly BookStoreContext _context;

        public FavoriteRepository(BookStoreContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Favorite>> GetUserFavoritesAsync(int userId)
        {
            return await _context.Favorites
                .Include(f => f.Book)
                .ThenInclude(b => b!.Category)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.AddedDate)
                .ToListAsync();
        }

        public async Task<bool> IsBookInFavoritesAsync(int userId, int bookId)
        {
            return await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.BookId == bookId);
        }

        public async Task RemoveFromFavoritesAsync(int userId, int bookId)
        {
            var favoriteItem = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.BookId == bookId);
            
            if (favoriteItem != null)
            {
                _context.Favorites.Remove(favoriteItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<int>> GetMostFavoritedBookIdsAsync(int count = 10)
        {
            return await _context.Favorites
                .GroupBy(f => f.BookId)
                .OrderByDescending(g => g.Count())
                .Take(count)
                .Select(g => g.Key)
                .ToListAsync();
        }
    }
}