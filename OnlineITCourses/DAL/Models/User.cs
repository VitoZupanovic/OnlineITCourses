namespace DAL.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PwdSalt { get; set; } = null!;

    public string PwdHash { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public int RoleId { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual Role Role { get; set; } = null!;
}
