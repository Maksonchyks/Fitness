using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Identity.Domain.Enums;
using FluentValidation;

namespace FitnessApp.Identity.Application.UseCases.Users.UpdateProfile
{
    public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User Id is required");

            RuleFor(x => x.FirstName).MaximumLength(50);
            RuleFor(x => x.LastName).MaximumLength(50);

            RuleFor(x => x.DateOfBirth)
                .Must(date => date == null || date < DateTime.UtcNow)
                .WithMessage("Date of birth cannot be in the future");
        }
    }
}
