using TaskManager.Domain.Entities;

public interface IAuditRepository
{
    Task AddAsync(AuditLog log);
    Task<List<AuditLog>> GetAllAsync();
}