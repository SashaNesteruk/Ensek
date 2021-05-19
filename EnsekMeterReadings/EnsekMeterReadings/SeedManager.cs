using EnsekMeterReadings.Application.RepositoryInterfaces;

using EnsekMeterReadings.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EnsekMeterReadings
{
    public static class SeedManager
    {
        public static IHost SeedDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                using (var appContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>())
                {
                    var scopedServices = scope.ServiceProvider;
                    var csvService = scopedServices.GetRequiredService<IFileReadRepository>();
                    appContext.Database.EnsureCreated();

                    appContext.UserAccounts.AddRange(csvService.ReadAccountsFile(@".\DataFiles\Test_Accounts.csv"));
                    appContext.SaveChanges();
                }
            }
            return host;
        }
    }
}
