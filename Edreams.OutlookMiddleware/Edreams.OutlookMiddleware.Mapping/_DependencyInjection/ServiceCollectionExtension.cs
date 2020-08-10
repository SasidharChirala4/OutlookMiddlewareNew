using Edreams.OutlookMiddleware.DataTransferObjects.Api;
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
        }
    }
}