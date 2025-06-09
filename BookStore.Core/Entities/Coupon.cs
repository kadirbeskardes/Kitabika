namespace BookStore.Core.Entities
{
    public class Coupon : BaseEntity
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal DiscountAmount { get; set; } 
        public decimal? DiscountPercentage { get; set; } 
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public ICollection<Order> Orders { get; set; }
    }
}
