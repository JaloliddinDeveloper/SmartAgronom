using System.Text.RegularExpressions;
using AqlliAgronom.Domain.Common;

namespace AqlliAgronom.Domain.ValueObjects;

public sealed class PhoneNumber : IEquatable<PhoneNumber>
{
    private static readonly Regex PhoneRegex =
        new(@"^\+?[1-9]\d{9,14}$", RegexOptions.Compiled);

    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    public static PhoneNumber Create(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new DomainException("PHONE_REQUIRED", "Phone number is required.");

        var normalized = raw.Trim().Replace(" ", "").Replace("-", "");
        if (!normalized.StartsWith("+")) normalized = "+" + normalized;

        if (!PhoneRegex.IsMatch(normalized))
            throw new DomainException("PHONE_INVALID", $"Phone number '{raw}' is invalid.");

        return new PhoneNumber(normalized);
    }

    public bool Equals(PhoneNumber? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is PhoneNumber p && Equals(p);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phone) => phone.Value;
}
