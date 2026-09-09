using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookExchange.Api.Data;
using BookExchange.Api.Models;
using BookExchange.Api.DTOs;
using System.Security.Claims;

namespace BookExchange.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MeetingsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateMeeting(CreateMeetingDto dto)
        {
            var buyerId = GetCurrentUserId();

            var listing = await _context.Listings.FindAsync(dto.ListingId);
            if (listing == null)
            {
                return NotFound(new { message = "Không tìm thấy tin đăng." });
            }

            if (listing.SellerId == buyerId)
            {
                return BadRequest(new { message = "Bạn không thể đặt lịch hẹn với chính mình." });
            }

            if (listing.Status != 0)
            {
                return BadRequest(new { message = "Tin đăng này không còn khả dụng." });
            }

            // Check if there is already a pending or accepted meeting for this listing by this buyer
            var existingMeeting = await _context.Meetings
                .FirstOrDefaultAsync(m => m.ListingId == dto.ListingId && m.BuyerId == buyerId && (m.Status == 0 || m.Status == 1));

            if (existingMeeting != null)
            {
                return BadRequest(new { message = "Bạn đã có lịch hẹn đang xử lý với tin đăng này." });
            }

            var meeting = new Meeting
            {
                ListingId = dto.ListingId,
                BuyerId = buyerId,
                ProposedTime = dto.ProposedTime,
                Location = dto.Location,
                Status = 0
            };

            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();

            return Ok(meeting);
        }

        [Authorize]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateMeetingStatus(int id, UpdateMeetingStatusDto dto)
        {
            var userId = GetCurrentUserId();
            var meeting = await _context.Meetings
                .Include(m => m.Listing)
                .FirstOrDefaultAsync(m => m.MeetingId == id);

            if (meeting == null)
            {
                return NotFound();
            }

            bool isSeller = meeting.Listing.SellerId == userId;
            bool isBuyer = meeting.BuyerId == userId;

            if (!isSeller && !isBuyer)
            {
                return Forbid();
            }

            // Status transitions:
            // 0: Pending -> 1: Accepted (Seller), 2: Rejected/Cancelled (Seller/Buyer)
            // 1: Accepted -> 3: Completed (Seller/Buyer), 2: Cancelled (Seller/Buyer)
            
            if (dto.Status == 1) // Accept
            {
                if (!isSeller) return Forbid();
                if (meeting.Status != 0) return BadRequest(new { message = "Trạng thái không hợp lệ." });
                meeting.Status = 1;
            }
            else if (dto.Status == 2) // Reject / Cancel
            {
                if (meeting.Status == 3) return BadRequest(new { message = "Không thể hủy lịch đã hoàn tất." });
                meeting.Status = 2;
            }
            else if (dto.Status == 3) // Complete
            {
                if (meeting.Status != 1) return BadRequest(new { message = "Chỉ có thể hoàn tất lịch đã xác nhận." });
                meeting.Status = 3;
            }
            else
            {
                return BadRequest(new { message = "Trạng thái không hợp lệ." });
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã cập nhật trạng thái lịch hẹn." });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMyMeetings()
        {
            var userId = GetCurrentUserId();
            var meetings = await _context.Meetings
                .Include(m => m.Listing)
                .ThenInclude(l => l.Seller)
                .Include(m => m.Buyer)
                .Where(m => m.BuyerId == userId || m.Listing.SellerId == userId)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new
                {
                    m.MeetingId,
                    m.ListingId,
                    ListingTitle = m.Listing.Title,
                    m.ProposedTime,
                    m.Location,
                    m.Status,
                    m.CreatedAt,
                    IsSeller = m.Listing.SellerId == userId,
                    PartnerName = m.Listing.SellerId == userId ? m.Buyer.FullName : m.Listing.Seller.FullName,
                    PartnerPhone = m.Listing.SellerId == userId ? m.Buyer.Phone : m.Listing.Seller.Phone
                })
                .ToListAsync();

            return Ok(meetings);
        }
    }
}
