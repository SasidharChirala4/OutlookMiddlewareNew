using Edreams.OutlookMiddleware.Model;
using Edreams.OutlookMiddleware.Model.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Edreams.OutlookMiddleware.DataAccess
{
    public class OutlookMiddlewareDbContext : DbContext
    {
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<CategorizationRequest> CategorizationRequests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.\\SQLDEV;Initial Catalog=EDREAMS-OUTLOOK-MIDDLEWARE;Integrated Security=True;MultipleActiveResultSets=true");
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
                e
                    .ToTable("Emails");
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
                    .HasConversion(new EnumToStringConverter<EmailStatus>());
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
                e.Property(x => x.UserPrincipalName).IsRequired();
                e.Property(x => x.UserPrincipalName).HasMaxLength(200);
                e.Property(x => x.InternetMessageId).IsRequired();
                e.Property(x => x.InternetMessageId).HasMaxLength(200);
                e.Property(x => x.IsCompose).IsRequired();
                e.Property(x => x.Sent).IsRequired();
                e.HasIndex(x => x.UserPrincipalName);
                e.HasIndex(x => new { x.UserPrincipalName, x.Sent, x.CategorizationRequestType, x.InternetMessageId });
                e.Property(x => x.InsertedBy).IsRequired();
                e.Property(x => x.InsertedBy).HasMaxLength(100);
                e.Property(x => x.UpdatedBy).IsRequired();
                e.Property(x => x.UpdatedBy).HasMaxLength(100);
            });
        }
    }
}