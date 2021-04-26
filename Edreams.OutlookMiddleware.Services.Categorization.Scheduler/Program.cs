using System.Security.Principal;
using Edreams.OutlookMiddleware.BusinessLogic.DependencyInjection;
using Edreams.OutlookMiddleware.Common._DependencyInjection;
using Edreams.OutlookMiddleware.Common.Security;
using Edreams.OutlookMiddleware.Common.Security.Interfaces;
using Edreams.Common.AzureServiceBus._DependencyInjection;
using Edreams.Common.Logging._DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Context;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Sinks.MSSqlServer;
using Serilog.Sinks.MSSqlServer.Sinks.MSSqlServer.Options;
using System.Collections.Generic;
using System.Data;
using System;

namespace Edreams.OutlookMiddleware.Services.Categorization.Scheduler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configBuilder =>
                {
                    configBuilder.AddEnvironmentVariables();
                })
                .ConfigureServices((hostBuilder, services) =>
                {
                    ISecurityContext securityContext = new SecurityContext();
                    securityContext.RefreshCorrelationId();
                    securityContext.SetUserIdentity(WindowsIdentity.GetCurrent());
                    services.AddSingleton(_ => securityContext);
                    LogContext.PushProperty("CorrelationId", securityContext.CorrelationId);
                    services.AddCommon();
                    services.AddEdreamsLogging();
                    services.AddConfiguration(hostBuilder.Configuration);
                    services.AddServiceBus();
                    services.AddBusinessLogic();

                    services.AddHostedService<CategorizationSchedulerWorker>();
                })
                .ConfigureLogging((hostContext, loggerBuilder) =>
                {
                    Log.Logger = new LoggerConfiguration()
                    // Start reading configuration from "appsettings.json"
                    .ReadFrom.Configuration(hostContext.Configuration)
                    // Enrich logging with contextual properties.
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("InsertedBy", WindowsIdentity.GetCurrent().Name)
                    // Enrich logging with deconstructed Exception properties.
                    .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                        .WithDefaultDestructurers()
                        .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() }))
                    // Write logging to the console.
                    .WriteTo.Console()
                    // Write logging to SQL Server database.
                    .WriteTo.MSSqlServer(
                        hostContext.Configuration.GetConnectionString("OutlookMiddlewareDbConnectionString"),
                        new SinkOptions
                        {
                            // Table creation is enforced by Entity Framework migrations.
                            AutoCreateSqlTable = false,
                            TableName = "Logs",
                        },
                        columnOptions: BuildSerilogSqlSinkColumnOptions())
                    .CreateLogger();

                    Serilog.Debugging.SelfLog.Enable(output =>
                    {
                        Console.WriteLine(output);
                    });
                    loggerBuilder.AddSerilog();
                });

        /// <summary>
        /// Configure SQL table columns for serlog
        /// </summary>
        /// <returns>Column Options</returns>
        private static ColumnOptions BuildSerilogSqlSinkColumnOptions()
        {
            ColumnOptions columnOptions = new ColumnOptions
            {
                AdditionalColumns = new List<SqlColumn>
                {
                    new SqlColumn("CorrelationId", SqlDbType.UniqueIdentifier),
                    new SqlColumn("SourceContext", SqlDbType.NVarChar, dataLength: 512),
                    new SqlColumn("MethodName", SqlDbType.NVarChar, dataLength: 512),
                    new SqlColumn("SourceFile", SqlDbType.NVarChar, dataLength: 512),
                    new SqlColumn("InsertedBy", SqlDbType.NVarChar, dataLength: 100),
                    new SqlColumn("LineNumber", SqlDbType.Int)
                }
            };
            columnOptions.Store.Remove(StandardColumn.MessageTemplate);
            columnOptions.Store.Remove(StandardColumn.Properties);
            columnOptions.Store.Add(StandardColumn.LogEvent);
            columnOptions.TimeStamp.ConvertToUtc = true;
            columnOptions.Id.ColumnName = "SysId";

            return columnOptions;
        }
    }
}