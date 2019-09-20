using System;
using System.Threading.Tasks;
using MaxRev.Servers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Energy_Platform
{
    class Program
    {
        static Task Main(string[] args)
        {
            var runtime = ReactorStartup.From(args,
                new ReactorStartupConfig
                {
                    AutoregisterControllers = true
                });
            runtime.Configure((with, core) =>
            {
                with.Server("main", 8080,
                    server => server.Config.Communication.MaxClientCacheAge = 0);
                with.Services(services =>
                {
                    var conn = $@"Server=db,1433;Database=Main;User=sa;Password={Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD")};";

                    services.AddDbContext<Database>(options =>
                    {
                        options.UseSqlServer(conn);
                    });
                    services.AddTransient<DbInitializationHelper>();
                });
                with.FinalizingStartup += async s =>
                {
                    s.Config.FileSystem.FileAccess = true;

                    var init = s.Services.GetRequiredService<DbInitializationHelper>();
                    try
                    {
                        await init.SetupDatabaseAsync();
                    }
                    catch (AggregateException)
                    {
                        // TODO: Handle the System.AggregateException
                    }
                };
            });
            return runtime.RunAsync();
        }

    }
}
