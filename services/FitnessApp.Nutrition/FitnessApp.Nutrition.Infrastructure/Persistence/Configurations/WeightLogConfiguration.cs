using FitnessApp.Nutrition.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessApp.Nutrition.Infrastructure.Persistence.Configurations
{
    public class WeightLogConfiguration : IEntityTypeConfiguration<WeightLog>
    {
        public void Configure(EntityTypeBuilder<WeightLog> builder)
        {
            builder.ToTable("WeightLogs");
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Id)
                .HasColumnName("Id")
                .ValueGeneratedNever();

            builder.Property(w => w.UserId)
                .HasColumnName("UserId")
                .IsRequired();

            builder.Property(w => w.Weight)
                .HasColumnName("Weight")
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(w => w.LoggedAt)
                .HasColumnName("LoggedAt")
                .IsRequired();

            builder.HasIndex(w => new { w.UserId, w.LoggedAt });

            builder.Ignore(w => w.DomainEvents);
        }
    }
}
