namespace BookStore.Web.Helpers
{
    public class CartItemDto
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
    }


}
