namespace TaskManager.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Role { get; private set; } // Admin or User
    public DateTime CreatedAt { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation property
    public ICollection<TaskItem> Tasks { get; private set; } = new List<TaskItem>();

    private User() { } // Required for EF Core

    public User(string name, string email, string passwordHash, string role)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}