namespace AqlliAgronom.Application.Features.MathGame.DTOs;

public record MathScoreDto(
    Guid Id,
    string PlayerName,
    int Score,
    int CorrectAnswers,
    int BestStreak,
    int LevelReached,
    string Difficulty,
    DateTime CreatedAt);
