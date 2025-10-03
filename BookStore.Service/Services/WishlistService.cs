using AutoMapper;
using BookStore.Core.Entities;
using BookStore.Core.Interfaces;
using BookStore.Service.DTOs;
using BookStore.Service.Interfaces;

namespace BookStore.Service.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WishlistService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WishlistDto>> GetUserWishlistAsync(int userId)
        {
            var wishlistItems = await _unitOfWork.Wishlists.GetUserWishlistAsync(userId);
            var wishlistDtos = new List<WishlistDto>();

            foreach (var item in wishlistItems)
            {
                var dto = _mapper.Map<WishlistDto>(item);
                
                // Fiyat düşüşü kontrolü
                if (item.OriginalPrice.HasValue && item.Book != null)
                {
                    dto.HasPriceDropped = item.Book.Price < item.OriginalPrice.Value;
                    dto.PriceDifference = item.OriginalPrice.Value - item.Book.Price;
                }

                wishlistDtos.Add(dto);
            }

            return wishlistDtos;
        }

        public async Task<WishlistStatsDto> GetWishlistStatsAsync(int userId)
        {
            var wishlistItems = await _unitOfWork.Wishlists.GetUserWishlistAsync(userId);
            var itemsList = wishlistItems.ToList();

            var stats = new WishlistStatsDto
            {
                TotalItems = itemsList.Count,
                LastAddedDate = itemsList.OrderByDescending(w => w.AddedDate).FirstOrDefault()?.AddedDate
            };

            // Fiyat düşen kitapları say ve toplam tasarrufu hesapla
            foreach (var item in itemsList)
            {
                if (item.OriginalPrice.HasValue && item.Book != null && item.Book.Price < item.OriginalPrice.Value)
                {
                    stats.BooksWithPriceDrop++;
                    stats.TotalSavings += item.OriginalPrice.Value - item.Book.Price;
                }
            }

            return stats;
        }

        public async Task<bool> AddToWishlistAsync(int userId, AddToWishlistDto addToWishlistDto)
        {
            try
            {
                // Zaten listede var mı kontrol et
                var exists = await _unitOfWork.Wishlists.IsBookInWishlistAsync(userId, addToWishlistDto.BookId);
                if (exists)
                {
                    return false; // Zaten listede var
                }

                // Kitabın mevcut fiyatını al
                var book = await _unitOfWork.Books.GetByIdAsync(addToWishlistDto.BookId);
                if (book == null)
                {
                    return false; // Kitap bulunamadı
                }

                var wishlistItem = new Wishlist
                {
                    UserId = userId,
                    BookId = addToWishlistDto.BookId,
                    IsNotificationEnabled = addToWishlistDto.IsNotificationEnabled,
                    OriginalPrice = book.Price,
                    CreatedDate = DateTime.Now
                };

                await _unitOfWork.Wishlists.AddAsync(wishlistItem);
                await _unitOfWork.CommitAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveFromWishlistAsync(int userId, int bookId)
        {
            try
            {
                await _unitOfWork.Wishlists.RemoveFromWishlistAsync(userId, bookId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsBookInWishlistAsync(int userId, int bookId)
        {
            return await _unitOfWork.Wishlists.IsBookInWishlistAsync(userId, bookId);
        }

        public async Task<bool> UpdateNotificationSettingAsync(int userId, int bookId, bool enableNotification)
        {
            try
            {
                var wishlistItems = await _unitOfWork.Wishlists.GetUserWishlistAsync(userId);
                var item = wishlistItems.FirstOrDefault(w => w.BookId == bookId);
                
                if (item != null)
                {
                    item.IsNotificationEnabled = enableNotification;
                    // UpdateAsync Repository interface'inde yok, direkt CommitAsync kullanıyoruz
                    await _unitOfWork.CommitAsync();
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<WishlistDto>> GetBooksWithPriceDropAsync()
        {
            var wishlistItems = await _unitOfWork.Wishlists.GetBooksWithPriceDropAsync();
            return _mapper.Map<IEnumerable<WishlistDto>>(wishlistItems);
        }
    }
}