namespace DAL.Models;

public partial class Course
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int DurationHours { get; set; }

    public string Level { get; set; } = null!;

    public decimal Price { get; set; }

    public int CourseGroupId { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual CourseGroup CourseGroup { get; set; } = null!;

    public virtual ICollection<CourseInstructor> CourseInstructors { get; set; } = new List<CourseInstructor>();
}
