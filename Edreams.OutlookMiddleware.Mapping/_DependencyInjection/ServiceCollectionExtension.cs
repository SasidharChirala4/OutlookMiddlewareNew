using Edreams.Contracts.Data.Logging;
using Edreams.OutlookMiddleware.DataTransferObjects;
using Edreams.OutlookMiddleware.DataTransferObjects.Api;
using Edreams.OutlookMiddleware.Mapping.Custom;
using Edreams.OutlookMiddleware.Mapping.Custom.Interfaces;
using Edreams.OutlookMiddleware.Mapping.Interfaces;
using Edreams.OutlookMiddleware.Model;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Edreams.OutlookMiddleware.Mapping.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static void AddMapping(this IServiceCollection services)
        {
            services.AddTransient<IMapper<CreateMailRequest, FilePreload>, CreateEmailRequestToFilePreloadMapper>();
            services.AddTransient<IMapper<RecordLogRequest, LogEntry>, RecordLogRequestToLogEntryMapper>();
            services.AddTransient<IMapper<Transaction, TransactionDto>, TransactionToTransactionDtoMapper>();
            services.AddTransient<IMapper<Transaction, HistoricTransaction>, TransactionToHistoricTransactionMapper>();

            services.AddSingleton<IPreloadedFilesToFilesMapper, PreloadedFilesToFilesMapper>();
        }
    }
}