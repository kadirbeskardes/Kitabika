using System.Security.Claims;
using BookStore.Service.DTOs;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IBookService _bookService;
        private readonly IUserService _userService;

        public ReviewsController(
            IReviewService reviewService,
            IBookService bookService,
            IUserService userService)
        {
            _reviewService = reviewService;
            _bookService = bookService;
            _userService = userService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> BookReviews(int bookId)
        {
            var book = await _bookService.GetBookByIdAsync(bookId);
            if (book == null)
                return NotFound();

            ViewBag.BookTitle = book.Title;
            var reviews = await _reviewService.GetReviewsByBookAsync(bookId);
            return View(reviews);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int bookId)
        {
            var book = await _bookService.GetBookByIdAsync(bookId);
            if (book == null)
                return NotFound();

            ViewBag.BookTitle = book.Title;
            return View(new CreateReviewDto { BookId = bookId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReviewDto createReviewDto)
        {
            if (!ModelState.IsValid)
            {
                var book = await _bookService.GetBookByIdAsync(createReviewDto.BookId);
                ViewBag.BookTitle = book?.Title;
                return View(createReviewDto);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await _reviewService.AddReviewAsync(createReviewDto, userId);

            TempData["SuccessMessage"] = "İnceleme başarıyla eklendi.";
            return RedirectToAction("Details", "Home", new { id = createReviewDto.BookId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var review = await _reviewService.GetReviewByIdAsync(id);

            if (review == null || review.UserId != userId)
                return NotFound();

            return View(new UpdateReviewDto
            {
                Title = review.Title,
                Content = review.Content,
                Rating = review.Rating
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateReviewDto updateReviewDto)
        {
            if (!ModelState.IsValid)
                return View(updateReviewDto);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await _reviewService.UpdateReviewAsync(id, updateReviewDto, userId);

            TempData["SuccessMessage"] = "İnceleme başarıyla güncellendi.";
            return RedirectToAction("MyReviews");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var review = await _reviewService.GetReviewByIdAsync(id);

            if (review == null || review.UserId != userId)
                return NotFound();

            await _reviewService.DeleteReviewAsync(id, userId);

            TempData["SuccessMessage"] = "İnceleme başarıyla silindi.";
            return RedirectToAction("MyReviews");
        }

        public async Task<IActionResult> MyReviews()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var reviews = await _reviewService.GetReviewsByUserAsync(userId);
            return View(reviews);
        }
    }
}