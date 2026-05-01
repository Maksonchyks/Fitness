using MediatR;

namespace FitnessApp.Nutrition.Application.Features.Commands.ResetTemplateQueue
{
    public record ResetTemplateQueueCommand() : IRequest<bool>;
}
