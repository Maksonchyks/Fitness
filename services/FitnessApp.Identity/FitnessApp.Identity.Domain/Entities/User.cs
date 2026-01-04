using FitnessApp.Identity.Domain.Common;
using FitnessApp.Identity.Domain.Enums;
using FitnessApp.Identity.Domain.Events;
using FitnessApp.Identity.Domain.Exceptions;
using FitnessApp.Identity.Domain.ValueObjects;
using System.Data;

namespace FitnessApp.Identity.Domain.Entities;

public class User : AggregateRoot
{
    // Основні властивості
    public Email Email { get; private set; }
    public string Username { get; private set; }
    public Password Password { get; private set; }
    public AccountStatus Status { get; private set; }

    // Профіль
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public Gender? Gender { get; private set; }
    public FitnessGoal? FitnessGoal { get; private set; }
    public string? ProfileImageUrl { get; private set; }

    // Метадані
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public bool EmailConfirmed { get; private set; }

    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    private User() : base()
    {
        Email = Email.Create("temp@email.com");
        Username = string.Empty;
        Password = Password.FromHash("temp", "temp");
        Status = AccountStatus.Active;
    }

    public static User Create(
        Email email,
        string username,
        Password password,
        string? firstName = null,
        string? lastName = null)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new DomainException("Username cannot be empty");

        if (username.Length < 2 || username.Length > 50)
            throw new DomainException("Username must be between 3 and 50 characters");

        var user = new User
        {
            Email = email ?? throw new ArgumentNullException(nameof(email)),
            Username = username.ToLower(),
            Password = password ?? throw new ArgumentNullException(nameof(password)),
            Status = AccountStatus.Active,
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = DateTime.UtcNow,
            EmailConfirmed = false
        };

        // Додаємо доменну подію
        user.AddDomainEvent(new UserRegisteredDomainEvent(
            user.Id,
            email.Value,
            username));

        return user;
    }

    // Додати роль користувачеві
    public void AddRole(Role role)
    {
        if (_userRoles.Any(ur => ur.RoleId == role.Id))
            throw new DomainException($"User already has role: {role.Name}");

        var userRole = new UserRole(this, role);
        _userRoles.Add(userRole);
    }

    // Видалити роль
    public void RemoveRole(Role role)
    {
        var userRole = _userRoles.FirstOrDefault(ur => ur.RoleId == role.Id);
        if (userRole != null)
            _userRoles.Remove(userRole);
    }

    // Перевірити, чи має користувач роль
    public bool HasRole(string roleName)
    {
        return _userRoles.Any(ur =>
            ur.Role.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
    }

    // Оновити профіль
    public void UpdateProfile(
        string? firstName,
        string? lastName,
        DateTime? dateOfBirth,
        Gender? gender,
        FitnessGoal? fitnessGoal,
        string? profileImageUrl = null)
    {
        if (dateOfBirth.HasValue)
        {
            Guard.AgainstFutureDate(dateOfBirth.Value, nameof(dateOfBirth));
            Guard.AgainstPastDate(dateOfBirth.Value, nameof(dateOfBirth));

            // Перевірка, що користувачу не менше 16 років
            var age = DateTime.UtcNow.Year - dateOfBirth.Value.Year;
            if (DateTime.UtcNow < dateOfBirth.Value.AddYears(age)) age--;

            if (age < 16)
                throw new DomainException("User must be at least 16 years old");
        }

        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        FitnessGoal = fitnessGoal;
        ProfileImageUrl = profileImageUrl;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserProfileUpdatedDomainEvent(
            Id,
            firstName,
            lastName,
            dateOfBirth ?? DateTime.MinValue,
            gender ?? Enums.Gender.PreferNotToSay,
            fitnessGoal ?? Enums.FitnessGoal.Fitness));
    }

    // Змінити пароль
    public void ChangePassword(Password newPassword)
    {
        Password = newPassword;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserPasswordChangedDomainEvent(Id, DateTime.UtcNow));
    }

    // Підтвердити email
    public void ConfirmEmail()
    {
        if (EmailConfirmed)
            throw new DomainException("Email is already confirmed");

        EmailConfirmed = true;
        UpdatedAt = DateTime.UtcNow;
    }

    // Деактивувати акаунт
    public void Deactivate()
    {
        if (Status == AccountStatus.Inactive)
            throw new DomainException("Account is already inactive");

        Status = AccountStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    // Активувати акаунт
    public void Activate()
    {
        if (Status == AccountStatus.Active)
            throw new DomainException("Account is already active");

        Status = AccountStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    // Призупинити акаунт
    public void Suspend()
    {
        Status = AccountStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;
    }

    // Додати refresh token
    public void AddRefreshToken(RefreshToken refreshToken)
    {
        _refreshTokens.Add(refreshToken);
    }

    // Оновити час останнього входу
    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    // Отримати повне ім'я
    public string GetFullName()
    {
        if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
            return $"{FirstName} {LastName}";

        return Username;
    }

    // Отримати вік
    public int? GetAge()
    {
        if (!DateOfBirth.HasValue)
            return null;

        var today = DateTime.UtcNow;
        var age = today.Year - DateOfBirth.Value.Year;

        if (today.Date < DateOfBirth.Value.AddYears(age))
            age--;

        return age;
    }
}