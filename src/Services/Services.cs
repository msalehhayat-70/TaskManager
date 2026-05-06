using TaskManager.Models;

namespace TaskManager.Services
{
    // ─── Interfaces ──────────────────────────────────────────────────────────
    public interface ITaskService
    {
        Task<List<ProjectTask>> GetAllAsync(int? projectId = null);
        Task<ProjectTask?> GetByIdAsync(int id);
        Task<ProjectTask> CreateAsync(ProjectTask task);
        Task<ProjectTask> UpdateAsync(ProjectTask task);
        Task<bool> DeleteAsync(int id);
        Task<DashboardViewModel> GetDashboardDataAsync(int userId);
    }

    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<User> RegisterAsync(RegisterViewModel model);
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetByIdAsync(int id);
    }

    // ─── In-Memory Store (no database needed) ────────────────────────────────
    public static class InMemoryStore
    {
        public static List<User> Users = new()
        {
            new User { Id = 1, FullName = "Administrator", Email = "admin@taskmanager.com",
                       PasswordHash = "Admin@123", Role = "Admin" },
            new User { Id = 2, FullName = "John Member",   Email = "john@taskmanager.com",
                       PasswordHash = "Member@123", Role = "Member" }
        };

        public static List<Project> Projects = new()
        {
            new Project { Id = 1, Name = "Sample Project", Description = "Default demo project",
                          Status = ProjectStatus.Active, OwnerId = 1, Owner = Users[0] },
            new Project { Id = 2, Name = "Website Redesign", Description = "Redesign the company website",
                          Status = ProjectStatus.Active, OwnerId = 1, Owner = Users[0] }
        };

        public static List<ProjectTask> Tasks = new()
        {
            new ProjectTask { Id = 1, Title = "Set up project structure", Priority = TaskPriority.High,
                              Status = Models.TaskStatus.Done, ProjectId = 1, Project = Projects[0],
                              AssignedToId = 1, AssignedTo = Users[0],
                              DueDate = DateTime.UtcNow.AddDays(-2) },
            new ProjectTask { Id = 2, Title = "Design database schema", Priority = TaskPriority.Medium,
                              Status = Models.TaskStatus.InProgress, ProjectId = 1, Project = Projects[0],
                              AssignedToId = 2, AssignedTo = Users[1],
                              DueDate = DateTime.UtcNow.AddDays(3) },
            new ProjectTask { Id = 3, Title = "Create wireframes", Priority = TaskPriority.Low,
                              Status = Models.TaskStatus.Todo, ProjectId = 2, Project = Projects[1],
                              AssignedToId = 2, AssignedTo = Users[1],
                              DueDate = DateTime.UtcNow.AddDays(7) },
            new ProjectTask { Id = 4, Title = "Write API documentation", Priority = TaskPriority.Medium,
                              Status = Models.TaskStatus.Todo, ProjectId = 1, Project = Projects[0],
                              DueDate = DateTime.UtcNow.AddDays(5) },
        };

        private static int _nextUserId = 3;
        private static int _nextTaskId = 5;

        public static int NextUserId()  => _nextUserId++;
        public static int NextTaskId()  => _nextTaskId++;
    }

    // ─── Task Service ─────────────────────────────────────────────────────────
    public class TaskService : ITaskService
    {
        public Task<List<ProjectTask>> GetAllAsync(int? projectId = null)
        {
            var tasks = InMemoryStore.Tasks.AsEnumerable();
            if (projectId.HasValue) tasks = tasks.Where(t => t.ProjectId == projectId.Value);
            return Task.FromResult(tasks.OrderByDescending(t => t.CreatedAt).ToList());
        }

        public Task<ProjectTask?> GetByIdAsync(int id) =>
            Task.FromResult(InMemoryStore.Tasks.FirstOrDefault(t => t.Id == id));

        public Task<ProjectTask> CreateAsync(ProjectTask task)
        {
            task.Id = InMemoryStore.NextTaskId();
            task.Project   = InMemoryStore.Projects.FirstOrDefault(p => p.Id == task.ProjectId);
            task.AssignedTo = task.AssignedToId.HasValue
                ? InMemoryStore.Users.FirstOrDefault(u => u.Id == task.AssignedToId)
                : null;
            InMemoryStore.Tasks.Add(task);
            return Task.FromResult(task);
        }

        public Task<ProjectTask> UpdateAsync(ProjectTask task)
        {
            var existing = InMemoryStore.Tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existing != null)
            {
                existing.Title        = task.Title;
                existing.Description  = task.Description;
                existing.Priority     = task.Priority;
                existing.Status       = task.Status;
                existing.DueDate      = task.DueDate;
                existing.ProjectId    = task.ProjectId;
                existing.AssignedToId = task.AssignedToId;
                existing.Project      = InMemoryStore.Projects.FirstOrDefault(p => p.Id == task.ProjectId);
                existing.AssignedTo   = task.AssignedToId.HasValue
                    ? InMemoryStore.Users.FirstOrDefault(u => u.Id == task.AssignedToId)
                    : null;
                existing.OnUpdate();
            }
            return Task.FromResult(existing ?? task);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var task = InMemoryStore.Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return Task.FromResult(false);
            InMemoryStore.Tasks.Remove(task);
            return Task.FromResult(true);
        }

        public Task<DashboardViewModel> GetDashboardDataAsync(int userId)
        {
            var tasks    = InMemoryStore.Tasks;
            var projects = InMemoryStore.Projects.Where(p => p.OwnerId == userId).ToList();
            var now      = DateTime.UtcNow;

            return Task.FromResult(new DashboardViewModel
            {
                TotalTasks      = tasks.Count,
                CompletedTasks  = tasks.Count(t => t.Status == Models.TaskStatus.Done),
                InProgressTasks = tasks.Count(t => t.Status == Models.TaskStatus.InProgress),
                OverdueTasks    = tasks.Count(t => t.DueDate < now && t.Status != Models.TaskStatus.Done),
                TotalProjects   = projects.Count,
                RecentTasks     = tasks.OrderByDescending(t => t.CreatedAt).Take(5).ToList(),
                ActiveProjects  = projects.Where(p => p.Status == ProjectStatus.Active).Take(5).ToList()
            });
        }
    }

    // ─── User Service ─────────────────────────────────────────────────────────
    public class UserService : IUserService
    {
        public Task<User?> AuthenticateAsync(string email, string password)
        {
            // Plain-text password comparison — no BCrypt, no database
            var user = InMemoryStore.Users.FirstOrDefault(
                u => u.Email == email && u.PasswordHash == password);
            return Task.FromResult(user);
        }

        public Task<User> RegisterAsync(RegisterViewModel model)
        {
            var user = new User
            {
                Id           = InMemoryStore.NextUserId(),
                FullName     = model.FullName,
                Email        = model.Email,
                PasswordHash = model.Password,  // stored as plain text for demo
                Role         = "Member"
            };
            InMemoryStore.Users.Add(user);
            return Task.FromResult(user);
        }

        public Task<List<User>> GetAllUsersAsync() =>
            Task.FromResult(InMemoryStore.Users.ToList());

        public Task<User?> GetByIdAsync(int id) =>
            Task.FromResult(InMemoryStore.Users.FirstOrDefault(u => u.Id == id));
    }
}
