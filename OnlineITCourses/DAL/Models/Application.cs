namespace DAL.Models;

public partial class Application
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int UserId { get; set; }

    public int CourseId { get; set; }

    public DateTime AppliedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
