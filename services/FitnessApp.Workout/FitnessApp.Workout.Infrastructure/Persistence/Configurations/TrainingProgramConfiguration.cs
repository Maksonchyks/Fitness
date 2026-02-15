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
    public class TrainingProgramConfiguration : IEntityTypeConfiguration<TrainingProgram>
    {
        public void Configure(EntityTypeBuilder<TrainingProgram> builder)
        {
            builder.ToTable("TrainingPrograms");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("Id")
                .ValueGeneratedNever();

            builder.Property(p => p.UserId)
                .HasColumnName("UserId")
                .IsRequired();

            builder.Property(p => p.CreatedOn)
                .HasColumnName("CreatedOn")
                .IsRequired()
                .HasDefaultValueSql("timezone('utc', now())");

            builder.OwnsOne(p => p.ProgramProfile, navigationBuilder =>
            {
                navigationBuilder.Property(pp => pp.Goal)
                    .HasColumnName("Goal")
                    .HasConversion<int>()
                    .IsRequired();

                navigationBuilder.Property(pp => pp.Intensity)
                    .HasColumnName("Intensity")
                    .HasConversion<int>()
                    .IsRequired();

                navigationBuilder.OwnsOne(pp => pp.PowerMetrics, metricsBuilder =>
                {
                    metricsBuilder.Property(pm => pm.SquatWeight)
                        .HasColumnName("PowerMetrics_SquatWeight")
                        .HasPrecision(5, 2)
                        .IsRequired(false);

                    metricsBuilder.Property(pm => pm.BenchPressWeight)
                        .HasColumnName("PowerMetrics_BenchPressWeight")
                        .HasPrecision(5, 2)
                        .IsRequired(false);

                    metricsBuilder.Property(pm => pm.DeadliftWeight)
                        .HasColumnName("PowerMetrics_DeadliftWeight")
                        .HasPrecision(5, 2)
                        .IsRequired(false);
                });
            });

            builder.HasMany(p => p.TrainingDays)
                .WithOne(d => d.TrainingProgram)
                .HasForeignKey(d => d.TrainingProgramId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(p => p.TrainingDays)
                .AutoInclude();

            builder.Ignore(p => p.DomainEvents);
        }
    }
}
