using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;

public class AuditService : IAuditService
{
    private readonly IAuditRepository _repo;

    public AuditService(IAuditRepository repo)
    {
        _repo = repo;
    }

    public async Task LogAsync(string action, string entityName, Guid entityId)
    {
        var log = new AuditLog(action, entityName, entityId);
        await _repo.AddAsync(log);
    }
    public async Task<List<AuditLog>> GetAllAsync()
{
    return await _repo.GetAllAsync();
}
}