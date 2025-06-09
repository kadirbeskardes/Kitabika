using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.ViewComponents
{
    public class BookReviewsViewComponent : ViewComponent
    {
        private readonly IReviewService _reviewService;

        public BookReviewsViewComponent(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int bookId)
        {
            var reviews = await _reviewService.GetReviewsByBookAsync(bookId);
            return View(reviews);
        }
    }
}