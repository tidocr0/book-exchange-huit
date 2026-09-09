using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BookExchange.Api.Data;
using BookExchange.Api.DTOs;
using BookExchange.Api.Models;

namespace BookExchange.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private const long _maxFileSize = 5 * 1024 * 1024; // 5 MB

        public ListingsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetListings([FromQuery] int? facultyId, [FromQuery] int? subjectId, [FromQuery] byte? condition, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] int? sellerId, [FromQuery] string? search)
        {
            var query = _context.Listings
                .Include(l => l.Images)
                .Include(l => l.Seller)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(l => l.Title.Contains(search) || l.Description.Contains(search));
            }

            // Default to Status = 0 (Đang bán) unless requested by the owner themselves
            bool isOwnerQuerying = false;
            if (sellerId.HasValue && User.Identity?.IsAuthenticated == true)
            {
                var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(currentUserIdStr, out int currentUserId) && currentUserId == sellerId.Value)
                {
                    isOwnerQuerying = true;
                }
            }

            if (!isOwnerQuerying)
            {
                query = query.Where(l => l.Status == 0);
            }

            if (facultyId.HasValue)
            {
                query = query.Where(l => l.Subject.FacultyId == facultyId.Value);
            }
            if (subjectId.HasValue)
            {
                query = query.Where(l => l.SubjectId == subjectId.Value);
            }
            if (condition.HasValue)
            {
                query = query.Where(l => l.Condition == condition.Value);
            }
            if (minPrice.HasValue)
            {
                query = query.Where(l => l.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(l => l.Price <= maxPrice.Value);
            }
            if (sellerId.HasValue)
            {
                query = query.Where(l => l.SellerId == sellerId.Value);
            }

            var listings = await query.OrderByDescending(l => l.CreatedAt).ToListAsync();

            var response = listings.Select(l => new ListingResponseDto
            {
                ListingId = l.ListingId,
                SellerId = l.SellerId,
                Title = l.Title,
                Description = l.Description,
                Price = l.Price,
                Condition = l.Condition,
                SubjectId = l.SubjectId,
                Status = l.Status,
                CreatedAt = l.CreatedAt,
                Seller = new SellerInfoDto
                {
                    UserId = l.Seller.UserId,
                    FullName = l.Seller.FullName,
                    Phone = l.Seller.Phone,
                    IsVerified = l.Seller.IsVerified
                },
                Images = l.Images.OrderBy(i => i.SortOrder).Select(i => new ListingImageDto
                {
                    ImageId = i.ImageId,
                    ImageUrl = i.ImageUrl,
                    SortOrder = i.SortOrder
                }).ToList()
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetListing(int id)
        {
            var listing = await _context.Listings
                .Include(l => l.Images)
                .Include(l => l.Seller)
                .FirstOrDefaultAsync(l => l.ListingId == id);

            if (listing == null)
            {
                return NotFound();
            }

            var response = new ListingResponseDto
            {
                ListingId = listing.ListingId,
                SellerId = listing.SellerId,
                Title = listing.Title,
                Description = listing.Description,
                Price = listing.Price,
                Condition = listing.Condition,
                SubjectId = listing.SubjectId,
                Status = listing.Status,
                CreatedAt = listing.CreatedAt,
                Seller = new SellerInfoDto
                {
                    UserId = listing.Seller.UserId,
                    FullName = listing.Seller.FullName,
                    Phone = listing.Seller.Phone,
                    IsVerified = listing.Seller.IsVerified
                },
                Images = listing.Images.OrderBy(i => i.SortOrder).Select(i => new ListingImageDto
                {
                    ImageId = i.ImageId,
                    ImageUrl = i.ImageUrl,
                    SortOrder = i.SortOrder
                }).ToList()
            };

            return Ok(response);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateListing([FromForm] ListingCreateDto dto)
        {
            if (dto.Images != null && dto.Images.Count > 5)
            {
                return BadRequest(new { message = "Chỉ được phép upload tối đa 5 ảnh cho mỗi tin đăng." });
            }

            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdStr, out int sellerId))
            {
                return Unauthorized();
            }

            var listing = new Listing
            {
                SellerId = sellerId,
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                Condition = dto.Condition,
                SubjectId = dto.SubjectId,
                Status = 0, // Đang bán
                CreatedAt = DateTime.UtcNow
            };

            _context.Listings.Add(listing);
            await _context.SaveChangesAsync(); // Save to generate ListingId

            // Handle image uploads
            if (dto.Images != null && dto.Images.Count > 0)
            {
                var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
                var uploadsFolder = Path.Combine(webRoot, "uploads", "listings");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                byte sortOrder = 0;
                foreach (var file in dto.Images)
                {
                    if (file.Length > _maxFileSize)
                    {
                        return BadRequest(new { message = $"File '{file.FileName}' vượt quá dung lượng tối đa 5MB." });
                    }

                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (string.IsNullOrEmpty(ext) || !_allowedExtensions.Contains(ext))
                    {
                        return BadRequest(new { message = $"File '{file.FileName}' sai định dạng. Chỉ chấp nhận .jpg, .jpeg, .png." });
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + ext;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var relativeUrl = $"/uploads/listings/{uniqueFileName}";
                    
                    var listingImage = new ListingImage
                    {
                        ListingId = listing.ListingId,
                        ImageUrl = relativeUrl,
                        SortOrder = sortOrder++
                    };
                    _context.ListingImages.Add(listingImage);
                }
                
                await _context.SaveChangesAsync();
            }

            return StatusCode(201, new { message = "Đăng tin thành công.", listingId = listing.ListingId });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateListing(int id, [FromBody] ListingUpdateDto dto)
        {
            var listing = await _context.Listings.FindAsync(id);
            if (listing == null)
            {
                return NotFound();
            }

            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (listing.SellerId.ToString() != currentUserIdStr)
            {
                return Forbid();
            }

            listing.Title = dto.Title;
            listing.Description = dto.Description;
            listing.Price = dto.Price;
            listing.Condition = dto.Condition;
            listing.SubjectId = dto.SubjectId;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật tin đăng thành công." });
        }

        [HttpPatch("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateListingStatus(int id, [FromBody] UpdateListingStatusDto dto)
        {
            var listing = await _context.Listings.FindAsync(id);
            if (listing == null)
            {
                return NotFound();
            }

            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (listing.SellerId.ToString() != currentUserIdStr)
            {
                return Forbid();
            }

            // Logic: CHỈ cho phép chuyển từ 0 -> 1 hoặc 0 -> 2
            if (listing.Status != 0)
            {
                return BadRequest(new { message = "Không thể chuyển ngược trạng thái này." });
            }

            if (dto.Status != 1 && dto.Status != 2)
            {
                return BadRequest(new { message = "Trạng thái mới không hợp lệ. Chỉ chấp nhận 1 (Đã bán) hoặc 2 (Đã gỡ)." });
            }

            listing.Status = dto.Status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật trạng thái thành công." });
        }
    }
}
