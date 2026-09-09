using System.ComponentModel.DataAnnotations;

namespace BookExchange.Api.Models
{
    public class ListingImage
    {
        [Key]
        public int ImageId { get; set; }
        public int ListingId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public byte SortOrder { get; set; }

        public Listing Listing { get; set; } = null!;
    }
}
