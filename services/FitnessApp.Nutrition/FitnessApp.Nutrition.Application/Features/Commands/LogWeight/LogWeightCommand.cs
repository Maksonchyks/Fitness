using FitnessApp.Nutrition.Application.DTOs;
using MediatR;

namespace FitnessApp.Nutrition.Application.Features.Commands.LogWeight
{
    public record LogWeightCommand(float Weight) : IRequest<WeightLogResponse>;
}
