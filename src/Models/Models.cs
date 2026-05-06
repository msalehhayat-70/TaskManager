using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManager.Models
{
    // ─── Base Entity (Abstraction / Encapsulation) ───────────────────────────
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual void OnUpdate() => UpdatedAt = DateTime.UtcNow;
    }

    // ─── User Model ──────────────────────────────────────────────────────────
    public class User : BaseEntity
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Member"; // Admin / Member

        // Navigation
        public ICollection<ProjectTask> AssignedTasks { get; set; } = new List<ProjectTask>();
        public ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
    }

    // ─── Project Model ───────────────────────────────────────────────────────
    public class Project : BaseEntity
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.Active;

        public DateTime? Deadline { get; set; }

        // FK
        public int OwnerId { get; set; }
        [ForeignKey(nameof(OwnerId))]
        public User? Owner { get; set; }

        // Navigation
        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    }

    // ─── Task Model (Inherits BaseEntity — Inheritance) ──────────────────────
    public class ProjectTask : BaseEntity
    {
        [Required, MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public TaskStatus Status { get; set; } = TaskStatus.Todo;

        public DateTime? DueDate { get; set; }

        // FK
        public int ProjectId { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }

        public int? AssignedToId { get; set; }
        [ForeignKey(nameof(AssignedToId))]
        public User? AssignedTo { get; set; }

        // Polymorphic method
        public virtual string GetStatusBadgeClass() => Status switch
        {
            TaskStatus.Todo       => "badge-secondary",
            TaskStatus.InProgress => "badge-primary",
            TaskStatus.Done       => "badge-success",
            TaskStatus.Cancelled  => "badge-danger",
            _                     => "badge-light"
        };

        public virtual string GetPriorityBadgeClass() => Priority switch
        {
            TaskPriority.Low    => "badge-info",
            TaskPriority.Medium => "badge-warning",
            TaskPriority.High   => "badge-danger",
            _                   => "badge-light"
        };
    }

    // ─── Enums ───────────────────────────────────────────────────────────────
    public enum TaskStatus   { Todo, InProgress, Done, Cancelled }
    public enum TaskPriority { Low, Medium, High }
    public enum ProjectStatus { Active, Completed, Archived }

    // ─── View Models ─────────────────────────────────────────────────────────
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterViewModel
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6), DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class DashboardViewModel
    {
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int TotalProjects { get; set; }
        public List<ProjectTask> RecentTasks { get; set; } = new();
        public List<Project> ActiveProjects { get; set; } = new();
    }
}
