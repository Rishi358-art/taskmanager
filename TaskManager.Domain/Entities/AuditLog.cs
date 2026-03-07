namespace TaskManager.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; private set; }
    public string Action { get; private set; }
    public string EntityName { get; private set; }
    public Guid EntityId { get; private set; }
    public DateTime Timestamp { get; private set; }

    private AuditLog() { }

    public AuditLog(string action, string entityName, Guid entityId)
    {
        Id = Guid.NewGuid();
        Action = action;
        EntityName = entityName;
        EntityId = entityId;
        Timestamp = DateTime.UtcNow;
    }
}