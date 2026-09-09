using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookExchange.Api.Data;
using BookExchange.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace BookExchange.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubjectsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetSubjects([FromQuery] int? facultyId)
        {
            var query = _context.Subjects.AsQueryable();

            if (facultyId.HasValue)
            {
                query = query.Where(s => s.FacultyId == facultyId.Value);
            }

            var subjects = await query
                .Select(s => new { s.SubjectId, s.Name, s.FacultyId })
                .ToListAsync();

            return Ok(subjects);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest(new { message = "Tên môn học không được để trống" });

            var exists = await _context.Subjects.AnyAsync(s => s.FacultyId == dto.FacultyId && s.Name == dto.Name);
            if (exists) return BadRequest(new { message = "Môn học này đã tồn tại trong Khoa" });

            var subject = new Subject
            {
                Name = dto.Name,
                FacultyId = dto.FacultyId
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return Ok(new { subject.SubjectId, subject.Name, subject.FacultyId });
        }
    }

    public class CreateSubjectDto
    {
        public string Name { get; set; } = null!;
        public int FacultyId { get; set; }
    }
}
