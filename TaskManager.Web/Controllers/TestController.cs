using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Web.Controllers;

public class TestController : Controller
{
    private readonly ICacheService _cache;

    public TestController(ICacheService cache)
    {
        _cache = cache;
    }

    public async Task<IActionResult> Index()
    {
        await _cache.SetAsync("test", "hello", TimeSpan.FromMinutes(5));
        var value = await _cache.GetAsync<string>("test");

        return Content(value ?? "no value");
    }
}