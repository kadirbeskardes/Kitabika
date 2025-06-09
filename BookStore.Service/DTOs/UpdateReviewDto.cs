using System.ComponentModel.DataAnnotations;

namespace BookStore.Service.DTOs
{
    public class UpdateReviewDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(2000)]
        public string Content { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
