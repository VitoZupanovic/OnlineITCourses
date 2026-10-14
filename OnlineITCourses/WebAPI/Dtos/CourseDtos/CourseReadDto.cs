namespace WebAPI.Dtos.CourseDtos
{
    public class CourseReadDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public int DurationHours { get; set; }

        public string Level { get; set; } = null!;

        public decimal Price { get; set; }

        public CourseGroupDto CourseGroup { get; set; } = null!;
    }
}
