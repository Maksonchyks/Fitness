using FitnessApp.Nutrition.Application.DTOs;
using MediatR;

namespace FitnessApp.Nutrition.Application.Features.Queries.GetWeightHistory
{
    public record GetWeightHistoryQuery(int Days = 30) : IRequest<List<WeightLogResponse>>;
}
