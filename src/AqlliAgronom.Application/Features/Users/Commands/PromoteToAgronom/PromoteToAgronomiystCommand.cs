using MediatR;

namespace AqlliAgronom.Application.Features.Users.Commands.PromoteToAgronom;

/// <summary>Returns promoted user's FullName on success.</summary>
public record PromoteToAgronomiystCommand(string Phone) : IRequest<string>;
