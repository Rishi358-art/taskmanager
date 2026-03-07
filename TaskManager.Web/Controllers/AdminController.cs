using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Interfaces;

namespace TaskManager.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IUserRepository _userRepo;

    public AdminController(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<IActionResult> Users()
    {
        var users = await _userRepo.GetAllAsync();
        return View(users);
    }
}