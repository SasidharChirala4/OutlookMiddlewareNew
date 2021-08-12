using Edreams.OutlookMiddleware.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace Edreams.OutlookMiddleware.Db
{
    [Cmdlet(VerbsData.Publish, "EdreamsOutlookMiddlewareDatabase")]
    public class PublishOutlookMiddlewareDatabaseCommand : Cmdlet
    {
        #region <| Private Members |>

        private readonly Dictionary<string, string> _assemblyRedirects = new Dictionary<string, string>
        { 
            // Assembly binding redirects for:
            // Upgrade to Entity Framework 3.1.8
            { "System.Threading.Tasks.Extensions, Version=4.2.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51", "System.Threading.Tasks.Extensions.dll" },
            { "Microsoft.Extensions.DependencyInjection.Abstractions, Version=3.1.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60", "Microsoft.Extensions.DependencyInjection.Abstractions.dll" },
            { "Microsoft.Extensions.Logging.Abstractions, Version=3.1.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60", "Microsoft.Extensions.Logging.Abstractions.dll" },
            { "Microsoft.Extensions.Options, Version=3.1.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60", "Microsoft.Extensions.Options.dll" },
            { "Microsoft.Extensions.Caching.Abstractions, Version=3.1.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60", "Microsoft.Extensions.Caching.Abstractions.dll" },
            { "Microsoft.Extensions.Primitives, Version=3.1.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60", "Microsoft.Extensions.Primitives.dll" },
            { "System.ComponentModel.Annotations, Version=4.2.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Annotations.dll" },
         };

        #endregion

        #region <| Powershell Parameters |>

        [Parameter(Mandatory = true)]
        public string ConnectionString { get; set; }


        #endregion

        #region <| Construction |>

        public PublishOutlookMiddlewareDatabaseCommand()
        {
            // Because PowerShell is not loading the config file containing AssemblyRedirects
            // We need to customize it by listening to the AppDomain.AssemblyResolve event.
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        #endregion

        #region <| Powershell CmdLet Implementation |>

        protected override void ProcessRecord()
        {
            using (var dbContext = new OutlookMiddlewareDbContext(ConnectionString))
            {
                dbContext.Database.SetCommandTimeout(3600);
                var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
                WriteObject(pendingMigrations.Count > 0 ? "The following pending migrations will be published" : "There are no pending migrations to publish");
                foreach (var pendingMigration in pendingMigrations)
                {
                    WriteObject(pendingMigration);
                }

                dbContext.Database.Migrate();
            }
        }

        #endregion

        #region <| Event Handlers |>

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // Look through known AssemblyRedirects and return the assembly from disk manually.
            foreach (KeyValuePair<string, string> assemblyRedirect in _assemblyRedirects)
            {
                if (assemblyRedirect.Key == args.Name)
                {
                    Assembly currentAssembly = Assembly.GetAssembly(typeof(PublishOutlookMiddlewareDatabaseCommand));
                    string currentAssemblyPath = Path.GetDirectoryName(currentAssembly.Location);

                    if (!string.IsNullOrEmpty(currentAssemblyPath))
                    {
                        string assemblyRedirectPath = Path.Combine(currentAssemblyPath, assemblyRedirect.Value);
                        WriteObject($"Assembly will be resolved from custom location: {assemblyRedirectPath}!");

                        return Assembly.LoadFrom(assemblyRedirectPath);
                    }
                }
            }

            // From documentation:
            // It is the responsibility of the ResolveEventHandler for this event to
            // return the assembly that is specified by the ResolveEventArgs.Name
            // property, or to return null if the assembly is not recognized.
            return null;
        }

        #endregion
    }
}
