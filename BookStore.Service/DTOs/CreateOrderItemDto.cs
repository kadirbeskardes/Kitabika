namespace BookStore.Service.DTOs
{
    public class CreateOrderItemDto
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
    }
}
