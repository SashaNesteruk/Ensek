using EnsekMeterReadings.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MediatR;
using System.Collections.Generic;
using EnsekMeterReadings.Models;
using EnsekMeterReadings.Application.Repositories;
using System.Reflection;
using EnsekMeterReadings.Application.Commands;
using EnsekMeterReadings.Application.Validators;
using EnsekMeterReadings.Middleware;
using EnsekMeterReadings.Application.RepositoryInterfaces;

namespace EnsekMeterReadings
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Configure services
            services.AddScoped<IMeterReadingRepository, MeterReadingRepository>();
            services.AddScoped<IUserAccountRepository, UserAccountRepository>();
            services.AddScoped<IFileReadRepository, FileReadRepository>();
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddTransient<IMeterReadingValidator, MeterReadingValidator>();
            services.AddTransient<IRequestHandler<AddMeterReadingsCommand, List<MeterReading>>, AddMeterReadingsCommandHandler>();

            // Configure Context
            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDb");
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EnsekMeterReadings", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EnsekMeterReadings v1"));
                app.UseCors(builder => builder.WithOrigins("http://localhost:4200"));
            }

            app.UseErrorHandlingMiddleware();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public static class ErrorHandlingMiddlewareExtension
    {
        public static IApplicationBuilder UseErrorHandlingMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
