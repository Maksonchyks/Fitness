using FitnessApp.Identity.Domain.Exceptions;

namespace FitnessApp.Identity.Domain.Entities;

public class RefreshToken : Entity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedByIp { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevokedByIp { get; private set; }
    public string? ReplacedByToken { get; private set; }
    public string? ReasonRevoked { get; private set; }

    public User? User { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsRevoked && !IsExpired;

    private RefreshToken()
    {
        Token = string.Empty;
        CreatedByIp = string.Empty;
    }

    public RefreshToken(
        Guid userId,
        string token,
        DateTime expiresAt,
        string createdByIp) : base()
    {
        UserId = userId;
        Token = token ?? throw new ArgumentNullException(nameof(token));
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
        CreatedByIp = createdByIp ?? throw new ArgumentNullException(nameof(createdByIp));
    }

    // Фабричний метод для створення
    public static RefreshToken Create(
        Guid userId,
        string token,
        int expirationDays,
        string ipAddress)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new DomainException("Token cannot be empty");

        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new DomainException("IP address cannot be empty");

        return new RefreshToken(
            userId,
            token,
            DateTime.UtcNow.AddDays(expirationDays),
            ipAddress);
    }

    // Відкликати токен
    public void Revoke(string revokedByIp, string? replacedByToken = null, string reason = "Replaced by new token")
    {
        if (IsRevoked)
            throw new DomainException("Token is already revoked");

        if (IsExpired)
            throw new DomainException("Token is already expired");

        if (string.IsNullOrWhiteSpace(revokedByIp))
            throw new DomainException("IP address cannot be empty");

        RevokedAt = DateTime.UtcNow;
        RevokedByIp = revokedByIp;
        ReplacedByToken = replacedByToken;
        ReasonRevoked = reason;
    }

    // Продовжити життя токена
    public void Extend(int additionalDays)
    {
        if (IsRevoked || IsExpired)
            throw new DomainException("Cannot extend revoked or expired token");

        ExpiresAt = ExpiresAt.AddDays(additionalDays);
    }
}