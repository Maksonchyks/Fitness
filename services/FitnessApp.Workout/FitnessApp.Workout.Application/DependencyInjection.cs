using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Application.Common;
using FitnessApp.Workout.Application.Interfaces.Generators;
using FitnessApp.Workout.Application.Services.Generators;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessApp.Workout.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(assembly);

            services.AddAutoMapper(assembly);

            services.AddScoped<ITrainingProgramFactory, TrainingProgramFactory>();

            services.AddScoped<ITrainingProgramGenerator, PowerliftingGenerator>();
            services.AddScoped<ITrainingProgramGenerator, BodybuildingGenerator>();
            services.AddScoped<ITrainingProgramGenerator, FitnessGenerator>();

            return services;
        }
    }
}
