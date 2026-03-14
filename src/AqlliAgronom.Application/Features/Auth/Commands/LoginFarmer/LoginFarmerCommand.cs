using AqlliAgronom.Application.Features.Auth.DTOs;
using MediatR;

namespace AqlliAgronom.Application.Features.Auth.Commands.LoginFarmer;

public record LoginFarmerCommand(string Phone, string Password) : IRequest<AuthResponseDto>;
