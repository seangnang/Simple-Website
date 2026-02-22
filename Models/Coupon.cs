using System.ComponentModel.DataAnnotations;

namespace SimpleWebsite.Models
{
    public class Coupon
    {
        [Key]
        public int CouponId { get; set; }

        [Required]
        public string Code { get; set; } = string.Empty;

        public decimal DiscountPercent { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime ExpiryDate { get; set; }

        public int MaxUses { get; set; } = 100;

        public int UsedCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}