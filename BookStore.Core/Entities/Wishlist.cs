namespace BookStore.Core.Entities
{
    public class Wishlist : BaseEntity
    {
        public int UserId { get; set; }
        public User? User { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;
        public bool IsNotificationEnabled { get; set; } = true; // Fiyat düşüşü bildirimi için
        public decimal? OriginalPrice { get; set; } // Favoriye eklendiği andaki fiyat
    }
}