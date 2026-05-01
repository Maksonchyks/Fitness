using FitnessApp.Nutrition.Application.DTOs;
using MediatR;

namespace FitnessApp.Nutrition.Application.Features.Queries.GetMealLogs
{
    public record GetMealLogsQuery(DateTime? Date = null) : IRequest<List<MealLogResponse>>;
}
