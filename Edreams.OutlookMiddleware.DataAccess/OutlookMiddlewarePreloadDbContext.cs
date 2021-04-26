using Edreams.OutlookMiddleware.Common.Configuration.Interfaces;
using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Edreams.OutlookMiddleware.DataAccess
{
    public class OutlookMiddlewarePreloadDbContext : DbContext
    {
        private readonly IEdreamsConfiguration _configuration;

        public DbSet<FilePreload> PreloadedFiles { get; set; }

        public OutlookMiddlewarePreloadDbContext(IEdreamsConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.OutlookMiddlewarePreloadDbConnectionString;
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FilePreload>(e =>
            {
                e
                    .ToTable("PreloadedFiles");
                e
                    .HasKey(x => x.Id)
                    .IsClustered(false);
                e
                    .HasIndex(x => x.SysId)
                    .IsUnique()
                    .IsClustered();
                e
                    .HasIndex(x => x.BatchId);
                e
                    .Property(x => x.SysId)
                    .ValueGeneratedOnAdd();
                e
                    .Property(x => x.EmailKind)
                    .HasConversion(new EnumToStringConverter<EmailKind>());
                e
                    .Property(x => x.Kind)
                    .HasConversion(new EnumToStringConverter<FileKind>());
                e
                    .Property(x => x.Status)
                    .HasConversion(new EnumToStringConverter<EmailPreloadStatus>());
                e
                    .Property(x => x.FileStatus)
                    .HasConversion(new EnumToStringConverter<FilePreloadStatus>());
                e.
                     Property(x => x.InternetMessageId)
                    .HasMaxLength(200);
                e.
                     Property(x => x.InsertedBy).IsRequired();
                e.
                     Property(x => x.InsertedBy).HasMaxLength(100);
                e.
                     Property(x => x.UpdatedBy).HasMaxLength(100);

            });
        }
    }
}