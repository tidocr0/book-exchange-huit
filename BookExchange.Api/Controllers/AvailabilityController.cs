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
    public class AvailabilityController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AvailabilityController(AppDbContext context)
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
        [HttpGet("my-schedule")]
        public async Task<ActionResult<ScheduleConfigDto>> GetMySchedule()
        {
            var userId = GetCurrentUserId();

            var availabilities = await _context.SellerAvailabilities
                .Where(a => a.UserId == userId)
                .Select(a => new TimeSlotDto { DayOfWeek = a.DayOfWeek, TimeSlot = a.TimeSlot })
                .ToListAsync();

            var blackouts = await _context.SellerBlackoutDates
                .Where(b => b.UserId == userId && b.BlackoutDate >= DateTime.Today)
                .Select(b => new BlackoutDateDto
                {
                    BlackoutId = b.BlackoutId,
                    BlackoutDate = b.BlackoutDate,
                    TimeSlot = b.TimeSlot
                })
                .ToListAsync();

            return Ok(new ScheduleConfigDto
            {
                Availabilities = availabilities,
                BlackoutDates = blackouts
            });
        }

        [Authorize]
        [HttpPut("my-schedule")]
        public async Task<IActionResult> UpdateMySchedule(UpdateAvailabilityDto dto)
        {
            var userId = GetCurrentUserId();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Remove existing availabilities
                var existing = await _context.SellerAvailabilities.Where(a => a.UserId == userId).ToListAsync();
                _context.SellerAvailabilities.RemoveRange(existing);
                await _context.SaveChangesAsync();

                // Add new ones, avoiding duplicates in the DTO
                var distinctSlots = dto.Slots.GroupBy(s => new { s.DayOfWeek, s.TimeSlot }).Select(g => g.First());
                foreach (var slot in distinctSlots)
                {
                    _context.SellerAvailabilities.Add(new SellerAvailability
                    {
                        UserId = userId,
                        DayOfWeek = slot.DayOfWeek,
                        TimeSlot = slot.TimeSlot
                    });
                }
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Lỗi khi cập nhật lịch rảnh.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("blackout")]
        public async Task<ActionResult<BlackoutDateDto>> AddBlackoutDate(CreateBlackoutDateDto dto)
        {
            var userId = GetCurrentUserId();

            var existing = await _context.SellerBlackoutDates.FirstOrDefaultAsync(b => 
                b.UserId == userId && 
                b.BlackoutDate == dto.BlackoutDate.Date && 
                b.TimeSlot == dto.TimeSlot);

            if (existing != null)
            {
                return BadRequest(new { message = "Lịch bận này đã tồn tại." });
            }

            var blackout = new SellerBlackoutDate
            {
                UserId = userId,
                BlackoutDate = dto.BlackoutDate.Date,
                TimeSlot = dto.TimeSlot
            };

            _context.SellerBlackoutDates.Add(blackout);
            await _context.SaveChangesAsync();

            return Ok(new BlackoutDateDto
            {
                BlackoutId = blackout.BlackoutId,
                BlackoutDate = blackout.BlackoutDate,
                TimeSlot = blackout.TimeSlot
            });
        }

        [Authorize]
        [HttpDelete("blackout/{id}")]
        public async Task<IActionResult> RemoveBlackoutDate(int id)
        {
            var userId = GetCurrentUserId();
            var blackout = await _context.SellerBlackoutDates.FirstOrDefaultAsync(b => b.BlackoutId == id && b.UserId == userId);
            
            if (blackout == null) return NotFound();

            _context.SellerBlackoutDates.Remove(blackout);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{sellerId}/dates")]
        public async Task<ActionResult<List<AvailableDateDto>>> GetAvailableDates(int sellerId, [FromQuery] int days = 14)
        {
            var today = DateTime.Today;
            var endDate = today.AddDays(days);

            // Get standard availabilities for this seller
            var availabilities = await _context.SellerAvailabilities
                .Where(a => a.UserId == sellerId)
                .ToListAsync();

            // Get blackout dates for this seller in the date range
            var blackouts = await _context.SellerBlackoutDates
                .Where(b => b.UserId == sellerId && b.BlackoutDate >= today && b.BlackoutDate <= endDate)
                .ToListAsync();

            // Get existing active meetings for this seller
            var existingMeetings = await _context.Meetings
                .Include(m => m.Listing)
                .Where(m => m.Listing.SellerId == sellerId && 
                            m.ProposedTime >= today && m.ProposedTime <= endDate.AddDays(1) &&
                            (m.Status == 0 || m.Status == 1))
                .ToListAsync();

            var result = new List<AvailableDateDto>();

            for (int i = 0; i < days; i++)
            {
                var currentDate = today.AddDays(i);
                byte dayOfWeek = (byte)currentDate.DayOfWeek; // 0 = Sunday

                // Find default slots for this day of week
                var defaultSlots = availabilities
                    .Where(a => a.DayOfWeek == dayOfWeek)
                    .Select(a => a.TimeSlot)
                    .ToList();

                if (!defaultSlots.Any()) continue; // Not available this day by default

                // Apply blackouts
                var currentBlackouts = blackouts.Where(b => b.BlackoutDate == currentDate).ToList();
                if (currentBlackouts.Any(b => b.TimeSlot == null))
                {
                    // Busy all day
                    continue;
                }

                // Remove specific blackout slots
                var blackoutSlots = currentBlackouts.Where(b => b.TimeSlot.HasValue).Select(b => b.TimeSlot.Value).ToList();
                var availableSlots = defaultSlots.Except(blackoutSlots).ToList();

                var meetingSlots = existingMeetings
                    .Where(m => m.ProposedTime.Date == currentDate)
                    .Select(m => 
                    {
                        var hour = m.ProposedTime.Hour;
                        if (hour >= 7 && hour < 9) return (byte)0;
                        if (hour >= 9 && hour < 11) return (byte)1;
                        if (hour >= 11 && hour < 13) return (byte)2;
                        if (hour >= 13 && hour < 15) return (byte)3;
                        if (hour >= 15 && hour < 17) return (byte)4;
                        if (hour >= 17 && hour < 19) return (byte)5;
                        if (hour >= 19 && hour < 21) return (byte)6;
                        return (byte)255;
                    })
                    .Where(s => s != 255)
                    .ToList();
                
                availableSlots = availableSlots.Except(meetingSlots).ToList();

                if (currentDate == today)
                {
                    var currentHour = DateTime.Now.Hour;
                    availableSlots = availableSlots.Where(s => 
                    {
                        var slotEndHour = s switch
                        {
                            0 => 9,
                            1 => 11,
                            2 => 13,
                            3 => 15,
                            4 => 17,
                            5 => 19,
                            6 => 21,
                            _ => 0
                        };
                        return slotEndHour > currentHour;
                    }).ToList();
                }

                if (availableSlots.Any())
                {
                    availableSlots.Sort();
                    result.Add(new AvailableDateDto
                    {
                        Date = currentDate,
                        AvailableSlots = availableSlots
                    });
                }
            }

            return Ok(result);
        }
    }
}
