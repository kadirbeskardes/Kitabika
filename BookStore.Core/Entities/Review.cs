namespace BookStore.Core.Entities
{
    public class Review : BaseEntity
    {
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; } 
        public DateTime ReviewDate { get; set; } = DateTime.Now;
    }
}
