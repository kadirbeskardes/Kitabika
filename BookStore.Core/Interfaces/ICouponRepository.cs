using BookStore.Core.Entities;

namespace BookStore.Core.Interfaces
{
    public interface ICouponRepository : IRepository<Coupon>
    {
        Task<Coupon> GetByCodeAsync(string code);
        Task<IEnumerable<Coupon>> GetActiveCouponsAsync();
    }
}
