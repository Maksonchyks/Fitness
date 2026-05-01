using FitnessApp.Nutrition.Application.DTOs;
using MediatR;

namespace FitnessApp.Nutrition.Application.Features.Queries.GetDailyProgress
{
    public record GetDailyProgressQuery(DateTime? Date = null) : IRequest<DailyProgressResponse>;
}
