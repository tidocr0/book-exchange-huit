using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookExchange.Api.Data;
using BookExchange.Api.DTOs;

namespace BookExchange.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private const long _maxFileSize = 5 * 1024 * 1024;

        public ProfileController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "Không xác định được danh tính người dùng." });
            }

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin người dùng." });
            }

            var response = new ProfileResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                AvatarUrl = user.AvatarUrl,
                IsVerified = user.IsVerified,
                VerificationStatus = user.VerificationStatus,
                CreatedAt = user.CreatedAt
            };

            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "Không xác định được danh tính người dùng." });
            }

            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                return BadRequest(new { message = "Họ và tên không được để trống." });
            }

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin người dùng." });
            }

            user.FullName = dto.FullName.Trim();
            user.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();

            await _context.SaveChangesAsync();

            var response = new ProfileResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                AvatarUrl = user.AvatarUrl,
                IsVerified = user.IsVerified,
                VerificationStatus = user.VerificationStatus,
                CreatedAt = user.CreatedAt
            };

            return Ok(response);
        }

        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar([FromForm] IFormFile? avatar)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "Không xác định được danh tính người dùng." });
            }

            if (avatar == null || avatar.Length == 0)
            {
                return BadRequest(new { message = "Vui lòng chọn ảnh đại diện." });
            }

            var ext = Path.GetExtension(avatar.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(ext))
            {
                return BadRequest(new { message = "Định dạng tệp không được hỗ trợ. Chỉ chấp nhận .jpg, .jpeg, .png." });
            }

            if (avatar.Length > _maxFileSize)
            {
                return BadRequest(new { message = "Dung lượng ảnh tối đa là 5MB." });
            }

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin người dùng." });
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", "avatars");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatar.CopyToAsync(stream);
            }

            user.AvatarUrl = $"/uploads/avatars/{fileName}";
            await _context.SaveChangesAsync();

            return Ok(new AvatarUploadResponseDto { AvatarUrl = user.AvatarUrl });
        }

        [HttpPost("verification-request")]
        public async Task<IActionResult> SubmitVerification([FromForm] IFormFile? CardImage, [FromForm] IFormFile? FrontImage, [FromForm] IFormFile? BackImage)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "Không xác định được danh tính người dùng." });
            }

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin người dùng." });
            }

            if (user.VerificationStatus == 1)
            {
                return BadRequest(new { message = "Bạn đã gửi yêu cầu xác thực rồi." });
            }

            if (user.VerificationStatus == 2)
            {
                return BadRequest(new { message = "Tài khoản đã được xác thực." });
            }

            var primaryImage = CardImage ?? FrontImage;
            if (primaryImage == null || primaryImage.Length == 0)
            {
                return BadRequest(new { message = "Vui lòng tải lên ảnh thẻ sinh viên." });
            }

            var frontExt = Path.GetExtension(primaryImage.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(frontExt))
            {
                return BadRequest(new { message = "Định dạng tệp không được hỗ trợ. Chỉ chấp nhận .jpg, .jpeg, .png." });
            }

            if (primaryImage.Length > _maxFileSize)
            {
                return BadRequest(new { message = "Dung lượng ảnh tối đa là 5MB." });
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", "students");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var frontFileName = $"{Guid.NewGuid()}{frontExt}";
            var frontFilePath = Path.Combine(uploadsFolder, frontFileName);
            using (var stream = new FileStream(frontFilePath, FileMode.Create))
            {
                await primaryImage.CopyToAsync(stream);
            }

            user.StudentIdFrontUrl = $"/uploads/students/{frontFileName}";

            if (BackImage != null && BackImage.Length > 0)
            {
                var backExt = Path.GetExtension(BackImage.FileName).ToLowerInvariant();
                if (_allowedExtensions.Contains(backExt) && BackImage.Length <= _maxFileSize)
                {
                    var backFileName = $"{Guid.NewGuid()}{backExt}";
                    var backFilePath = Path.Combine(uploadsFolder, backFileName);
                    using (var stream = new FileStream(backFilePath, FileMode.Create))
                    {
                        await BackImage.CopyToAsync(stream);
                    }
                    user.StudentIdBackUrl = $"/uploads/students/{backFileName}";
                }
            }

            user.VerificationStatus = 1;
            user.IsVerified = false;

            await _context.SaveChangesAsync();

            return Ok(new VerificationRequestResponseDto
            {
                VerificationStatus = user.VerificationStatus,
                Message = "Gửi yêu cầu xác thực thành công. Vui lòng chờ Admin duyệt."
            });
        }

        private int? GetCurrentUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("id");
            if (int.TryParse(idStr, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}
