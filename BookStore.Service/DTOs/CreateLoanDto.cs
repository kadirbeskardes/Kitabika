using System.ComponentModel.DataAnnotations;

namespace BookStore.Service.DTOs
{
    public class CreateLoanDto
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(1, 30)]
        public int LoanDays { get; set; } = 14;
    }
}
