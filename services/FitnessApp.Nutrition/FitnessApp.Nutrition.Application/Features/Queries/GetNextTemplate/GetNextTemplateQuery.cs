using FitnessApp.Nutrition.Application.DTOs;
using MediatR;

namespace FitnessApp.Nutrition.Application.Features.Queries.GetNextTemplate
{
    public record GetNextTemplateQuery() : IRequest<MealPlanTemplateResponse?>;
}
