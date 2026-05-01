using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Workout.Application.DTOs;
using FitnessApp.Workout.Domain.Entities;
using FitnessApp.Workout.Domain.ValueObjects;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FitnessApp.Workout.Application.Mappings
{
    public class TrainingProgramMappingProfile : Profile
    {
        public TrainingProgramMappingProfile()
        {
            CreateMap<TrainingProgram, TrainingProgramResponse>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedOn))
                .ForMember(dest => dest.Goal, opt => opt.MapFrom(src => src.ProgramProfile.Goal))
                .ForMember(dest => dest.Intensity, opt => opt.MapFrom(src => src.ProgramProfile.Intensity));

            CreateMap<TrainingDay, TrainingDayDto>();

            CreateMap<ExerciseSet, ExerciseSetDto>();

            CreateMap<WorkoutSession, WorkoutSessionDto>();
        }
    }
}
