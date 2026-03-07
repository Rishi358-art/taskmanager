using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class AuditRepository : IAuditRepository
{
    private readonly AppDbContext _context;

    public AuditRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditLog log)
    {
        await _context.AuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }
    public async Task<List<AuditLog>> GetAllAsync()
{
    return await _context.AuditLogs
        .OrderByDescending(a => a.Timestamp)
        .ToListAsync();
}
}