using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Workout.Application.DTOs;
using FitnessApp.Workout.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FitnessApp.Workout.Application.Mappings
{
    public class TrainingProgramMappingProfile : Profile
    {
        public TrainingProgramMappingProfile()
        {
            CreateMap<TrainingProgram, TrainingProgramResponse>();

            CreateMap<TrainingDay, TrainingDayDto>();

            CreateMap<ExerciseSet, ExerciseSetDto>();
        }
    }
}
