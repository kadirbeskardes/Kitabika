namespace BookStore.Service.DTOs
{
    public class LoanDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string BookCoverImage { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; }
        public decimal? FineAmount { get; set; }
        public bool IsOverdue => Status == "Active" && DueDate < DateTime.Now;
    }
}
