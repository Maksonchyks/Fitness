using AutoMapper;
using FitnessApp.Nutrition.Application.DTOs;
using FitnessApp.Nutrition.Application.Interfaces;
using FitnessApp.Nutrition.Domain.Entities;
using FitnessApp.Nutrition.Domain.Interfaces.Persistence;
using MediatR;

namespace FitnessApp.Nutrition.Application.Features.Queries.GetNextTemplate
{
    public class GetNextTemplateHandler : IRequestHandler<GetNextTemplateQuery, MealPlanTemplateResponse?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetNextTemplateHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<MealPlanTemplateResponse?> Handle(GetNextTemplateQuery request, CancellationToken ct)
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedAccessException();

            var userId = _currentUserService.UserId!.Value;

            // Get or create queue for this user
            var queue = await _unitOfWork.TemplateQueues.GetByUserIdAsync(userId, ct);
            if (queue == null)
            {
                queue = UserTemplateQueue.Create(userId);
                await _unitOfWork.TemplateQueues.AddAsync(queue, ct);
            }

            // Get all templates
            var allTemplates = (await _unitOfWork.Templates.GetAllAsync(ct)).ToList();
            if (allTemplates.Count == 0)
                return null;

            // Filter out viewed templates
            var available = allTemplates
                .Where(t => !queue.IsViewed(t.Id))
                .ToList();

            // Auto-reset if all templates have been viewed
            if (available.Count == 0)
            {
                queue.Reset();
                available = allTemplates;
            }

            // Pick random template from available
            var random = new Random();
            var selected = available[random.Next(available.Count)];

            // Mark as viewed
            queue.MarkAsViewed(selected.Id);
            await _unitOfWork.SaveChangesAsync(ct);

            // Map to response
            var totalNutrition = _mapper.Map<NutritionValueDto>(selected.TotalNutrition);
            var meals = selected.Meals
                .Select(m => _mapper.Map<TemplateMealResponse>(m))
                .ToList();

            var remaining = queue.GetRemainingCount(allTemplates.Count);

            return new MealPlanTemplateResponse(
                Id: selected.Id,
                Name: selected.Name,
                Description: selected.Description,
                TotalNutrition: totalNutrition,
                Meals: meals,
                RemainingTemplates: remaining
            );
        }
    }
}
