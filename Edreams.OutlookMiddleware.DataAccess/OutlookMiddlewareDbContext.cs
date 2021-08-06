using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Edreams.OutlookMiddleware.DataAccess
{
    public class OutlookMiddlewareDbContext : DbContext
    {
        private readonly IEdreamsConfiguration _configuration;

        public DbSet<Batch> Batches { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<CategorizationRequest> CategorizationRequests { get; set; }
        public DbSet<EmailRecipient> EmailRecipients { get; set; }
        public DbSet<Transaction> TransactionQueue { get; set; }
        public DbSet<HistoricTransaction> TransactionHistory { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<ProjectTaskUserInvolvement> ProjectTaskUserInvolvements { get; set; }
        public DbSet<Metadata> Metadata { get; set; }
        public OutlookMiddlewareDbContext(IEdreamsConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.OutlookMiddlewareDbConnectionString;
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
                    .Property(x => x.UploadOption)
                    .HasConversion(new EnumToStringConverter<EmailUploadOptions>());
                e
                    .Property(x => x.UploadLocationSite).HasMaxLength(200);
                e
                    .Property(x => x.UploadLocationFolder).HasMaxLength(200);
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
                e.Property(x=>x.EmailKind)
                     .HasConversion(new EnumToStringConverter<EmailKind>());
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
                e.Property(x => x.ShouldUpload).HasDefaultValue(true);
                e.Property(x => x.OriginalName).HasMaxLength(255);
                e.Property(x => x.NewName).HasMaxLength(255);
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
                e.Property(x => x.Status).HasConversion(new EnumToStringConverter<CategorizationRequestStatus>());
                e.Property(x => x.Type).HasConversion(new EnumToStringConverter<CategorizationRequestType>());
                e.HasIndex(x => x.EmailAddress);
                e.Property(x => x.InsertedBy).IsRequired();
                e.Property(x => x.InsertedBy).HasMaxLength(100);
                e.Property(x => x.UpdatedBy).HasMaxLength(100);
            });

            modelBuilder.Entity<Transaction>(e =>
            {
                e.ToTable("TransactionQueue");
                e.HasKey(x => x.Id).IsClustered(false);
                e.HasIndex(x => x.SysId).IsUnique().IsClustered();
                e.Property(x => x.SysId).ValueGeneratedOnAdd();
                e.Property(x => x.Status).HasConversion(new EnumToStringConverter<TransactionStatus>());
                e.Property(x => x.Type).HasConversion(new EnumToStringConverter<TransactionType>());
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
                e.Property(x => x.Type).HasConversion(new EnumToStringConverter<TransactionType>());
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
                e.Property(x => x.Kind).IsRequired();
                e.Property(x => x.Kind).HasConversion(new EnumToStringConverter<EmailRecipientKind>());
                e.Property(x => x.InsertedBy).IsRequired();
                e.Property(x => x.InsertedBy).HasMaxLength(100);
                e.Property(x => x.UpdatedBy).HasMaxLength(100);
            });
            modelBuilder.Entity<Log>(e =>
            {
                e.ToTable("Logs");
                e.Property(x => x.Id).HasColumnName("SysId");
                e.Property(x => x.SourceContext).HasMaxLength(512);
                e.Property(x => x.MethodName).HasMaxLength(512);
                e.Property(x => x.SourceFile).HasMaxLength(512);
                e.Property(x => x.InsertedBy).IsRequired();
                e.Property(x => x.InsertedBy).HasMaxLength(100);
                e.HasIndex(x => x.Level);
                e.HasIndex(x => x.TimeStamp);
                e.HasIndex(x => x.CorrelationId);
            });
            modelBuilder.Entity<ProjectTask>(e =>
            {
                e.ToTable("ProjectTasks");
                e.HasKey(x => x.Id).IsClustered(false);
                e.HasIndex(x => x.SysId).IsUnique().IsClustered();
                e.Property(x => x.SysId).ValueGeneratedOnAdd();
                e.Property(x => x.TaskName).HasMaxLength(200);
                e.Property(x => x.Description).HasMaxLength(300);
                e.Property(x => x.Priority).HasMaxLength(10).HasConversion(new EnumToStringConverter<ProjectTaskPriority>());
                e.Property(x => x.InsertedBy).IsRequired();
                e.Property(x => x.InsertedBy).HasMaxLength(100);
                e.Property(x => x.UpdatedBy).HasMaxLength(100);
            });
            modelBuilder.Entity<ProjectTaskUserInvolvement>(e =>
            {
                e.ToTable("ProjectTaskUserInvolvements");
                e.HasKey(x => x.Id).IsClustered(false);
                e.HasIndex(x => x.SysId).IsUnique().IsClustered();
                e.Property(x => x.SysId).ValueGeneratedOnAdd();
                e.Property(x => x.UserId).HasMaxLength(100);
                e.Property(x => x.Type).HasMaxLength(10).HasConversion(new EnumToStringConverter<ProjectTaskUserInvolvementType>());
                e.Property(x => x.PrincipalName).HasMaxLength(100);
                e.Property(x => x.InsertedBy).IsRequired();
                e.Property(x => x.InsertedBy).HasMaxLength(100);
                e.Property(x => x.UpdatedBy).HasMaxLength(100);
            });
            modelBuilder.Entity<Metadata>(e =>
            {
                e.ToTable("Metadata");
                e.HasKey(x => x.Id).IsClustered(false);
                e.HasIndex(x => x.SysId).IsUnique().IsClustered();
                e.Property(x => x.SysId).ValueGeneratedOnAdd();
                e.Property(x => x.PropertyName).HasMaxLength(100);
                e.Property(x => x.InsertedBy).IsRequired();
                e.Property(x => x.InsertedBy).HasMaxLength(100);
                e.Property(x => x.UpdatedBy).HasMaxLength(100);
            });
        }
    }
}