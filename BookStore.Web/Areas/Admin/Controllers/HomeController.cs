using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;

        public HomeController(
            IBookService bookService,
            ICategoryService categoryService,
            IUserService userService,
            IOrderService orderService)
        {
            _bookService = bookService;
            _categoryService = categoryService;
            _userService = userService;
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var bookCount = (await _bookService.GetAllBooksAsync()).Count();
            var categoryCount = (await _categoryService.GetAllCategoriesAsync()).Count();
            var userCount = (await _userService.GetAllUsersAsync()).Count();
            var orderCount = (await _orderService.GetAllOrdersAsync()).Count();

            ViewBag.BookCount = bookCount;
            ViewBag.CategoryCount = categoryCount;
            ViewBag.UserCount = userCount;
            ViewBag.OrderCount = orderCount;

            return View();
        }
    }
}
