using BookStore.Core.Interfaces;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookStore.Web.ViewComponents
{
    public class BookAvailabilityViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookAvailabilityViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync(int bookId)
        {
            var isAvailable = await _unitOfWork.Loans.IsBookAvailableAsync(bookId);
            ViewBag.BookId=bookId;
            return View(isAvailable);
        }
    }
}