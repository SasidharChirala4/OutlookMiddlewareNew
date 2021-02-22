using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;

namespace Edreams.OutlookMiddleware.DataAccess
{
    public class OutlookMiddlewareDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<CategorizationRequest> CategorizationRequests { get; set; }
        public DbSet<EmailRecipient> EmailRecipients { get; set; }
        public DbSet<Transaction> TransactionQueue { get; set; }
        public DbSet<HistoricTransaction> TransactionHistory { get; set; }

        public OutlookMiddlewareDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.GetConnectionString("OutlookMiddlewareDbConnectionString");
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Batch>(e =>
            {
                e
                    .ToTable("Batches");
                e
                    .HasKey(x => x.Id)
                    .IsClustered(false);
                e
                    .HasIndex(x => x.SysId)
                    .IsUnique()
                    .IsClustered();
                e
                    .Property(x => x.SysId)
                    .ValueGeneratedOnAdd();
                e
                    .Property(x => x.Status)
                    .HasConversion(new EnumToStringConverter<BatchStatus>());
            });

            modelBuilder.Entity<Email>(e =>
            {
                e.ToTable("Emails");
                e.HasKey(x => x.Id)
                    .IsClustered(false);
                e.HasIndex(x => x.SysId)
                    .IsUnique()
                    .IsClustered();
                e.Property(x => x.SysId)
                    .ValueGeneratedOnAdd();
                e.Property(x => x.Status)
                    .HasConversion(new EnumToStringConverter<EmailStatus>());
                e.Property(x => x.InternetMessageId)
                    .HasMaxLength(200);
                e.HasIndex(x => x.InternetMessageId);
            });

            modelBuilder.Entity<File>(e =>
            {
                e.ToTable("Files");
                e.HasKey(x => x.Id).IsClustered(false);
                e.HasIndex(x => x.SysId).IsUnique().IsClustered();
                e.Property(x => x.SysId).ValueGeneratedOnAdd();
                e.Property(x => x.Kind).HasConversion(new EnumToStringConverter<FileKind>());
                e.Property(x => x.Status).HasConversion(new EnumToStringConverter<FileStatus>());
            });

            modelBuilder.Entity<CategorizationRequest>(e =>
            {
                e.ToTable("CategorizationRequest");
                e.HasKey(x => x.Id).IsClustered(false);
                e.HasIndex(x => x.SysId).IsUnique().IsClustered();
                e.Property(x => x.EmailAddress).IsRequired();
                e.Property(x => x.EmailAddress).HasMaxLength(200);
                e.Property(x => x.InternetMessageId).IsRequired();
                e.Property(x => x.InternetMessageId).HasMaxLength(200);
                e.Property(x => x.Status).IsRequired();
                e.Property(x => x.Type).IsRequired();
                e.HasIndex(x => x.EmailAddress);
                e.HasIndex(x => new { x.EmailAddress, x.Status, x.CategorizationRequestType, x.InternetMessageId });
                e.Property(x => x.InsertedBy).IsRequired();
                e.Property(x => x.InsertedBy).HasMaxLength(100);
                e.Property(x => x.UpdatedBy).IsRequired();
                e.Property(x => x.UpdatedBy).HasMaxLength(100);
            });

            modelBuilder.Entity<Transaction>(e =>
            {
                e.ToTable("TransactionQueue");
                e.HasKey(x => x.Id).IsClustered(false);
                e.HasIndex(x => x.SysId).IsUnique().IsClustered();
                e.Property(x => x.SysId).ValueGeneratedOnAdd();
                e.Property(x => x.Status).HasConversion(new EnumToStringConverter<TransactionStatus>());
                e.Property(x => x.ProcessingEngine).HasMaxLength(100);
                e.Property(x => x.InsertedBy).IsRequired();
                e.Property(x => x.InsertedBy).HasMaxLength(100);
                e.Property(x => x.UpdatedBy).HasMaxLength(100);
            });

            modelBuilder.Entity<HistoricTransaction>(e =>
            {
                e.ToTable("TransactionHistory");
                e.HasKey(x => x.Id).IsClustered(false);
                e.HasIndex(x => x.SysId).IsUnique().IsClustered();
                e.Property(x => x.SysId).ValueGeneratedOnAdd();
                e.Property(x => x.Status).HasConversion(new EnumToStringConverter<TransactionStatus>());
                e.Property(x => x.ProcessingEngine).HasMaxLength(100);
                e.Property(x => x.InsertedBy).IsRequired();
                e.Property(x => x.InsertedBy).HasMaxLength(100);
                e.Property(x => x.UpdatedBy).HasMaxLength(100);
            });

            modelBuilder.Entity<EmailRecipient>(e =>
            {
                e.ToTable("EmailRecipients");
                e.HasKey(x => x.Id).IsClustered(false);
                e.HasIndex(x => x.SysId).IsUnique().IsClustered();
                e.Property(x => x.SysId).ValueGeneratedOnAdd();
                e.Property(x => x.Recipient).IsRequired();
                e.Property(x => x.Recipient).HasMaxLength(200);
                e.Property(x => x.Type).IsRequired();
                e.Property(x => x.Type).HasConversion(new EnumToStringConverter<EmailRecipientType>());
                e.Property(x => x.InsertedBy).IsRequired();
                e.Property(x => x.InsertedBy).HasMaxLength(100);
                e.Property(x => x.UpdatedBy).HasMaxLength(100);
            });
        }
    }
}