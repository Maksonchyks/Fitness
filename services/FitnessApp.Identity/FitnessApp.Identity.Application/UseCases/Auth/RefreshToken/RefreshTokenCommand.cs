using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Application.DTOs;
using MediatR;

namespace FitnessApp.Identity.Application.UseCases.Auth.RefreshToken
{
    public record RefreshTokenCommand : IRequest<TokenResponse>
    {
        public string RefreshToken { get; init; } = string.Empty;
        public string? IpAddress { get; init; }
    }
}
