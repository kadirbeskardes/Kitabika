namespace BookStore.Web.Helpers
{
    public class CartItemDto
    {
        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
    }
    public class Cart
    {
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public string CouponCode { get; set; }
        public decimal CouponDiscount { get; set; }

        public decimal TotalPrice => Items.Sum(i => i.TotalPrice) - CouponDiscount;
    }

}
