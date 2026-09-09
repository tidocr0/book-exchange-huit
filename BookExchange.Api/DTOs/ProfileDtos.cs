using System;

namespace BookExchange.Api.DTOs
{
    public class ProfileResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsVerified { get; set; }
        public byte VerificationStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProfileUpdateDto
    {
        public string FullName { get; set; } = null!;
        public string? Phone { get; set; }
    }

    public class AvatarUploadResponseDto
    {
        public string AvatarUrl { get; set; } = null!;
    }

    public class VerificationRequestResponseDto
    {
        public byte VerificationStatus { get; set; }
        public string Message { get; set; } = null!;
    }
}
