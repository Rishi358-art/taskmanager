using Microsoft.EntityFrameworkCore;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Application.Interfaces;
using TaskManager.Infrastructure.Repositories;
using TaskManager.Application.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using TaskManager.Application.Security;
using TaskManager.Infrastructure.Security;
using TaskManager.Infrastructure.Services;
using Microsoft.AspNetCore.RateLimiting;
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);
Console.WriteLine("ENV RAW DB: " + Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection"));
Console.WriteLine("CONFIG DB: " + builder.Configuration.GetConnectionString("DefaultConnection"));
Console.WriteLine("ENV RAW REDIS: " + Environment.GetEnvironmentVariable("Redis__ConnectionString"));
Console.WriteLine("CONFIG REDIS: " + builder.Configuration["Redis:ConnectionString"]);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));
    Console.WriteLine(builder.Configuration["Redis:ConnectionString"]);
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
});
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 20; // requests
        opt.Window = TimeSpan.FromSeconds(10);
        opt.QueueLimit = 0;
    });
});
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskCommentRepository, TaskCommentRepository>();
builder.Services.AddScoped<ITaskCommentService, TaskCommentService>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler("/Home/Error");
app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
