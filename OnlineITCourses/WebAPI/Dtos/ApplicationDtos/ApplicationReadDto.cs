using WebAPI.Dtos.AuthDtos;
using WebAPI.Dtos.CourseDtos;

namespace WebAPI.Dtos.ApplicationDtos
{
    public class ApplicationReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public CourseSmallDto Course { get; set; } = null!;
        public UserSmallDto User { get; set; } = null!;
        public DateTime AppliedAt { get; set; }
        public string Status { get; set; } = null!;
    }
}
