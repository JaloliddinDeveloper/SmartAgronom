using AqlliAgronom.Domain.Common;
using AqlliAgronom.Domain.Enums;
using AqlliAgronom.Domain.Events.Users;
using AqlliAgronom.Domain.ValueObjects;

namespace AqlliAgronom.Domain.Entities;

public class User : AggregateRoot
{
    public string FullName { get; private set; } = default!;
    public PhoneNumber Phone { get; private set; } = default!;
    public string? Email { get; private set; }
    public string PasswordHash { get; private set; } = default!;
    public UserRole Role { get; private set; }
    public Language PreferredLanguage { get; private set; }
    public long? TelegramChatId { get; private set; }
    public string? TelegramUsername { get; private set; }
    public string? Region { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime? LastLoginAt { get; private set; }

    private User() { }

    public static User Register(
        string fullName,
        string phone,
        string passwordHash,
        Language language = Language.Uzbek,
        string? email = null,
        string? region = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("USER_NAME_REQUIRED", "Full name is required.");

        var user = new User
        {
            FullName = fullName.Trim(),
            Phone = PhoneNumber.Create(phone),
            PasswordHash = passwordHash,
            PreferredLanguage = language,
            Email = email?.Trim().ToLowerInvariant(),
            Region = region,
            Role = UserRole.Farmer
        };

        user.AddDomainEvent(new UserRegisteredEvent(user.Id, user.FullName, user.Phone));
        return user;
    }

    public void UpdateProfile(string fullName, Language language, string? email, string? region)
    {
        FullName = fullName.Trim();
        PreferredLanguage = language;
        Email = email?.Trim().ToLowerInvariant();
        Region = region;
        SetUpdatedAt();
        AddDomainEvent(new UserProfileUpdatedEvent(Id));
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new DomainException("PASSWORD_REQUIRED", "Password hash is required.");
        PasswordHash = newPasswordHash;
        SetUpdatedAt();
    }

    public void AssignRole(UserRole role)
    {
        Role = role;
        SetUpdatedAt();
    }

    public void LinkTelegram(long chatId, string? username)
    {
        TelegramChatId = chatId;
        TelegramUsername = username;
        SetUpdatedAt();
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }
}
