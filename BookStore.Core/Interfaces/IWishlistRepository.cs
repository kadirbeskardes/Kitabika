using BookStore.Core.Entities;

namespace BookStore.Core.Interfaces
{
    public interface IWishlistRepository : IRepository<Wishlist>
    {
        Task<IEnumerable<Wishlist>> GetUserWishlistAsync(int userId);
        Task<bool> IsBookInWishlistAsync(int userId, int bookId);
        Task RemoveFromWishlistAsync(int userId, int bookId);
        Task<IEnumerable<Wishlist>> GetBooksWithPriceDropAsync(); // Fiyat düşen kitaplar için
    }
}