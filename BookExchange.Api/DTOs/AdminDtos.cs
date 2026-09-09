using System;

namespace BookExchange.Api.DTOs
{
    public class AdminStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalListings { get; set; }
        public int TotalListingsSold { get; set; }
        public int TotalMeetingsCompleted { get; set; }
        public int PendingVerifications { get; set; }
        public int PendingReports { get; set; }
    }

    public class PendingVerificationDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? StudentIdFrontUrl { get; set; }
        public string? StudentIdBackUrl { get; set; }
    }

    public class ReportDetailDto
    {
        public int ReportId { get; set; }
        public string ReporterFullName { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public string? ListingTitle { get; set; }
        public string? ReportedUserFullName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ResolveReportRequestDto
    {
        public int Action { get; set; }
    }
}
