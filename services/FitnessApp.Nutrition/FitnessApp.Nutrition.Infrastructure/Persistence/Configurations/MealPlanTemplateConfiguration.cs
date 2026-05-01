using FitnessApp.Nutrition.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessApp.Nutrition.Infrastructure.Persistence.Configurations
{
    public class MealPlanTemplateConfiguration : IEntityTypeConfiguration<MealPlanTemplate>
    {
        public void Configure(EntityTypeBuilder<MealPlanTemplate> builder)
        {
            builder.ToTable("MealPlanTemplates");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .HasColumnName("Id")
                .ValueGeneratedNever();

            builder.Property(t => t.Name)
                .HasColumnName("Name")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(t => t.Description)
                .HasColumnName("Description")
                .HasMaxLength(500)
                .IsRequired();

            builder.OwnsOne(t => t.TotalNutrition, n =>
            {
                n.Property(v => v.Calories).HasColumnName("TotalCalories").HasPrecision(8, 2).IsRequired();
                n.Property(v => v.Proteins).HasColumnName("TotalProteins").HasPrecision(8, 2).IsRequired();
                n.Property(v => v.Fats).HasColumnName("TotalFats").HasPrecision(8, 2).IsRequired();
                n.Property(v => v.Carbs).HasColumnName("TotalCarbs").HasPrecision(8, 2).IsRequired();
            });

            builder.HasMany(t => t.Meals)
                .WithOne(m => m.Template)
                .HasForeignKey(m => m.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(t => t.Meals)
                .AutoInclude();

            builder.Ignore(t => t.DomainEvents);
        }
    }
}
