using BookStore.Service.DTOs;

namespace BookStore.Service.Interfaces
{
    public interface IWishlistService
    {
        Task<IEnumerable<WishlistDto>> GetUserWishlistAsync(int userId);
        Task<WishlistStatsDto> GetWishlistStatsAsync(int userId);
        Task<bool> AddToWishlistAsync(int userId, AddToWishlistDto addToWishlistDto);
        Task<bool> RemoveFromWishlistAsync(int userId, int bookId);
        Task<bool> IsBookInWishlistAsync(int userId, int bookId);
        Task<bool> UpdateNotificationSettingAsync(int userId, int bookId, bool enableNotification);
        Task<IEnumerable<WishlistDto>> GetBooksWithPriceDropAsync();
    }
}