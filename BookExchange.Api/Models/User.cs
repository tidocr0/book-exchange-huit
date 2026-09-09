using System;
using System.Collections.Generic;

namespace BookExchange.Api.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }
        public string? StudentIdFrontUrl { get; set; }
        public string? StudentIdBackUrl { get; set; }
        public byte VerificationStatus { get; set; }
        public bool IsVerified { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        public ICollection<Listing> Listings { get; set; } = new List<Listing>();
        public ICollection<Meeting> MeetingsAsBuyer { get; set; } = new List<Meeting>();
        public ICollection<Review> ReviewsGiven { get; set; } = new List<Review>();
        public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();
        public ICollection<Report> ReportsMade { get; set; } = new List<Report>();
        public ICollection<Report> ReportsReceived { get; set; } = new List<Report>();
    }
}
