using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookExchange.Api.Data;
using BookExchange.Api.DTOs;

namespace BookExchange.Api.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = new AdminStatsDto
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalListings = await _context.Listings.CountAsync(),
                TotalListingsSold = await _context.Listings.CountAsync(l => l.Status == 1),
                TotalMeetingsCompleted = await _context.Meetings.CountAsync(m => m.Status == 3),
                PendingVerifications = await _context.Users.CountAsync(u => u.VerificationStatus == 1),
                PendingReports = await _context.Reports.CountAsync(r => r.Status == 0)
            };

            return Ok(stats);
        }

        [HttpGet("verification-requests")]
        public async Task<IActionResult> GetVerificationRequests()
        {
            var requests = await _context.Users
                .Where(u => u.VerificationStatus == 1)
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new PendingVerificationDto
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    StudentIdFrontUrl = u.StudentIdFrontUrl,
                    StudentIdBackUrl = u.StudentIdBackUrl
                })
                .ToListAsync();

            return Ok(requests);
        }

        [HttpPatch("verification-requests/{userId}/approve")]
        public async Task<IActionResult> ApproveVerification(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng." });
            }

            user.VerificationStatus = 2;
            user.IsVerified = true;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã duyệt xác thực tài khoản." });
        }

        [HttpPatch("verification-requests/{userId}/reject")]
        public async Task<IActionResult> RejectVerification(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng." });
            }

            user.VerificationStatus = 3;
            user.IsVerified = false;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã từ chối xác thực tài khoản." });
        }

        [HttpGet("reports")]
        public async Task<IActionResult> GetReports()
        {
            var reports = await _context.Reports
                .Include(r => r.Reporter)
                .Include(r => r.Listing)
                .Include(r => r.ReportedUser)
                .Where(r => r.Status == 0)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReportDetailDto
                {
                    ReportId = r.ReportId,
                    ReporterFullName = r.Reporter.FullName,
                    Reason = r.Reason,
                    ListingTitle = r.Listing != null ? r.Listing.Title : null,
                    ReportedUserFullName = r.ReportedUser != null ? r.ReportedUser.FullName : (r.Listing != null ? r.Listing.Seller.FullName : null),
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return Ok(reports);
        }

        [HttpPatch("reports/{id}/resolve")]
        public async Task<IActionResult> ResolveReport(int id, [FromBody] ResolveReportRequestDto dto)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null)
            {
                return NotFound(new { message = "Không tìm thấy báo cáo." });
            }

            report.Status = 1;

            if (dto.Action == 1 && report.ListingId.HasValue)
            {
                var listing = await _context.Listings.FindAsync(report.ListingId.Value);
                if (listing != null)
                {
                    listing.Status = 2;
                }
            }
            else if (dto.Action == 2)
            {
                int? targetUserId = report.ReportedUserId;
                if (!targetUserId.HasValue && report.ListingId.HasValue)
                {
                    var listing = await _context.Listings.FindAsync(report.ListingId.Value);
                    if (listing != null)
                    {
                        targetUserId = listing.SellerId;
                    }
                }

                if (targetUserId.HasValue)
                {
                    var targetUser = await _context.Users.FindAsync(targetUserId.Value);
                    if (targetUser != null)
                    {
                        targetUser.IsActive = false;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã xử lý báo cáo thành công." });
        }
    }
}
