using Microsoft.EntityFrameworkCore;
using WhatsAppMessenger.Models;

namespace WhatsAppMessenger.Data
{
    /// <summary>
    /// DbContext de Entity Framework Core para SQL Server
    /// </summary>
    public class MessengerDbContext : DbContext
    {
        public MessengerDbContext(DbContextOptions<MessengerDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la tabla Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnType("NVARCHAR(255)");

                entity.Property(e => e.DisplayName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnType("NVARCHAR(255)");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .HasColumnType("NVARCHAR(20)");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasName("IX_Users_Email");
            });

            // Configuración de la tabla Messages
            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Messages");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.EncryptedContent)
                    .IsRequired()
                    .HasColumnType("NVARCHAR(MAX)");

                entity.Property(e => e.SentAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.IsRead)
                    .HasDefaultValue(false);

                entity.Property(e => e.MessageStatus)
                    .HasMaxLength(50)
                    .HasDefaultValue("Sent")
                    .HasColumnType("NVARCHAR(50)");

                // Relaciones
                entity.HasOne(e => e.Sender)
                    .WithMany(u => u.SentMessages)
                    .HasForeignKey(e => e.SenderId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Receiver)
                    .WithMany(u => u.ReceivedMessages)
                    .HasForeignKey(e => e.ReceiverId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Índices
                entity.HasIndex(e => e.SenderId)
                    .HasName("IX_Messages_SenderId");

                entity.HasIndex(e => e.ReceiverId)
                    .HasName("IX_Messages_ReceiverId");

                entity.HasIndex(e => new { e.SenderId, e.ReceiverId })
                    .HasName("IX_Messages_SenderReceiver");
            });
        }
    }
}
