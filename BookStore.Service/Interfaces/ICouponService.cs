using BookStore.Service.DTOs;

namespace BookStore.Service.Interfaces
{
    public interface ICouponService
    {
        Task<IEnumerable<CouponDto>> GetAllCouponsAsync();
        Task<CouponDto> GetCouponByIdAsync(int id);
        Task<CouponDto> CreateCouponAsync(CreateCouponDto createCouponDto);
        Task UpdateCouponAsync(int id, CreateCouponDto updateCouponDto);
        Task DeleteCouponAsync(int id);
        Task<CouponValidationResult> ValidateCouponAsync(string code);
    }

    public class CouponValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public CouponDto Coupon { get; set; }
    }
}
