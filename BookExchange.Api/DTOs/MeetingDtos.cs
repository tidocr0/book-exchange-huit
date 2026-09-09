using System;
using System.ComponentModel.DataAnnotations;

namespace BookExchange.Api.DTOs
{
    public class CreateMeetingDto
    {
        [Required]
        public int ListingId { get; set; }

        [Required]
        public DateTime ProposedTime { get; set; }

        public string? Location { get; set; }
    }

    public class UpdateMeetingStatusDto
    {
        [Required]
        public byte Status { get; set; } // 1: Accepted, 2: Rejected/Cancelled, 3: Completed
    }
}
