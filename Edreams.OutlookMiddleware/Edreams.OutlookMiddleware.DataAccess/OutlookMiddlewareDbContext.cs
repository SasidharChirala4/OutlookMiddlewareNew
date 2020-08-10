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
                e
                    .ToTable("Files");
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
                    .Property(x => x.Kind)
                    .HasConversion(new EnumToStringConverter<FileKind>());
                e
                    .Property(x => x.Status)
                    .HasConversion(new EnumToStringConverter<FileStatus>());
            });
        }
    }
}