using FitnessApp.Nutrition.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessApp.Nutrition.Infrastructure.Persistence.Configurations
{
    public class TemplateMealConfiguration : IEntityTypeConfiguration<TemplateMeal>
    {
        public void Configure(EntityTypeBuilder<TemplateMeal> builder)
        {
            builder.ToTable("TemplateMeals");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .HasColumnName("Id")
                .ValueGeneratedNever();

            builder.Property(m => m.TemplateId)
                .HasColumnName("TemplateId")
                .IsRequired();

            builder.Property(m => m.MealType)
                .HasColumnName("MealType")
                .HasConversion<int>()
                .IsRequired();

            builder.Property(m => m.DishName)
                .HasColumnName("DishName")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(m => m.Description)
                .HasColumnName("Description")
                .HasMaxLength(500)
                .IsRequired();

            builder.OwnsOne(m => m.Nutrition, n =>
            {
                n.Property(v => v.Calories).HasColumnName("Calories").HasPrecision(8, 2).IsRequired();
                n.Property(v => v.Proteins).HasColumnName("Proteins").HasPrecision(8, 2).IsRequired();
                n.Property(v => v.Fats).HasColumnName("Fats").HasPrecision(8, 2).IsRequired();
                n.Property(v => v.Carbs).HasColumnName("Carbs").HasPrecision(8, 2).IsRequired();
            });

            builder.Ignore(m => m.DomainEvents);
        }
    }
}
