using BookStore.Core.Entities;

namespace BookStore.Core.Interfaces
{
    public interface IFavoriteRepository : IRepository<Favorite>
    {
        Task<IEnumerable<Favorite>> GetUserFavoritesAsync(int userId);
        Task<bool> IsBookInFavoritesAsync(int userId, int bookId);
        Task RemoveFromFavoritesAsync(int userId, int bookId);
        Task<IEnumerable<int>> GetMostFavoritedBookIdsAsync(int count = 10);
    }
}