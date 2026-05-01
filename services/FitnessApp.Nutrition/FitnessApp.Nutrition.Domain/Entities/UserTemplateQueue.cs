using FitnessApp.Nutrition.Domain.Common;
using FitnessApp.Nutrition.Domain.Exceptions;

namespace FitnessApp.Nutrition.Domain.Entities
{
    public class UserTemplateQueue : AggregateRoot
    {
        public Guid UserId { get; private set; }
        
        public List<Guid> ViewedTemplateIds { get; private set; } = new();
        
        public DateTime LastResetAt { get; private set; }

        protected UserTemplateQueue() { }

        private UserTemplateQueue(Guid userId) : base()
        {
            UserId = userId;
            LastResetAt = DateTime.UtcNow;
        }

        public static UserTemplateQueue Create(Guid userId)
        {
            Guard.AgainstEmptyGuid(userId, nameof(UserId));
            return new UserTemplateQueue(userId);
        }

        public void MarkAsViewed(Guid templateId)
        {
            if (templateId == Guid.Empty)
                throw new DomainException("TemplateId cannot be empty");

            if (!ViewedTemplateIds.Contains(templateId))
            {
                ViewedTemplateIds.Add(templateId);
            }
        }

        public bool IsViewed(Guid templateId)
        {
            return ViewedTemplateIds.Contains(templateId);
        }

        public int GetRemainingCount(int totalTemplates)
        {
            return totalTemplates - ViewedTemplateIds.Count;
        }

        public void Reset()
        {
            ViewedTemplateIds.Clear();
            LastResetAt = DateTime.UtcNow;
        }
    }
}
