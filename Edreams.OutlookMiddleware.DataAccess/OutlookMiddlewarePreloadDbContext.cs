﻿using Edreams.OutlookMiddleware.Enums;
using Edreams.OutlookMiddleware.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;

namespace Edreams.OutlookMiddleware.DataAccess
{
    public class OutlookMiddlewarePreloadDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        
        public DbSet<FilePreload> PreloadedFiles { get; set; }

        public OutlookMiddlewarePreloadDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration.GetConnectionString("OutlookMiddlewarePreloadDbConnectionString");
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
                    .Property(x => x.Kind)
                    .HasConversion(new EnumToStringConverter<FileKind>());
                e
                    .Property(x => x.Status)
                    .HasConversion(new EnumToStringConverter<EmailPreloadStatus>());
                e
                    .Property(x => x.FileStatus)
                    .HasConversion(new EnumToStringConverter<FilePreloadStatus>());
            });
        }
    }
}