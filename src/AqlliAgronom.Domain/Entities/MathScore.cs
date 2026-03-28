using AqlliAgronom.Domain.Common;

namespace AqlliAgronom.Domain.Entities;

public class MathScore : BaseEntity
{
    public string PlayerName { get; private set; } = default!;
    public int Score { get; private set; }
    public int CorrectAnswers { get; private set; }
    public int BestStreak { get; private set; }
    public int LevelReached { get; private set; }
    public string Difficulty { get; private set; } = default!;

    private MathScore() { }

    public static MathScore Create(
        string playerName,
        int score,
        int correctAnswers,
        int bestStreak,
        int levelReached,
        string difficulty)
    {
        if (string.IsNullOrWhiteSpace(playerName))
            throw new DomainException("PLAYER_NAME_REQUIRED", "Player name is required.");

        if (!new[] { "easy", "medium", "hard" }.Contains(difficulty))
            throw new DomainException("INVALID_DIFFICULTY", "Difficulty must be easy, medium, or hard.");

        return new MathScore
        {
            PlayerName   = playerName.Trim()[..Math.Min(playerName.Trim().Length, 50)],
            Score        = Math.Max(0, score),
            CorrectAnswers = Math.Max(0, correctAnswers),
            BestStreak   = Math.Max(0, bestStreak),
            LevelReached = Math.Max(1, levelReached),
            Difficulty   = difficulty,
        };
    }
}
