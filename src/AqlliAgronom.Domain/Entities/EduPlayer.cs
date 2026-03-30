using AqlliAgronom.Domain.Common;

namespace AqlliAgronom.Domain.Entities;

public class EduPlayer : BaseEntity
{
    public string Name { get; private set; } = default!;

    private EduPlayer() { }

    public static EduPlayer Register(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("PLAYER_NAME_REQUIRED", "Player name is required.");

        return new EduPlayer
        {
            Name = name.Trim()[..Math.Min(name.Trim().Length, 50)]
        };
    }
}
