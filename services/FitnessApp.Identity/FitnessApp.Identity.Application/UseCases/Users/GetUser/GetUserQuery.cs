using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Application.DTOs;
using MediatR;

namespace FitnessApp.Identity.Application.UseCases.Users.GetUser
{
    public record GetUserQuery : IRequest<UserResponse>
    {
        public Guid UserId { get; init; }
    }
}
