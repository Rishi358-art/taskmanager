using System.Threading.Tasks;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services;

public interface IAuditService
{
    Task LogAsync(string action, string entityName, Guid entityId);
    Task<List<AuditLog>> GetAllAsync();
}