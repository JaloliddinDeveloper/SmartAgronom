using AqlliAgronom.Application.Features.Auth.DTOs;
using AqlliAgronom.Domain.Enums;
using MediatR;

namespace AqlliAgronom.Application.Features.Auth.Commands.RegisterFarmer;

public record RegisterFarmerCommand(
    string FullName,
    string Phone,
    string Password,
    Language Language = Language.Uzbek,
    string? Email = null,
    string? Region = null) : IRequest<AuthResponseDto>;
