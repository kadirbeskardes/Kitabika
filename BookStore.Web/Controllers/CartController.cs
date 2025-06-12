using System.Security.Claims;
using BookStore.Service.DTOs;
using BookStore.Service.Interfaces;
using BookStore.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IOrderService _orderService;
        private readonly ICouponService _couponService;

        public CartController(IBookService bookService, IOrderService orderService, ICouponService couponService)
        {
            _bookService = bookService;
            _orderService = orderService;
            _couponService = couponService;
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int bookId, int quantity = 1)
        {
            if (quantity <= 0)
            {
                return BadRequest("Adet 0'dan büyük olmalýdýr.");
            }

            var book = await _bookService.GetBookByIdAsync(bookId);
            if (book == null)
            {
                return NotFound();
            }

            if (book.Stock < quantity)
            {
                TempData["ErrorMessage"] = $"Stokta yalnýzca {book.Stock} adet mevcut.";
                return RedirectToAction("Details", "Home", new { id = bookId });
            }

            var cart = GetCart();
            var existingItem = cart.Items.FirstOrDefault(item => item.BookId == bookId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItemDto
                {
                    BookId = bookId,
                    BookTitle = book.Title,
                    UnitPrice = book.Price,
                    Quantity = quantity
                });
            }

            SaveCart(cart);
            TempData["SuccessMessage"] = $"{book.Title} sepete eklendi.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int bookId)
        {
            var cart = GetCart();
            var itemToRemove = cart.Items.FirstOrDefault(item => item.BookId == bookId);

            if (itemToRemove != null)
            {
                cart.Items.Remove(itemToRemove);
                SaveCart(cart);
                TempData["SuccessMessage"] = "Ürün sepetten kaldýrýldý.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateCart(int bookId, int quantity)
        {
            if (quantity <= 0)
            {
                return RemoveFromCart(bookId);
            }

            var cart = GetCart();
            var itemToUpdate = cart.Items.FirstOrDefault(item => item.BookId == bookId);

            if (itemToUpdate != null)
            {
                itemToUpdate.Quantity = quantity;
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(ApplyCouponDto applyCouponDto)
        {
            var result = await _couponService.ValidateCouponAsync(applyCouponDto.Code);

            if (result.IsValid)
            {
                var cart = GetCart();
                cart.CouponCode = applyCouponDto.Code;
                cart.CouponDiscount = result.Coupon.DiscountPercentage.HasValue
    ? cart.Items.Sum(i => i.TotalPrice) * result.Coupon.DiscountPercentage.Value / 100
    : result.Coupon.DiscountAmount ?? 0m;


                SaveCart(cart);
                TempData["SuccessMessage"] = "Kupon baþarýyla uygulandý";
            }
            else
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveCoupon()
        {
            var cart = GetCart();
            cart.CouponCode = null;
            cart.CouponDiscount = 0;
            SaveCart(cart);

            TempData["SuccessMessage"] = "Kupon baþarýyla kaldýrýldý";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Sepetiniz boþ.";
                return RedirectToAction("Index");
            }

            return View(new CreateOrderDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CreateOrderDto createOrderDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var cart = GetCart();

            if (!cart.Items.Any())
            {
                ModelState.AddModelError("", "Sepet boþ");
                return View(createOrderDto);
            }

            createOrderDto.CouponCode = cart.CouponCode?.Trim();
            createOrderDto.UserId = userId;
            createOrderDto.OrderItems = cart.Items.Select(item => new CreateOrderItemDto
            {
                BookId = item.BookId,
                Quantity = item.Quantity
            }).ToList();

            try
            {
                var order = await _orderService.CreateOrderAsync(createOrderDto);
                ClearCart();
                HttpContext.Response.Cookies.Delete("AppliedCoupon");
                return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Sipariþiniz iþlenirken hata oluþtu: {ex.Message}");
                return View(createOrderDto);
            }
        }

        public IActionResult OrderConfirmation(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }

        public async Task<IActionResult> OrderHistory()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null || order.UserId != userId)
            {
                return NotFound();
            }

            return View(order);
        }

        private Cart GetCart()
        {
            return HttpContext.Session.GetObject<Cart>("Cart") ?? new Cart();
        }

        private void SaveCart(Cart cart)
        {
            HttpContext.Session.SetObject("Cart", cart);
        }

        private void ClearCart()
        {
            HttpContext.Session.Remove("Cart");
        }
    }
}