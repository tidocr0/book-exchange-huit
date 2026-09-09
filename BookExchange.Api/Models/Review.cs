using System;

namespace BookExchange.Api.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int MeetingId { get; set; }
        public int ReviewerId { get; set; }
        public int RevieweeId { get; set; }
        public byte Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Meeting Meeting { get; set; } = null!;
        public User Reviewer { get; set; } = null!;
        public User Reviewee { get; set; } = null!;
    }
}
