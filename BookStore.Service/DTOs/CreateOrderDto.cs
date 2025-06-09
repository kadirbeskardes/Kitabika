namespace BookStore.Service.DTOs
{
    public class CreateOrderDto
    {
        public int UserId { get; set; }
        public List<CreateOrderItemDto> OrderItems { get; set; }
        public string CouponCode { get; set; }
    }
}
