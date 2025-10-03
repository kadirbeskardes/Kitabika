using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public HomeController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardStats = await _dashboardService.GetDashboardStatsAsync();
            return View(dashboardStats);
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardData(DateTime? startDate, DateTime? endDate)
        {
            var dashboardStats = await _dashboardService.GetDashboardStatsAsync(startDate, endDate);
            return Json(dashboardStats);
        }
    }
}
