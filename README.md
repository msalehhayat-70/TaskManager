# Task Manager – Containerized Full-Stack App (C# + Docker)

## Quick Start (Docker Compose)
```bash
docker-compose up --build
```
Open: http://localhost:8080
Login: admin@taskmanager.com / Admin@123

## Project Structure
```
TaskManager/
├── src/                          # ASP.NET Core 8 application
│   ├── Controllers/Controllers.cs  # MVC + REST API controllers
│   ├── Models/Models.cs            # Domain models + ViewModels
│   ├── Data/AppDbContext.cs         # EF Core DbContext
│   ├── Services/Services.cs        # Business logic (ITaskService, IUserService)
│   ├── Views/                      # Razor views
│   ├── Program.cs                  # App entry point
│   ├── appsettings.json            # Configuration
│   └── TaskManager.csproj
├── scripts/database.sql            # Manual DB setup script
├── Dockerfile                      # Multi-stage Docker build
├── docker-compose.yml              # App + SQL Server orchestration
├── LabReport_TaskManager.docx      # Full lab report
└── README.md
```

## Without Docker (local dev)
1. Install .NET 8 SDK and SQL Server
2. Update appsettings.json connection string
3. `cd src && dotnet run`

## REST API
- GET    /api/ApiTasks
- GET    /api/ApiTasks/{id}
- POST   /api/ApiTasks
- PUT    /api/ApiTasks/{id}
- DELETE /api/ApiTasks/{id}
