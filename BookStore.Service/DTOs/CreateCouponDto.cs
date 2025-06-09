// Se
using System;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Service.DTOs
{
    public class CreateCouponDto
    {
        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [Range(0, 1000)]
        public decimal DiscountAmount { get; set; } = 0;

        [Range(0, 100)]
        public decimal? DiscountPercentage { get; set; }

        [Required]
        public DateTime ValidFrom { get; set; } = DateTime.Now;

        [Required]
        public DateTime ValidTo { get; set; } = DateTime.Now.AddMonths(1);

        [Range(1, int.MaxValue)]
        public int? UsageLimit { get; set; }

        public bool IsActive { get; set; } = true;
    }
}