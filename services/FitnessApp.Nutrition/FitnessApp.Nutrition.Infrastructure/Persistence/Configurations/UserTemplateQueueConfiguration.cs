using FitnessApp.Nutrition.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessApp.Nutrition.Infrastructure.Persistence.Configurations
{
    public class UserTemplateQueueConfiguration : IEntityTypeConfiguration<UserTemplateQueue>
    {
        public void Configure(EntityTypeBuilder<UserTemplateQueue> builder)
        {
            builder.ToTable("UserTemplateQueues");
            builder.HasKey(q => q.Id);

            builder.Property(q => q.Id)
                .HasColumnName("Id")
                .ValueGeneratedNever();

            builder.Property(q => q.UserId)
                .HasColumnName("UserId")
                .IsRequired();

            builder.Property(q => q.LastResetAt)
                .HasColumnName("LastResetAt")
                .IsRequired();

            // Store viewed template IDs as JSON column
            builder.Property(q => q.ViewedTemplateIds)
                .HasColumnName("ViewedTemplateIds")
                .HasColumnType("jsonb");

            builder.HasIndex(q => q.UserId).IsUnique();

            builder.Ignore(q => q.DomainEvents);
        }
    }
}
