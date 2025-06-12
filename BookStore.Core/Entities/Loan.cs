namespace BookStore.Core.Entities
{
    public class Loan : BaseEntity
    {
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime LoanDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; } = "Active"; // Active, Returned, Overdue
        public decimal? FineAmount { get; set; }
    }
}
