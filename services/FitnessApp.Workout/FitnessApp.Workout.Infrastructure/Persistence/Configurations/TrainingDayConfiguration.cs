using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitnessApp.Workout.Domain.Entities;
using FitnessApp.Workout.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Workout.Infrastructure.Persistence.Configurations
{
    public class TrainingDayConfiguration : IEntityTypeConfiguration<TrainingDay>
    {
        public void Configure(EntityTypeBuilder<TrainingDay> builder)
        {
            builder.ToTable("TrainingDays");

            builder.HasKey(td => td.Id);
            builder.Property(td => td.Id)
                .ValueGeneratedNever();

            builder.Property(td => td.TrainingProgramId)
                .IsRequired();

            builder.Property(td => td.DayNumber)
                .IsRequired();

            builder.HasOne(td => td.TrainingProgram)
                .WithMany(p => p.TrainingDays)
                .HasForeignKey(td => td.TrainingProgramId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.OwnsMany(td => td.Exercises, exerciseBuilder =>
            {
                exerciseBuilder.ToTable("PlannedExercises");

                exerciseBuilder.Property<Guid>("Id")
                    .ValueGeneratedOnAdd();

                exerciseBuilder.HasKey("Id");

                exerciseBuilder.WithOwner()
                    .HasForeignKey("TrainingDayId");

                exerciseBuilder.Property(e => e.ExerciseType)
                    .HasConversion<int>()
                    .IsRequired();

                exerciseBuilder.Property(e => e.Weight)
                    .HasPrecision(5, 2)
                    .IsRequired();

                exerciseBuilder.Property(e => e.Reps)
                    .IsRequired();

                exerciseBuilder.Property(e => e.Sets)
                    .IsRequired();

                exerciseBuilder.HasIndex("TrainingDayId");
            });

            builder.Navigation(td => td.Exercises)
                .AutoInclude();
        }
    }
}
