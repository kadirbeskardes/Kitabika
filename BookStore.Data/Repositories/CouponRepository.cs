using BookStore.Core.Entities;
using BookStore.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Data.Repositories
{
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {
        public CouponRepository(BookStoreContext context) : base(context) { }

        public async Task<Coupon> GetByCodeAsync(string code)
        {
            return await _entities
                .FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<IEnumerable<Coupon>> GetActiveCouponsAsync()
        {
            var now = DateTime.Now;
            return await _entities
                .Where(c => c.IsActive &&
                           c.ValidFrom <= now &&
                           c.ValidTo >= now &&
                           (c.UsageLimit == null || c.UsedCount < c.UsageLimit))
                .ToListAsync();
        }
    }
}
