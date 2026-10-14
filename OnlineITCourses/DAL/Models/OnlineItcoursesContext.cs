using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class OnlineItcoursesContext : DbContext
{
    public OnlineItcoursesContext()
    {
    }

    public OnlineItcoursesContext(DbContextOptions<OnlineItcoursesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseGroup> CourseGroups { get; set; }

    public virtual DbSet<CourseInstructor> CourseInstructors { get; set; }

    public virtual DbSet<Instructor> Instructors { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.ToTable("Application");

            entity.HasIndex(e => new { e.UserId, e.CourseId }, "UQ_Application_User_Course").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Course).WithMany(p => p.Applications)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Application_Course");

            entity.HasOne(d => d.User).WithMany(p => p.Applications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Application_User");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Course");

            entity.HasIndex(e => e.Name, "UQ_Course_Name").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Level).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.CourseGroup).WithMany(p => p.Courses)
                .HasForeignKey(d => d.CourseGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Course_CourseGroup");
        });

        modelBuilder.Entity<CourseGroup>(entity =>
        {
            entity.ToTable("CourseGroup");

            entity.HasIndex(e => e.Name, "UQ_CourseGroup_Name").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<CourseInstructor>(entity =>
        {
            entity.ToTable("CourseInstructor");

            entity.HasIndex(e => new { e.CourseId, e.InstructorId }, "UQ_CourseInstructor").IsUnique();

            entity.HasOne(d => d.Course).WithMany(p => p.CourseInstructors)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseInstructor_Course");

            entity.HasOne(d => d.Instructor).WithMany(p => p.CourseInstructors)
                .HasForeignKey(d => d.InstructorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseInstructor_Instructor");
        });

        modelBuilder.Entity<Instructor>(entity =>
        {
            entity.ToTable("Instructor");

            entity.HasIndex(e => e.Email, "UQ_Instructor_Email").IsUnique();

            entity.Property(e => e.Biography).HasMaxLength(1000);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Name).HasMaxLength(150);
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.ToTable("Log");

            entity.Property(e => e.Message).HasMaxLength(1024);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.HasIndex(e => e.Name, "UQ_Role_Name").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ_User_Email").IsUnique();

            entity.HasIndex(e => e.Username, "UQ_User_Username").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.PwdHash).HasMaxLength(256);
            entity.Property(e => e.PwdSalt).HasMaxLength(256);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
