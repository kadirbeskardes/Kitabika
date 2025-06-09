using BookStore.Core.Enums;

namespace BookStore.Core.Entities
{
    public class Order : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending; 
        public ICollection<OrderItem> OrderItems { get; set; }
        public decimal?  DiscountAmount { get; set; }
        public int? CouponId { get; set; }
    }
}
