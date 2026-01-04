using FitnessApp.Identity.Domain.Entities;

public class UserRole : Entity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTime AssignedAt { get; private set; }

    // Навігаційні властивості (nullable для EF Core)
    public User? User { get; private set; }
    public Role? Role { get; private set; }

    // Приватний конструктор для EF Core
    private UserRole() : base() { }

    public UserRole(User user, Role role) : base()
    {
        UserId = user.Id;
        RoleId = role.Id;
        User = user ?? throw new ArgumentNullException(nameof(user));
        Role = role ?? throw new ArgumentNullException(nameof(role));
        AssignedAt = DateTime.UtcNow;
    }
}