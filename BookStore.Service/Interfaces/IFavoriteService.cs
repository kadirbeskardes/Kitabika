using BookStore.Service.DTOs;

namespace BookStore.Service.Interfaces
{
    public interface IFavoriteService
    {
        Task<IEnumerable<FavoriteDto>> GetUserFavoritesAsync(int userId);
        Task<bool> AddToFavoritesAsync(int userId, AddToFavoritesDto addToFavoritesDto);
        Task<bool> RemoveFromFavoritesAsync(int userId, int bookId);
        Task<bool> IsBookInFavoritesAsync(int userId, int bookId);
        Task<IEnumerable<BookDto>> GetMostFavoritedBooksAsync(int count = 10);
        Task<int> GetFavoritesCountAsync(int userId);
    }
}