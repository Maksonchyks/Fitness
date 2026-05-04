using FitnessApp.Communication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Communication.Infrastructure.Persistence
{
    public class CommunicationDbContext : DbContext
    {
        public DbSet<ChatRoom> ChatRooms => Set<ChatRoom>();
        public DbSet<Participant> Participants => Set<Participant>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
        public DbSet<UserNotificationPreference> NotificationPreferences => Set<UserNotificationPreference>();
        public DbSet<ReminderSchedule> ReminderSchedules => Set<ReminderSchedule>();
        public DbSet<UserNotification> UserNotifications => Set<UserNotification>();
        public DbSet<CommunicationUser> Users => Set<CommunicationUser>();

        public CommunicationDbContext(DbContextOptions<CommunicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Participant>()
                .HasOne(p => p.ChatRoom)
                .WithMany(r => r.Participants)
                .HasForeignKey(p => p.ChatRoomId);

            modelBuilder.Entity<Participant>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(m => m.ChatRoom)
                .WithMany(r => r.Messages)
                .HasForeignKey(m => m.ChatRoomId);

            modelBuilder.Entity<ReminderSchedule>()
                .HasOne(s => s.Preference)
                .WithMany(p => p.Schedules)
                .HasForeignKey(s => s.PreferenceId);

            modelBuilder.Entity<CommunicationUser>()
                .HasKey(u => u.Id);
        }
    }
}
