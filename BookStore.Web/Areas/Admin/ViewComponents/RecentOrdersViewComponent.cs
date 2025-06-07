using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Areas.Admin.ViewComponents
{
    public class RecentOrdersViewComponent : ViewComponent
    {
        private readonly IOrderService _orderService;

        public RecentOrdersViewComponent(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var recentOrders = orders.OrderByDescending(o => o.OrderDate).Take(5);
            return View(recentOrders);
        }
    }
}
