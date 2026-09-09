using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace BookExchange.Api.DTOs
{
    public class ListingCreateDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public byte Condition { get; set; }
        public int SubjectId { get; set; }
        
        public IFormFileCollection? Images { get; set; }
    }

    public class ListingUpdateDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public byte Condition { get; set; }
        public int SubjectId { get; set; }
    }

    public class UpdateListingStatusDto
    {
        public byte Status { get; set; }
    }

    public class SellerInfoDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Phone { get; set; }
        public bool IsVerified { get; set; }
    }

    public class ListingImageDto
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public byte SortOrder { get; set; }
    }

    public class ListingResponseDto
    {
        public int ListingId { get; set; }
        public int SellerId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public byte Condition { get; set; }
        public int SubjectId { get; set; }
        public byte Status { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public SellerInfoDto Seller { get; set; } = null!;
        public List<ListingImageDto> Images { get; set; } = new List<ListingImageDto>();
    }
}
