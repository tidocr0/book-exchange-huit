namespace BookExchange.Api.Models
{
    public class Subject
    {
        public int SubjectId { get; set; }
        public string Name { get; set; } = null!;
        public int FacultyId { get; set; }

        public Faculty Faculty { get; set; } = null!;
    }
}
