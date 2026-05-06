using Microsoft.AspNetCore.Mvc;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.Controllers
{
    // ─── Home / Dashboard ────────────────────────────────────────────────────
    public class HomeController : Controller
    {
        private readonly ITaskService _taskSvc;
        public HomeController(ITaskService taskSvc) => _taskSvc = taskSvc;

        public async Task<IActionResult> Index()
        {
            var uid = HttpContext.Session.GetInt32("UserId");
            if (uid == null) return RedirectToAction("Login", "Account");
            var vm = await _taskSvc.GetDashboardDataAsync(uid.Value);
            return View(vm);
        }
    }

    // ─── Account (Login / Register) ──────────────────────────────────────────
    public class AccountController : Controller
    {
        private readonly IUserService _userSvc;
        public AccountController(IUserService userSvc) => _userSvc = userSvc;

        [HttpGet] public IActionResult Login()   => View();
        [HttpGet] public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userSvc.AuthenticateAsync(model.Email, model.Password);
            if (user == null) { ModelState.AddModelError("", "Invalid credentials."); return View(model); }
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userSvc.RegisterAsync(model);
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }

    // ─── Tasks CRUD ──────────────────────────────────────────────────────────
    public class TasksController : Controller
    {
        private readonly ITaskService _taskSvc;
        private readonly IUserService _userSvc;
        public TasksController(ITaskService ts, IUserService us) { _taskSvc = ts; _userSvc = us; }

        private IActionResult? RequireAuth()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");
            return null;
        }

        public async Task<IActionResult> Index(int? projectId)
        {
            var r = RequireAuth(); if (r != null) return r;
            return View(await _taskSvc.GetAllAsync(projectId));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var r = RequireAuth(); if (r != null) return r;
            ViewBag.Users = await _userSvc.GetAllUsersAsync();
            return View(new ProjectTask());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProjectTask task)
        {
            if (!ModelState.IsValid) { ViewBag.Users = await _userSvc.GetAllUsersAsync(); return View(task); }
            await _taskSvc.CreateAsync(task);
            TempData["Success"] = "Task created successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var r = RequireAuth(); if (r != null) return r;
            var task = await _taskSvc.GetByIdAsync(id);
            if (task == null) return NotFound();
            ViewBag.Users = await _userSvc.GetAllUsersAsync();
            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProjectTask task)
        {
            if (!ModelState.IsValid) { ViewBag.Users = await _userSvc.GetAllUsersAsync(); return View(task); }
            await _taskSvc.UpdateAsync(task);
            TempData["Success"] = "Task updated successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _taskSvc.DeleteAsync(id);
            TempData["Success"] = "Task deleted.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var task = await _taskSvc.GetByIdAsync(id);
            if (task == null) return NotFound();
            return View(task);
        }
    }
}
