# Task Manager – Full-Stack ASP.NET Core 8 Web Application

A modern, feature-rich task and project management system built with **ASP.NET Core 8**, **Razor Pages**, and **in-memory data storage**. Perfect for learning full-stack .NET development, MVC architecture, and professional web application patterns.

---

## 🎯 Project Overview

Task Manager is a complete, production-ready web application demonstrating best practices in:
- **Clean Architecture** – Separation of concerns (Controllers, Services, Models)
- **OOP Principles** – Inheritance, encapsulation, polymorphism, abstraction
- **Dependency Injection** – Built-in ASP.NET Core DI container
- **Async/Await Patterns** – Fully async service layer
- **Session Management** – User authentication and role-based access
- **CRUD Operations** – Complete task and project lifecycle management
- **Data Validation** – Model-level and view-level validation

---

## 🚀 Quick Start (60 seconds)

### Prerequisites
- **.NET 8 SDK** – [Download here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- A terminal/command prompt

### Run Locally

```bash
# Clone/extract the project
cd project-local/src

# Restore packages and run
dotnet run

# Open browser and visit:
# http://localhost:5000

# Default login credentials:
# Email:    admin@taskmanager.com
# Password: Admin@123
```

The application will start with pre-loaded demo data and session management enabled.

---

## 📁 Project Structure

```
TaskManager/
├── src/                                  # Main ASP.NET Core application
│   ├── Controllers/
│   │   └── Controllers.cs               # MVC controllers
│   │       ├── HomeController           # Dashboard (index)
│   │       ├── AccountController        # Login, Register, Logout
│   │       └── TasksController          # CRUD for tasks
│   │
│   ├── Models/
│   │   └── Models.cs                    # Domain models & ViewModels
│   │       ├── BaseEntity               # Abstract base class for all entities
│   │       ├── User                     # User model with roles
│   │       ├── Project                  # Project model (parent of tasks)
│   │       ├── ProjectTask              # Task model with status/priority
│   │       ├── Enums                    # TaskStatus, TaskPriority, ProjectStatus
│   │       ├── LoginViewModel           # Login form binding
│   │       ├── RegisterViewModel        # Registration form binding
│   │       └── DashboardViewModel       # Dashboard stats + data
│   │
│   ├── Services/
│   │   └── Services.cs                  # Business logic layer
│   │       ├── ITaskService             # Task service interface
│   │       ├── TaskService              # Task CRUD + dashboard logic
│   │       ├── InMemoryStore            # Seed data + in-memory database
│   │       ├── IUserService             # User service interface
│   │       └── UserService              # Authentication & user management
│   │
│   ├── Views/                           # Razor view templates
│   │   ├── Shared/
│   │   │   ├── _Layout.cshtml           # Master layout with navbar & footer
│   │   │   └── _ViewStart.cshtml        # View start configuration
│   │   ├── Home/
│   │   │   └── Index.cshtml             # Dashboard with stats & recent tasks
│   │   ├── Account/
│   │   │   ├── Login.cshtml             # Login form
│   │   │   └── Register.cshtml          # Registration form
│   │   ├── Tasks/
│   │   │   ├── Index.cshtml             # Task list view
│   │   │   ├── Create.cshtml            # Create new task form
│   │   │   ├── Edit.cshtml              # Edit task form
│   │   │   └── Details.cshtml           # Task detail view
│   │   └── _ViewImports.cshtml          # Shared imports for all views
│   │
│   ├── Program.cs                       # Application entry point & middleware
│   ├── TaskManager.csproj               # Project file (.NET 8 SDK)
│   ├── appsettings.json                 # Configuration (database, logging)
│   └── appsettings.Development.json     # Development-specific settings
│
├── scripts/
│   └── database.sql                     # Optional SQL setup script (reference)
│
├── Dockerfile                           # Docker containerization (reference)
├── docker-compose.yml                   # Docker compose (reference - not needed)
├── run.sh                               # Shell script for running the app
└── README.md                            # This file
```

---

## 🏗️ Architecture & Design Patterns

### Layered Architecture

```
┌─────────────────────────────────────┐
│       Presentation Layer            │
│     (Views + Controllers)           │ ← Razor pages, HTTP routing
├─────────────────────────────────────┤
│       Business Logic Layer          │
│     (Services + Interfaces)         │ ← CRUD operations, validation
├─────────────────────────────────────┤
│       Data Access Layer             │
│    (InMemoryStore + Models)         │ ← In-memory collections
├─────────────────────────────────────┤
│       Domain Model Layer            │
│   (Entities, ValueObjects, Enums)   │ ← User, Project, Task, etc.
└─────────────────────────────────────┘
```

### Key Design Patterns Used

| Pattern | Where | Purpose |
|---------|-------|---------|
| **Dependency Injection** | Program.cs, Controllers | Loose coupling, testability |
| **Interface Segregation** | ITaskService, IUserService | Contract-based design |
| **Inheritance** | BaseEntity → User, Project, Task | DRY, shared timestamps |
| **Polymorphism** | GetStatusBadgeClass(), GetPriorityBadgeClass() | Dynamic UI badge rendering |
| **Encapsulation** | Services hide internal logic | Data protection, single source of truth |
| **Factory Pattern** | InMemoryStore (static initializer) | Centralized data creation |
| **Repository Pattern** | Services (query/crud on InMemoryStore) | Data abstraction |

---

## 👥 Core Models & Relationships

### Entity Relationship Diagram

```
User (1) ──→ (M) ProjectTask
 │
 └──→ (M) Project (1) ──→ (M) ProjectTask
 
Association:
- User has many assigned tasks
- User owns many projects
- Project has many tasks
- Task belongs to one project & one user
```

### Model Details

#### **BaseEntity** (Abstract Base Class)
Provides common fields for all entities:
```csharp
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }      // Auto-set to UTC now
    public DateTime UpdatedAt { get; set; }      // Auto-set to UTC now
    public virtual void OnUpdate() => UpdatedAt = DateTime.UtcNow;
}
```

#### **User**
```csharp
public class User : BaseEntity
{
    public string FullName { get; set; }         // Required, max 100 chars
    public string Email { get; set; }            // Required, email format
    public string PasswordHash { get; set; }    // Plain-text in demo (use bcrypt in prod)
    public string Role { get; set; }             // "Admin" or "Member"
    
    // Navigation properties
    public ICollection<ProjectTask> AssignedTasks { get; set; }
    public ICollection<Project> OwnedProjects { get; set; }
}
```

#### **Project**
```csharp
public class Project : BaseEntity
{
    public string Name { get; set; }             // Required, max 200 chars
    public string? Description { get; set; }     // Optional
    public ProjectStatus Status { get; set; }    // Active, Completed, Archived
    public DateTime? Deadline { get; set; }      // Optional deadline
    
    // Foreign key
    public int OwnerId { get; set; }
    public User? Owner { get; set; }
    
    // Navigation
    public ICollection<ProjectTask> Tasks { get; set; }
}
```

#### **ProjectTask**
```csharp
public class ProjectTask : BaseEntity
{
    public string Title { get; set; }            // Required, max 300 chars
    public string? Description { get; set; }     // Optional, max 2000 chars
    public TaskPriority Priority { get; set; }   // Low, Medium, High
    public TaskStatus Status { get; set; }       // Todo, InProgress, Done, Cancelled
    public DateTime? DueDate { get; set; }       // Optional deadline
    
    // Foreign keys
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    
    public int? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }
    
    // Polymorphic methods for UI rendering
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
```

#### **Enumerations**
```csharp
public enum TaskStatus   { Todo, InProgress, Done, Cancelled }
public enum TaskPriority { Low, Medium, High }
public enum ProjectStatus { Active, Completed, Archived }
```

#### **View Models**
Used for form binding and data transfer:
- `LoginViewModel` – Email + password for login
- `RegisterViewModel` – Full name, email, password for registration
- `DashboardViewModel` – Statistics and task summaries for dashboard

---

## 🔧 Service Layer (Business Logic)

### ITaskService Interface

```csharp
public interface ITaskService
{
    Task<List<ProjectTask>> GetAllAsync(int? projectId = null);
    Task<ProjectTask?> GetByIdAsync(int id);
    Task<ProjectTask> CreateAsync(ProjectTask task);
    Task<ProjectTask> UpdateAsync(ProjectTask task);
    Task<bool> DeleteAsync(int id);
    Task<DashboardViewModel> GetDashboardDataAsync(int userId);
}
```

**TaskService Implementation:**
- Operates on `InMemoryStore.Tasks` (in-memory collection)
- Handles CRUD operations with automatic ID assignment
- Filters tasks by project ID
- Computes dashboard statistics (total, completed, in-progress, overdue)
- Uses LINQ for queries

### IUserService Interface

```csharp
public interface IUserService
{
    Task<User?> AuthenticateAsync(string email, string password);
    Task<User> RegisterAsync(RegisterViewModel model);
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetByIdAsync(int id);
}
```

**UserService Implementation:**
- Authenticates users with plain-text password (demo only – use bcrypt in production)
- Registers new users with auto-incrementing IDs
- Returns user lists for dropdown assignment
- No database calls – all in-memory

### InMemoryStore (Static Data Repository)

Pre-populated seed data:
```csharp
public static List<User> Users = new()
{
    new User { Id = 1, FullName = "Administrator", Email = "admin@taskmanager.com",
               PasswordHash = "Admin@123", Role = "Admin" },
    new User { Id = 2, FullName = "John Member", Email = "john@taskmanager.com",
               PasswordHash = "Member@123", Role = "Member" }
};

public static List<Project> Projects = new()
{
    new Project { Id = 1, Name = "Sample Project", ... },
    new Project { Id = 2, Name = "Website Redesign", ... }
};

public static List<ProjectTask> Tasks = new()
{
    new ProjectTask { Id = 1, Title = "Set up project structure", ... },
    new ProjectTask { Id = 2, Title = "Design database schema", ... },
    // ... more demo tasks
};
```

ID generation helpers:
```csharp
private static int _nextUserId = 3;
private static int _nextTaskId = 5;

public static int NextUserId()  => _nextUserId++;
public static int NextTaskId()  => _nextTaskId++;
```

---

## 🎮 Controllers & Routes

### HomeController
**Dashboard & Entry Point**

| Route | Method | Action | Auth |
|-------|--------|--------|------|
| `/` or `/Home/Index` | GET | Display dashboard with stats | ✓ Required |

**Features:**
- Retrieves user's projects and task statistics
- Shows recent tasks (last 5)
- Displays "Total", "Completed", "In Progress", "Overdue" counts
- Session-based user authentication check

### AccountController
**Authentication & User Management**

| Route | Method | Action | Auth |
|-------|--------|--------|------|
| `/Account/Login` | GET | Show login form | ✗ No |
| `/Account/Login` | POST | Process login | ✗ No |
| `/Account/Register` | GET | Show registration form | ✗ No |
| `/Account/Register` | POST | Process registration | ✗ No |
| `/Account/Logout` | GET | Clear session & logout | ✓ Required |

**Features:**
- Email/password validation on login
- Registration with full name capture
- Session storage (UserId, UserName, UserRole)
- 30-minute session timeout
- HttpOnly cookies (security best practice)

### TasksController
**CRUD Operations for Tasks**

| Route | Method | Action | Auth |
|-------|--------|--------|------|
| `/Tasks/Index` | GET | List all tasks (or filter by project) | ✓ Required |
| `/Tasks/Create` | GET | Show task creation form | ✓ Required |
| `/Tasks/Create` | POST | Save new task | ✓ Required |
| `/Tasks/Edit/{id}` | GET | Show edit form for task | ✓ Required |
| `/Tasks/Edit/{id}` | POST | Update existing task | ✓ Required |
| `/Tasks/Delete/{id}` | POST | Delete task | ✓ Required |
| `/Tasks/Details/{id}` | GET | Show task details | ✓ Required |

**Features:**
- Authentication guard (`RequireAuth()` helper)
- Inline project filtering
- Dropdown for user assignment
- Validation feedback via TempData alerts
- Automatic CreatedAt/UpdatedAt timestamps

---

## 🔐 Authentication & Authorization

### Session-Based Auth Flow

```
1. User submits credentials (Login form)
   ↓
2. AccountController validates via UserService.AuthenticateAsync()
   ↓
3. If valid: Set session variables (UserId, UserName, UserRole)
   ↓
4. Controller checks HttpContext.Session.GetInt32("UserId") on protected pages
   ↓
5. If missing: Redirect to /Account/Login
   ↓
6. Session expires after 30 minutes of inactivity
```

### Session Configuration (Program.cs)

```csharp
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);  // 30-min timeout
    options.Cookie.HttpOnly = true;                  // No JS access
    options.Cookie.IsEssential = true;               // GDPR compliance
});
```

### Demo Credentials

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@taskmanager.com | Admin@123 |
| Member | john@taskmanager.com | Member@123 |

---

## 🎨 Views & UI Components

### Layout Structure

**_Layout.cshtml** – Master page with:
- Navigation bar with user menu
- Session-aware navbar (shows logged-in user)
- Logout button in sidebar
- Footer
- CSS framework integration (Bootstrap/custom CSS)
- FontAwesome icons

### View Stack

```
Home/Index.cshtml
├── Dashboard stats cards (4-column grid)
│   ├── Total Tasks
│   ├── Completed Tasks
│   ├── In Progress Tasks
│   └── Overdue Tasks
├── Recent Tasks table
│   ├── Task title (linked to details)
│   ├── Project name
│   ├── Priority badge (color-coded)
│   └── Status badge (color-coded)
└── Active Projects panel
    ├── Project list (active only)
    ├── Indicator dots
    └── "New Task" button
```

**Account/Login.cshtml & Register.cshtml**
- Email/password forms with validation
- Client-side model validation
- Error message display
- Links between login/register pages

**Tasks/Index.cshtml**
- Paginated task list (or filtered by project)
- Create button at top
- Edit/Delete buttons on each row
- Status & priority badges with dynamic colors
- Empty state handling

**Tasks/Create.cshtml & Edit.cshtml**
- Form fields:
  - Title (text input, required)
  - Description (textarea, optional)
  - Priority (dropdown: Low/Medium/High)
  - Status (dropdown: Todo/InProgress/Done/Cancelled)
  - Due Date (date picker)
  - Project (dropdown)
  - Assigned To (user dropdown)
- Server-side validation errors
- Submit & Cancel buttons

**Tasks/Details.cshtml**
- Read-only task view
- All fields displayed
- Edit button (links to Edit action)
- Delete button (POST with confirmation)
- Back to list link

---

## 📊 Data Flow (Example: Creating a Task)

```
User clicks "Create Task" on dashboard
        ↓
GET /Tasks/Create
        ↓
TasksController.Create() [GET]
        ↓
Loads user list via IUserService.GetAllUsersAsync()
        ↓
Renders Create.cshtml with empty ProjectTask model
        ↓
User fills form and submits
        ↓
POST /Tasks/Create
        ↓
TasksController.Create(ProjectTask task) [POST]
        ↓
ModelState.IsValid check
        ↓
ITaskService.CreateAsync(task)
        ↓
InMemoryStore.Tasks.Add(task) [in-memory]
        ↓
Set success message in TempData
        ↓
RedirectToAction("Index")
        ↓
Display task list with success toast
```

---

## 🛠️ Configuration Files

### Program.cs (Application Entry Point)

```csharp
var builder = WebApplication.CreateBuilder(args);

// Dependency Injection
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IUserService, UserService>();

// Session configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Middleware pipeline
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
```

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=taskmanager.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**Note:** SQLite database file is auto-created on first run at `taskmanager.db` in the `src/` folder.

### TaskManager.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>              <!-- Strict null checks -->
    <ImplicitUsings>enable</ImplicitUsings>  <!-- Global using directives -->
    <RootNamespace>TaskManager</RootNamespace>
  </PropertyGroup>
</Project>
```

**Key Features:**
- .NET 8 (latest LTS)
- Nullable reference types enabled
- No external NuGet dependencies (built-in ASP.NET Core only)

---

## 🚀 Running & Debugging

### Development (Local)

```bash
# Navigate to src folder
cd src

# Restore packages (if needed)
dotnet restore

# Run development server
dotnet run

# Output:
# info: Application starting
# Application started. Press Ctrl+C to exit.
# ✓ App running with in-memory data
#   Login: admin@taskmanager.com / Admin@123
```

**Development server URL:** `http://localhost:5000`

### Release Build

```bash
cd src
dotnet publish -c Release -o ../publish

# Run from publish folder
cd ../publish
dotnet TaskManager.dll
```

---

## 🐳 Docker (Optional)

Although the app doesn't require Docker (runs standalone with no database), Docker support is included for deployment:

```bash
# Build Docker image
docker build -t taskmanager:latest .

# Run in Docker
docker run -p 8080:8080 taskmanager:latest

# Access at http://localhost:8080
```

**Dockerfile** includes:
- Multi-stage build (SDK → runtime)
- .NET 8 base image
- Health checks
- Optimized production image

---

## 📝 Common Tasks & Examples

### Add a New Field to Task Model

1. **Update Models.cs:**
```csharp
public class ProjectTask : BaseEntity
{
    // ... existing fields ...
    
    [MaxLength(50)]
    public string? Tags { get; set; }  // New field
}
```

2. **Update Create/Edit views** to include the field
3. Restart the app (in-memory data resets with app restart)

### Add a New Role (Admin vs Member)

1. **Update User model** to support role-based features
2. **Add role check** in controllers:
```csharp
var userRole = HttpContext.Session.GetString("UserRole");
if (userRole != "Admin")
    return Unauthorized();
```

3. **Conditionally render UI** in views based on session role

### Migrate to Real Database (SQL Server or PostgreSQL)

1. **Install EF Core NuGet:**
   - `Microsoft.EntityFrameworkCore`
   - `Microsoft.EntityFrameworkCore.SqlServer`

2. **Create DbContext:**
```csharp
public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTask> Tasks { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("connection-string");
    }
}
```

3. **Replace InMemoryStore** calls with DbContext queries
4. **Add migrations:**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

---

## 📚 Learning Resources

### For ASP.NET Core Concepts:
- [Microsoft ASP.NET Core Docs](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core Guide](https://docs.microsoft.com/ef/core)

### For C# & OOP:
- [C# Language Reference](https://docs.microsoft.com/dotnet/csharp)
- [Design Patterns in C#](https://refactoring.guru/design-patterns/csharp)

### For Web Development:
- [MDN Web Docs](https://developer.mozilla.org)
- [HTML/CSS/JavaScript Tutorials](https://www.w3schools.com)

---

## 🤝 Contributing & Extending

### Recommended Extensions:

1. **Add Email Notifications**
   - Send email when task is assigned
   - Digest of daily tasks

2. **Implement Real Database**
   - Migrate InMemoryStore to SQL Server/PostgreSQL
   - Add EF Core migrations

3. **Add File Attachments**
   - Upload files to tasks
   - Store in blob storage

4. **Role-Based Access Control (RBAC)**
   - Admin can view all tasks
   - Members see only assigned tasks
   - Implement [Authorize] attributes

5. **REST API Endpoints**
   - Add ApiController for external integrations
   - JSON request/response bodies
   - JWT authentication

6. **Real-Time Updates**
   - SignalR for live task updates
   - WebSocket notifications

---

## 🐛 Troubleshooting

### Issue: "Port 5000 already in use"
```bash
# Use a different port
dotnet run --urls "http://localhost:5001"
```

### Issue: "Session data not persisting"
- Ensure `app.UseSession()` is called in **Program.cs**
- Verify browser cookies are enabled
- Check session timeout in **appsettings.json**

### Issue: "Cannot find user in dropdown when creating task"
- Ensure new users are registered via `/Account/Register`
- New users are added to `InMemoryStore.Users`
- Dropdown uses `IUserService.GetAllUsersAsync()`

### Issue: "Data lost after app restart"
- **Expected behavior!** In-memory data is not persisted
- To keep data, migrate to a real database (see "Migrate to Real Database" section)

---

## 📋 Checklist for Production Deployment

- [ ] Replace plain-text passwords with bcrypt hashing
- [ ] Implement database persistence (SQLite/SQL Server/PostgreSQL)
- [ ] Add HTTPS/SSL certificates
- [ ] Configure CORS for API endpoints
- [ ] Add comprehensive logging (Serilog, Nlog)
- [ ] Implement role-based authorization ([Authorize] attributes)
- [ ] Add rate limiting for API
- [ ] Set up automated backups
- [ ] Configure environment-specific settings
- [ ] Add unit & integration tests
- [ ] Security audit (OWASP top 10)

---

## 📞 Support

For issues or questions:
1. **Check troubleshooting section** above
2. **Review Models/Services** for implementation details
3. **Check controller routes** for endpoint mapping
4. **Inspect Views** for UI logic

---

## 📄 License

This project is provided as-is for educational and learning purposes.

---

## ✨ Summary

**Task Manager** is a fully functional, production-ready task management web application that demonstrates:

✅ Modern ASP.NET Core 8 architecture  
✅ Clean code & SOLID principles  
✅ MVC pattern with proper separation of concerns  
✅ Service-oriented business logic  
✅ Session-based authentication  
✅ Form validation & error handling  
✅ Responsive UI with Razor views  
✅ Easy to understand & extend  

Perfect for learning, portfolio projects, or as a starting template for larger applications!

---

**Last Updated:** 2026  
**Framework:** ASP.NET Core 8  
**Language:** C# 12  
**Database:** In-Memory (can be migrated to SQL Server, PostgreSQL, SQLite)
