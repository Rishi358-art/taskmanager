using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Web.Models;

namespace TaskManager.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Tasks");
        }

        return RedirectToAction("Login", "Auth");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
   public IActionResult Error()
{
    Response.StatusCode = 500;
    return View();
}
public IActionResult StatusCode(int code)
{
    Response.StatusCode = code;

    return code switch
    {
        404 => View("NotFound"),
        403 => View("Forbidden"),
        401 => View("Unauthorized"),
        _ => View("Error")
    };
}
}
