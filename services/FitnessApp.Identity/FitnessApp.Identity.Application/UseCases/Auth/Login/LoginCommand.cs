using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Application.DTOs;
using MediatR;

namespace FitnessApp.Identity.Application.UseCases.Auth.Login
{
    public record LoginCommand : IRequest<AuthResponse>
    {
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string? IpAddress { get; init; }
    }
}
