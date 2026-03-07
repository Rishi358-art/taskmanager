using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Security;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Security;
using TaskManager.Web.ViewModels;

namespace TaskManager.Web.Controllers;

public class AuthController : Controller
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _hasher;
    private readonly ICacheService _cacheService;
    public AuthController(IUserRepository userRepo, IPasswordHasher hasher,ICacheService cacheService)
    {
        _userRepo = userRepo;
        _hasher = hasher;
        _cacheService = cacheService;
    }

    [HttpGet]
public IActionResult Login() => View();

[HttpPost]
public async Task<IActionResult> Login(LoginViewModel model)
{
    if (!ModelState.IsValid) return View(model);

    var user = await _userRepo.GetByEmailAsync(model.Email);
    if (user == null) return View(model);
    bool ok = _hasher.Verify(model.Password, user.PasswordHash);

Console.WriteLine(ok);
    if (!_hasher.Verify(model.Password, user.PasswordHash))
    {
        ModelState.AddModelError("", "Invalid credentials");
        return View(model);
    }
   
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.Role, user.Role)
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

    await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(identity));
    var cacheKey = $"login_{user.Id}";

var alreadyLogged = await _cacheService.GetAsync<string>(cacheKey);

if (alreadyLogged == null)
{
    TempData["Success"] = $"Welcome {user.Name}!";
    await _cacheService.SetAsync(cacheKey, "visited", TimeSpan.FromDays(30));
}
else
{
    TempData["Success"] = $"Welcome back {user.Name}!";
}

return RedirectToAction("Index", "Tasks");
}

    public IActionResult Register() => View();

    [HttpPost]
    [HttpPost]
public async Task<IActionResult> Register(RegisterViewModel model)
{
    if (!ModelState.IsValid) return View(model);

    var existing = await _userRepo.GetByEmailAsync(model.Email);
    if (existing != null)
    {
        ModelState.AddModelError("", "Email already exists");
        return View(model);
    }

    var hashed = _hasher.Hash(model.Password);
    var hashedPassword=_hasher.Hash("Admin@123");
    Console.WriteLine($"PASSWORD:{hashedPassword}");
    var user = new User(model.Name, model.Email, hashed, "User");

    await _userRepo.AddAsync(user);

    return RedirectToAction("Login");
}
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        TempData["Success"] = "Logging Out!";
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied() => View();
}