namespace DAL.Models;

public partial class Instructor
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Biography { get; set; }

    public virtual ICollection<CourseInstructor> CourseInstructors { get; set; } = new List<CourseInstructor>();
}
