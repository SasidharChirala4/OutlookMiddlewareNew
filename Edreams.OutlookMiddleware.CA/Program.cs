using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Edreams.Common.AzureServiceBus._DependencyInjection;
using Edreams.Common.Logging._DependencyInjection;
using Edreams.Common.Security._DependencyInjection;
using Edreams.OutlookMiddleware.BusinessLogic.DependencyInjection;
using Edreams.OutlookMiddleware.BusinessLogic.Interfaces;
using Edreams.OutlookMiddleware.Common._DependencyInjection;
using Edreams.OutlookMiddleware.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Edreams.OutlookMiddleware.CA
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder().Build();

            IServiceCollection services = new ServiceCollection();
            _ = services.AddEdreamsSecurity(WindowsIdentity.GetCurrent());
            services.AddCommon();
            services.AddEdreamsLogging();
            services.AddConfiguration(configuration);
            services.AddServiceBus();
            services.AddBusinessLogic();
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            try
            {

                var transactionManager = serviceProvider.GetService<ITransactionQueueManager>();
                await transactionManager.CreateCategorizationTransaction(Guid.NewGuid());

                var transaction = await transactionManager.GetNextCategorizationTransaction();
                await transactionManager.UpdateTransactionStatusAndArchive(transaction.Id, TransactionStatus.ProcessingSucceeded);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}