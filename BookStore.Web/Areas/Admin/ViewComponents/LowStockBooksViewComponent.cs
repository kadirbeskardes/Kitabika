using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Areas.Admin.ViewComponents
{
    public class LowStockBooksViewComponent : ViewComponent
    {
        private readonly IBookService _bookService;

        public LowStockBooksViewComponent(IBookService bookService)
        {
            _bookService = bookService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var books = await _bookService.GetAllBooksAsync();
            var lowStockBooks = books.Where(b => b.Stock < 10).OrderBy(b => b.Stock).Take(5);
            return View(lowStockBooks);
        }
    }
}
