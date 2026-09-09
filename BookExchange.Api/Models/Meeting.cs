using System;

namespace BookExchange.Api.Models
{
    public class Meeting
    {
        public int MeetingId { get; set; }
        public int ListingId { get; set; }
        public int BuyerId { get; set; }
        public DateTime ProposedTime { get; set; }
        public string? Location { get; set; }
        public byte Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Listing Listing { get; set; } = null!;
        public User Buyer { get; set; } = null!;
        public Review? Review { get; set; }
    }
}
