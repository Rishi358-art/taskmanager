using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "Admin")]

public class AuditController : Controller
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    public async Task<IActionResult> Index()
    {
        var logs = await _auditService.GetAllAsync();
        return View(logs);
    }
}