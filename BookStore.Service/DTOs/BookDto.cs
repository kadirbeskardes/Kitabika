namespace BookStore.Service.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int PublicationYear { get; set; }
    }
}
