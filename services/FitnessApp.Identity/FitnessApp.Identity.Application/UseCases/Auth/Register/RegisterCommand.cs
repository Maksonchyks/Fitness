using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Application.DTOs;
using MediatR;

namespace FitnessApp.Identity.Application.UseCases.Auth.Register
{
    public record RegisterCommand : IRequest<AuthResponse>
    {
        public string Email { get; init; } = string.Empty;
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string ConfirmPassword { get; init; } = string.Empty;
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public DateTime? DateOfBirth { get; init; }
        public string? Gender { get; init; }
        public string? FitnessGoal { get; init; }
        public string? IpAddress { get; init; }
    }
}
