using BookStore.Service.DTOs;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Web.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly IWishlistService _wishlistService;
        private readonly IBookService _bookService;

        public WishlistController(IWishlistService wishlistService, IBookService bookService)
        {
            _wishlistService = wishlistService;
            _bookService = bookService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var wishlist = await _wishlistService.GetUserWishlistAsync(userId);
            var stats = await _wishlistService.GetWishlistStatsAsync(userId);

            ViewBag.Stats = stats;
            return View(wishlist);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int bookId, bool enableNotification = true)
        {
            var userId = GetCurrentUserId();
            var addToWishlistDto = new AddToWishlistDto
            {
                BookId = bookId,
                IsNotificationEnabled = enableNotification
            };

            var result = await _wishlistService.AddToWishlistAsync(userId, addToWishlistDto);

            if (result)
            {
                TempData["Success"] = "Kitap istek listenize eklendi!";
            }
            else
            {
                TempData["Error"] = "Kitap zaten istek listenizde veya bir hata oluştu.";
            }

            return RedirectToAction("Details", "Home", new { id = bookId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist(int bookId)
        {
            var userId = GetCurrentUserId();
            var result = await _wishlistService.RemoveFromWishlistAsync(userId, bookId);

            if (result)
            {
                TempData["Success"] = "Kitap istek listenizden kaldırıldı!";
            }
            else
            {
                TempData["Error"] = "Bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateNotification(int bookId, bool enableNotification)
        {
            var userId = GetCurrentUserId();
            var result = await _wishlistService.UpdateNotificationSettingAsync(userId, bookId, enableNotification);

            return Json(new { success = result });
        }

        [HttpGet]
        public async Task<IActionResult> CheckWishlistStatus(int bookId)
        {
            var userId = GetCurrentUserId();
            var isInWishlist = await _wishlistService.IsBookInWishlistAsync(userId, bookId);

            return Json(new { isInWishlist });
        }

        [HttpPost]
        public async Task<IActionResult> ToggleWishlist(int bookId)
        {
            var userId = GetCurrentUserId();
            var isInWishlist = await _wishlistService.IsBookInWishlistAsync(userId, bookId);

            bool result;
            string message;

            if (isInWishlist)
            {
                result = await _wishlistService.RemoveFromWishlistAsync(userId, bookId);
                message = result ? "Kitap istek listenizden kaldırıldı!" : "Bir hata oluştu.";
            }
            else
            {
                var addToWishlistDto = new AddToWishlistDto { BookId = bookId, IsNotificationEnabled = true };
                result = await _wishlistService.AddToWishlistAsync(userId, addToWishlistDto);
                message = result ? "Kitap istek listenize eklendi!" : "Bir hata oluştu.";
            }

            return Json(new 
            { 
                success = result, 
                message = message, 
                isInWishlist = !isInWishlist && result 
            });
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}