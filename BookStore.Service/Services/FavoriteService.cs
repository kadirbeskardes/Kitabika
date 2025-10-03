using AutoMapper;
using BookStore.Core.Entities;
using BookStore.Core.Interfaces;
using BookStore.Service.DTOs;
using BookStore.Service.Interfaces;

namespace BookStore.Service.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FavoriteService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FavoriteDto>> GetUserFavoritesAsync(int userId)
        {
            var favorites = await _unitOfWork.Favorites.GetUserFavoritesAsync(userId);
            return _mapper.Map<IEnumerable<FavoriteDto>>(favorites);
        }

        public async Task<bool> AddToFavoritesAsync(int userId, AddToFavoritesDto addToFavoritesDto)
        {
            try
            {
                // Zaten favorilerde var mı kontrol et
                var exists = await _unitOfWork.Favorites.IsBookInFavoritesAsync(userId, addToFavoritesDto.BookId);
                if (exists)
                {
                    return false; // Zaten favorilerde var
                }

                // Kitabın var olup olmadığını kontrol et
                var book = await _unitOfWork.Books.GetByIdAsync(addToFavoritesDto.BookId);
                if (book == null)
                {
                    return false; // Kitap bulunamadı
                }

                var favoriteItem = new Favorite
                {
                    UserId = userId,
                    BookId = addToFavoritesDto.BookId,
                    CreatedDate = DateTime.Now
                };

                await _unitOfWork.Favorites.AddAsync(favoriteItem);
                await _unitOfWork.CommitAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveFromFavoritesAsync(int userId, int bookId)
        {
            try
            {
                await _unitOfWork.Favorites.RemoveFromFavoritesAsync(userId, bookId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsBookInFavoritesAsync(int userId, int bookId)
        {
            return await _unitOfWork.Favorites.IsBookInFavoritesAsync(userId, bookId);
        }

        public async Task<IEnumerable<BookDto>> GetMostFavoritedBooksAsync(int count = 10)
        {
            var bookIds = await _unitOfWork.Favorites.GetMostFavoritedBookIdsAsync(count);
            var books = new List<BookDto>();

            foreach (var bookId in bookIds)
            {
                var book = await _unitOfWork.Books.GetByIdAsync(bookId);
                if (book != null)
                {
                    books.Add(_mapper.Map<BookDto>(book));
                }
            }

            return books;
        }

        public async Task<int> GetFavoritesCountAsync(int userId)
        {
            var favorites = await _unitOfWork.Favorites.GetUserFavoritesAsync(userId);
            return favorites.Count();
        }
    }
}