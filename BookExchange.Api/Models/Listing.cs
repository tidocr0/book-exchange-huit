using System;
using System.Collections.Generic;

namespace BookExchange.Api.Models
{
    public class Listing
    {
        public int ListingId { get; set; }
        public int SellerId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public byte Condition { get; set; }
        public int SubjectId { get; set; }
        public byte Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User Seller { get; set; } = null!;
        public Subject Subject { get; set; } = null!;
        public ICollection<ListingImage> Images { get; set; } = new List<ListingImage>();
        public ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
