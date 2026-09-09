using System.Collections.Generic;

namespace BookExchange.Api.Models
{
    public class Faculty
    {
        public int FacultyId { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }
}
