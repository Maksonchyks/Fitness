using MediatR;

namespace FitnessApp.Nutrition.Application.Features.Commands.DeleteMealLog
{
    public record DeleteMealLogCommand(Guid MealLogId) : IRequest<bool>;
}
