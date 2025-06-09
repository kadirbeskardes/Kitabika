using System.ComponentModel.DataAnnotations;

namespace BookStore.Service.DTOs
{
    public class ApplyCouponDto
    {
        [Required]
        public string Code { get; set; }
    }
}
