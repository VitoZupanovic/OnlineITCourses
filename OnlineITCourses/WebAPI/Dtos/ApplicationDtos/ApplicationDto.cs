using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos.ApplicationDtos
{
    public class ApplicationDto
    {
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }
    }
}
