using BookStore.Core.Entities;
using BookStore.Core.Interfaces;
using BookStore.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Data.Repositories
{
    public class WishlistRepository : Repository<Wishlist>, IWishlistRepository
    {
        private readonly BookStoreContext _context;

        public WishlistRepository(BookStoreContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Wishlist>> GetUserWishlistAsync(int userId)
        {
            return await _context.Wishlists
                .Include(w => w.Book)
                .ThenInclude(b => b!.Category)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.AddedDate)
                .ToListAsync();
        }

        public async Task<bool> IsBookInWishlistAsync(int userId, int bookId)
        {
            return await _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.BookId == bookId);
        }

        public async Task RemoveFromWishlistAsync(int userId, int bookId)
        {
            var wishlistItem = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.BookId == bookId);
            
            if (wishlistItem != null)
            {
                _context.Wishlists.Remove(wishlistItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Wishlist>> GetBooksWithPriceDropAsync()
        {
            return await _context.Wishlists
                .Include(w => w.Book)
                .Include(w => w.User)
                .Where(w => w.IsNotificationEnabled && 
                           w.OriginalPrice.HasValue && 
                           w.Book!.Price < w.OriginalPrice.Value)
                .ToListAsync();
        }
    }
}