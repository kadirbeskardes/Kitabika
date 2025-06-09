using BookStore.Service.DTOs;
using BookStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CouponsController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponsController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<IActionResult> Index()
        {
            var coupons = await _couponService.GetAllCouponsAsync();
            return View(coupons);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCouponDto createCouponDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _couponService.CreateCouponAsync(createCouponDto);
                    TempData["SuccessMessage"] = "Kupon başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(createCouponDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(new CreateCouponDto
            {
                Code = coupon.Code,
                Description = coupon.Description,
                DiscountAmount = coupon.DiscountAmount ?? 0m,  
                DiscountPercentage = coupon.DiscountPercentage,
                ValidFrom = coupon.ValidFrom,
                ValidTo = coupon.ValidTo,
                UsageLimit = coupon.UsageLimit,
                IsActive = coupon.IsActive
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateCouponDto updateCouponDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _couponService.UpdateCouponAsync(id, updateCouponDto);
                    TempData["SuccessMessage"] = "Kupon başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(updateCouponDto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _couponService.DeleteCouponAsync(id);
            TempData["SuccessMessage"] = "Kupon başarıyla silindi";
            return RedirectToAction(nameof(Index));
        }
    }
}