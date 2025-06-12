using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Areas.Admin.ViewComponents
{
    public class BookLoanStatusViewComponent : ViewComponent
    {
        private readonly ILoanService _loanService;

        public BookLoanStatusViewComponent(ILoanService loanService)
        {
            _loanService = loanService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int bookId)
        {
            var isAvailable = await _loanService.IsBookAvailableAsync(bookId);
            return View(isAvailable);
        }
    }
}
