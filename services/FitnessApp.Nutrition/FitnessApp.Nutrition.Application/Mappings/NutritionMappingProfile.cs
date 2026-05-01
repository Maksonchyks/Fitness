using AutoMapper;
using FitnessApp.Nutrition.Application.DTOs;
using FitnessApp.Nutrition.Domain.Entities;
using FitnessApp.Nutrition.Domain.ValueObjects;

namespace FitnessApp.Nutrition.Application.Mappings
{
    public class NutritionMappingProfile : Profile
    {
        public NutritionMappingProfile()
        {
            CreateMap<NutritionValue, NutritionValueDto>();

            CreateMap<MealLog, MealLogResponse>()
                .ForCtorParam("Nutrition", opt => opt.MapFrom(src => src.Nutrition));

            CreateMap<DailyTarget, DailyTargetResponse>()
                .ForCtorParam("Target", opt => opt.MapFrom(src => src.Target));

            CreateMap<WeightLog, WeightLogResponse>();

            CreateMap<TemplateMeal, TemplateMealResponse>()
                .ForCtorParam("Nutrition", opt => opt.MapFrom(src => src.Nutrition));
        }
    }
}
