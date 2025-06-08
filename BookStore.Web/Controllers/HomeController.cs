using BookStore.Service.DTOs;
using BookStore.Service.Interfaces;
using BookStore.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;

        public HomeController(IBookService bookService, ICategoryService categoryService)
        {
            _bookService = bookService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllBooksAsync();
            var categories = await _categoryService.GetAllCategoriesAsync();

            ViewBag.Categories = categories;
            return View(books);
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return RedirectToAction("Index");

            var books = await _bookService.SearchBooksAsync(searchTerm);
            var categories = await _categoryService.GetAllCategoriesAsync(); 

            ViewBag.Categories = categories; 
            return View("Index", books);
        }

        public async Task<IActionResult> Category(int id)
        {
            var books = await _bookService.GetBooksByCategoryAsync(id);
            var category = await _categoryService.GetCategoryByIdAsync(id);
            var categories = await _categoryService.GetAllCategoriesAsync(); 

            ViewBag.CategoryName = category?.Name;
            ViewBag.Categories = categories; 
            return View("Index", books);
        }

        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
                return NotFound();

            return View(book);
        }

        
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}