namespace BookStore.Service.DTOs
{
    public class BookSearchDto
    {
        public string? SearchTerm { get; set; }
        public string? Author { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public int? CategoryId { get; set; }
        public bool? InStock { get; set; }
        public string? SortBy { get; set; } = "title"; // title, author, price, year
        public bool SortDescending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}