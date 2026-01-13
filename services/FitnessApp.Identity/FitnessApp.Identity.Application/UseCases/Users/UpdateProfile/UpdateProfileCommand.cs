using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Application.DTOs;
using MediatR;

namespace FitnessApp.Identity.Application.UseCases.Users.UpdateProfile
{
    public record UpdateProfileCommand : IRequest<UserResponse>
    {
        public Guid UserId { get; init; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public DateTime? DateOfBirth { get; init; }
        public string? Gender { get; init; }
        public string? FitnessGoal { get; init; }
        public string? ProfileImageUrl { get; init; }
    }
}
