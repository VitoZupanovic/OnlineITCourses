using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dtos.CourseDtos
{
    public class CourseDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Range(1, int.MaxValue)]
        public int DurationHours { get; set; }

        [Required]
        public string Level { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public int CourseGroupId { get; set; }
    }
}
