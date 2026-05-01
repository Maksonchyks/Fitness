using FitnessApp.Nutrition.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessApp.Nutrition.Infrastructure.Persistence.Configurations
{
    public class DailyTargetConfiguration : IEntityTypeConfiguration<DailyTarget>
    {
        public void Configure(EntityTypeBuilder<DailyTarget> builder)
        {
            builder.ToTable("DailyTargets");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id)
                .HasColumnName("Id")
                .ValueGeneratedNever();

            builder.Property(d => d.UserId)
                .HasColumnName("UserId")
                .IsRequired();

            builder.Property(d => d.Goal)
                .HasColumnName("Goal")
                .HasConversion<int>()
                .IsRequired();

            builder.Property(d => d.CreatedAt)
                .HasColumnName("CreatedAt")
                .IsRequired();

            builder.Property(d => d.UpdatedAt)
                .HasColumnName("UpdatedAt");

            builder.OwnsOne(d => d.Target, n =>
            {
                n.Property(v => v.Calories).HasColumnName("TargetCalories").HasPrecision(8, 2).IsRequired();
                n.Property(v => v.Proteins).HasColumnName("TargetProteins").HasPrecision(8, 2).IsRequired();
                n.Property(v => v.Fats).HasColumnName("TargetFats").HasPrecision(8, 2).IsRequired();
                n.Property(v => v.Carbs).HasColumnName("TargetCarbs").HasPrecision(8, 2).IsRequired();
            });

            builder.HasIndex(d => d.UserId).IsUnique();

            builder.Ignore(d => d.DomainEvents);
        }
    }
}
