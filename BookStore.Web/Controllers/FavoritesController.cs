using BookStore.Service.DTOs;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Web.Controllers
{
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly IFavoriteService _favoriteService;
        private readonly IBookService _bookService;

        public FavoritesController(IFavoriteService favoriteService, IBookService bookService)
        {
            _favoriteService = favoriteService;
            _bookService = bookService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var favorites = await _favoriteService.GetUserFavoritesAsync(userId);
            return View(favorites);
        }

        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int bookId)
        {
            var userId = GetCurrentUserId();
            var addToFavoritesDto = new AddToFavoritesDto { BookId = bookId };

            var result = await _favoriteService.AddToFavoritesAsync(userId, addToFavoritesDto);

            if (result)
            {
                TempData["Success"] = "Kitap favorilerinize eklendi!";
            }
            else
            {
                TempData["Error"] = "Kitap zaten favorilerinizde veya bir hata oluştu.";
            }

            return RedirectToAction("Details", "Home", new { id = bookId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int bookId)
        {
            var userId = GetCurrentUserId();
            var result = await _favoriteService.RemoveFromFavoritesAsync(userId, bookId);

            if (result)
            {
                TempData["Success"] = "Kitap favorilerinizden kaldırıldı!";
            }
            else
            {
                TempData["Error"] = "Bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> CheckFavoriteStatus(int bookId)
        {
            var userId = GetCurrentUserId();
            var isInFavorites = await _favoriteService.IsBookInFavoritesAsync(userId, bookId);

            return Json(new { isInFavorites });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int bookId)
        {
            var userId = GetCurrentUserId();
            var isInFavorites = await _favoriteService.IsBookInFavoritesAsync(userId, bookId);

            bool result;
            string message;

            if (isInFavorites)
            {
                result = await _favoriteService.RemoveFromFavoritesAsync(userId, bookId);
                message = result ? "Kitap favorilerinizden kaldırıldı!" : "Bir hata oluştu.";
            }
            else
            {
                var addToFavoritesDto = new AddToFavoritesDto { BookId = bookId };
                result = await _favoriteService.AddToFavoritesAsync(userId, addToFavoritesDto);
                message = result ? "Kitap favorilerinize eklendi!" : "Bir hata oluştu.";
            }

            return Json(new 
            { 
                success = result, 
                message = message, 
                isInFavorites = !isInFavorites && result 
            });
        }

        [AllowAnonymous]
        public async Task<IActionResult> MostFavorited()
        {
            var books = await _favoriteService.GetMostFavoritedBooksAsync(20);
            return View(books);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}