using Edreams.Common.DataAccess.Interfaces;
using Edreams.OutlookMiddleware.DataAccess.Repositories;
using Edreams.OutlookMiddleware.Model;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Edreams.OutlookMiddleware.DataAccess.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static void AddDataAccess(this IServiceCollection services)
        {
            services.AddDbContext<OutlookMiddlewareDbContext>();
            services.AddDbContext<OutlookMiddlewarePreloadDbContext>();

            services.AddTransient<IRepository<FilePreload>, PreloadedFilesRepository>();

            services.AddTransient<IRepository<Batch>, BatchesRepository>();
            services.AddTransient<IRepository<Email>, EmailsRepository>();
            services.AddTransient<IRepository<File>, FilesRepository>();
            services.AddTransient<IRepository<EmailRecipient>, EmailRecipientsRepository>();
            services.AddTransient<IRepository<CategorizationRequest>, CategorizationRequestRepository>();
            services.AddTransient<IRepository<ProjectTask>, ProjectTaskRepository>();
            services.AddTransient<IRepository<File>, FilesRepository>();
            services.AddTransient<IRepository<Transaction>, TransactionsRepository>();
            services.AddTransient<IRepository<HistoricTransaction>, HistoricTransactionsRepository>();
            services.AddTransient<IRepository<MetaData>, MetaDataRepository>();
        }
    }
}