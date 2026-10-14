using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos.CourseDtos
{
    public class InstructorDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public string? Biography { get; set; }
    }
}
