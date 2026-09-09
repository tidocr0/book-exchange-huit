using System;

namespace BookExchange.Api.Models
{
    public class Report
    {
        public int ReportId { get; set; }
        public int ReporterId { get; set; }
        public int? ListingId { get; set; }
        public int? ReportedUserId { get; set; }
        public string Reason { get; set; } = null!;
        public byte Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User Reporter { get; set; } = null!;
        public Listing? Listing { get; set; }
        public User? ReportedUser { get; set; }
    }
}
