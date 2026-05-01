using FitnessApp.Nutrition.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessApp.Nutrition.Infrastructure.Persistence.Configurations
{
    public class MealLogConfiguration : IEntityTypeConfiguration<MealLog>
    {
        public void Configure(EntityTypeBuilder<MealLog> builder)
        {
            builder.ToTable("MealLogs");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .HasColumnName("Id")
                .ValueGeneratedNever();

            builder.Property(m => m.UserId)
                .HasColumnName("UserId")
                .IsRequired();

            builder.Property(m => m.MealName)
                .HasColumnName("MealName")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(m => m.MealType)
                .HasColumnName("MealType")
                .HasConversion<int>()
                .IsRequired();

            builder.Property(m => m.LoggedAt)
                .HasColumnName("LoggedAt")
                .IsRequired();

            builder.OwnsOne(m => m.Nutrition, n =>
            {
                n.Property(v => v.Calories).HasColumnName("Calories").HasPrecision(8, 2).IsRequired();
                n.Property(v => v.Proteins).HasColumnName("Proteins").HasPrecision(8, 2).IsRequired();
                n.Property(v => v.Fats).HasColumnName("Fats").HasPrecision(8, 2).IsRequired();
                n.Property(v => v.Carbs).HasColumnName("Carbs").HasPrecision(8, 2).IsRequired();
            });

            builder.HasIndex(m => new { m.UserId, m.LoggedAt });

            builder.Ignore(m => m.DomainEvents);
        }
    }
}
