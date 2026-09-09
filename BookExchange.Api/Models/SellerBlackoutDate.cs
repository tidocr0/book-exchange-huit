using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookExchange.Api.Models
{
    [Table("SellerBlackoutDates")]
    public class SellerBlackoutDate
    {
        [Key]
        public int BlackoutId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime BlackoutDate { get; set; }

        public byte? TimeSlot { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
