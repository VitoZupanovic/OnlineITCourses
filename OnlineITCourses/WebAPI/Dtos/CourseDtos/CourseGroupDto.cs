using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos.CourseDtos
{
    public class CourseGroupDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

    }
}
