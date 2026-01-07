using FitnessApp.Identity.Domain.Entities;

public class Role : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsDefault { get; private set; }

    // Навігаційна властивість
    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private Role() : base()
    {
        Name = string.Empty;
        Description = string.Empty;
    }

    public Role(string name, string description, bool isDefault = false) : base()
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        IsDefault = isDefault;
    }

    public void Update(string name, string description)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
    }

    public void MarkAsDefault() => IsDefault = true;
    public void RemoveDefault() => IsDefault = false;
}