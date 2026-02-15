using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Workout.Infrastructure.Persistence.Configurations
{
    public class WorkoutSessionConfiguration : IEntityTypeConfiguration<WorkoutSession>
    {
        public void Configure(EntityTypeBuilder<WorkoutSession> builder)
        {
            builder.ToTable("WorkoutSessions");

            builder.HasKey(ws => ws.Id);

            builder.Property(ws => ws.UserId)
                .IsRequired();

            builder.Property(ws => ws.TrainingDayId)
                .IsRequired();

            builder.Property(ws => ws.Date)
                .IsRequired()
                .HasDefaultValueSql("timezone('utc', now())");

            
            builder.OwnsMany(ws => ws.PerformedExercises, eb =>
            {
                eb.ToTable("PerformedExercises");

                eb.WithOwner().HasForeignKey("WorkoutSessionId");

                eb.Property<Guid>("Id");
                eb.HasKey("Id");

                eb.Property(e => e.CreatedOn)
                .IsRequired()
                .HasDefaultValueSql("timezone('utc', now())");

                eb.Property(e => e.ExerciseType).IsRequired();

                eb.Property(e => e.Weight).HasPrecision(5, 2);
                eb.Property(e => e.Reps).IsRequired();
                eb.Property(e => e.Sets).IsRequired();
            });

            builder.Ignore(ws => ws.DomainEvents);
        }
    }
}
