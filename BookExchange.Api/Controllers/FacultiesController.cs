using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookExchange.Api.Data;
using BookExchange.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace BookExchange.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacultiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FacultiesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetFaculties()
        {
            var faculties = await _context.Faculties
                .Select(f => new { f.FacultyId, f.Name })
                .ToListAsync();
            return Ok(faculties);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateFaculty([FromBody] CreateFacultyDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { message = "Tên khoa không được để trống." });

            var exists = await _context.Faculties.AnyAsync(f => f.Name == dto.Name);
            if (exists)
                return BadRequest(new { message = "Khoa này đã tồn tại." });

            var faculty = new Faculty
            {
                Name = dto.Name
            };

            _context.Faculties.Add(faculty);
            await _context.SaveChangesAsync();

            return Ok(new { facultyId = faculty.FacultyId });
        }
    }

    public class CreateFacultyDto
    {
        public string Name { get; set; } = null!;
    }
}
