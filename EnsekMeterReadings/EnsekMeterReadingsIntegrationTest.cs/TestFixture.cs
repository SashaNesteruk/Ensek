using AutoFixture;
using EnsekMeterReadings;
using EnsekMeterReadings.Application.RepositoryInterfaces;
using EnsekMeterReadings.Context;
using EnsekMeterReadings.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace EnsekMeterReadingsIntegrationTest.cs
{
    public class TestFixture
    {
        public HttpClient Client { get; }

        public List<UserAccount> Accounts;
        public Fixture Fixture { get; }

        private TestServer _server;

        public TestFixture()
        {
            Fixture = new Fixture();

            _server = new TestServer(new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .ConfigureTestServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType ==
                             typeof(DbContextOptions<ApplicationContext>));

                    services.Remove(descriptor);

                    services.AddDbContext<ApplicationContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                    });

                    var sp = services.BuildServiceProvider();

                    using var scope = sp.CreateScope();
                    var scopedServices = scope.ServiceProvider;
                    var dbContext = scopedServices.GetRequiredService<ApplicationContext>();
                    var csvService = scopedServices.GetRequiredService<IFileReadRepository>();
                    dbContext.Database.EnsureCreated();
                    dbContext.UserAccounts.AddRange(csvService.ReadAccountsFile(@".\DataFiles\Test_Accounts.csv"));
                    dbContext.SaveChanges();
                    Accounts = dbContext.UserAccounts.ToList();
                })
            );

            Client = _server.CreateClient();

        }

        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }
    }
}
