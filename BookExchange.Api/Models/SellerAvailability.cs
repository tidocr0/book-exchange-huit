using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookExchange.Api.Models
{
    [Table("SellerAvailability")]
    public class SellerAvailability
    {
        [Key]
        public int AvailabilityId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public byte DayOfWeek { get; set; } // 0 = Sunday, 1 = Monday, ..., 6 = Saturday

        [Required]
        public byte TimeSlot { get; set; } // 0 = Sáng, 1 = Trưa, 2 = Chiều, 3 = Tối

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
