namespace BookStore.Service.DTOs
{
    public class WishlistDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public BookDto? Book { get; set; }
        public DateTime AddedDate { get; set; }
        public bool IsNotificationEnabled { get; set; }
        public decimal? OriginalPrice { get; set; }
        public bool HasPriceDropped { get; set; }
        public decimal? PriceDifference { get; set; }
    }

    public class AddToWishlistDto
    {
        public int BookId { get; set; }
        public bool IsNotificationEnabled { get; set; } = true;
    }

    public class FavoriteDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public BookDto? Book { get; set; }
        public DateTime AddedDate { get; set; }
    }

    public class AddToFavoritesDto
    {
        public int BookId { get; set; }
    }

    public class WishlistStatsDto
    {
        public int TotalItems { get; set; }
        public int BooksWithPriceDrop { get; set; }
        public decimal TotalSavings { get; set; }
        public DateTime? LastAddedDate { get; set; }
    }
}