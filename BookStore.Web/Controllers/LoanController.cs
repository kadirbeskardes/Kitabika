using BookStore.Service.DTOs;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStore.Web.Controllers
{
    [Authorize]
    public class LoanController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly IBookService _bookService;

        public LoanController(ILoanService loanService, IBookService bookService)
        {
            _loanService = loanService;
            _bookService = bookService;
        }

        public async Task<IActionResult> MyLoans()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var loans = await _loanService.GetUserLoansAsync(userId);
            return View(loans);
        }

        [HttpPost]
        public async Task<IActionResult> Borrow(int bookId, int loanDays = 14)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var loan = await _loanService.CreateLoanAsync(new CreateLoanDto
                {
                    BookId = bookId,
                    UserId = userId,
                    LoanDays = loanDays
                });

                TempData["SuccessMessage"] = "Kitap başarıyla ödünç alındı";
                return RedirectToAction("MyLoans");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Details", "Home", new { id = bookId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Return(int id)
        {
            var loan = await _loanService.GetLoanByIdAsync(id);
            if (loan == null || loan.UserId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return NotFound();

            return View(new ReturnLoanDto());
        }

        [HttpPost]
        public async Task<IActionResult> Return(int id, ReturnLoanDto returnLoanDto)
        {
            try
            {
                var loan = await _loanService.ReturnLoanAsync(id, returnLoanDto);
                TempData["SuccessMessage"] = $"Kitap başarıyla iade edildi{(loan.FineAmount > 0 ? $". Ceza: {loan.FineAmount:C}" : "")}";
                return RedirectToAction("MyLoans");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(returnLoanDto);
            }
        }
    }
}
