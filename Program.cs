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
                    var conn = @"Server=(localdb)\mssqllocaldb;Database=Main;ConnectRetryCount=5";

                    services.AddDbContext<Database>(options =>
                    {
                        options.UseSqlServer(conn);
                    });
                });
            });
            return runtime.RunAsync();
        }
    }
}
