using BookStore.Service.DTOs;
using BookStore.Service.Interfaces;
using BookStore.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LoansController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly IBookService _bookService;
        private readonly IUserService _userService;

        public LoansController(ILoanService loanService, IUserService userService, IBookService bookService)
        {
            _loanService = loanService;
            _bookService = bookService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var loans = await _loanService.GetAllLoansAsync();
            return View(loans.OrderByDescending(l => l.LoanDate).ToList());
        }

        public async Task<IActionResult> Overdue()
        {
            var loans = await _loanService.GetOverdueLoansAsync();
            return View(loans);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Books = await _bookService.GetAllBooksAsync();
            ViewBag.Users = await _userService.GetAllUsersAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateLoanDto createLoanDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _loanService.CreateLoanAsync(createLoanDto);
                    TempData["SuccessMessage"] = "Ödünç başarıyla oluşturuldu";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            ViewBag.Books = await _bookService.GetAllBooksAsync();
            ViewBag.Users = await _userService.GetAllUsersAsync();
            return View(createLoanDto);
        }

        [HttpGet]
        public async Task<IActionResult> Return(int id)
        {
            var loan = await _loanService.GetLoanByIdAsync(id);
            if (loan == null)
            {
                return NotFound();
            }

            ViewBag.LoanInfo = $"{loan.BookTitle} (Ödünç Alan: {loan.UserName})";
            return View(new ReturnLoanDto());
        }

        [HttpPost]
        public async Task<IActionResult> Return(int id, ReturnLoanDto returnLoanDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var loan = await _loanService.ReturnLoanAsync(id, returnLoanDto);
                    TempData["SuccessMessage"] = $"Kitap başarıyla iade edildi{(loan.FineAmount > 0 ? $". Ceza: {loan.FineAmount:C}" : "")}";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var loanInfo = await _loanService.GetLoanByIdAsync(id);
            ViewBag.LoanInfo = $"{loanInfo.BookTitle} (Ödünç Alan: {loanInfo.UserName})";
            return View(returnLoanDto);
        }

        [HttpGet]
        public async Task<IActionResult> CheckBookAvailability(int bookId)
        {
            var isAvailable = await _loanService.IsBookAvailableAsync(bookId);
            return Json(new { isAvailable });
        }
    }
}
