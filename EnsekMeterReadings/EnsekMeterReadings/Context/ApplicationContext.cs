using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnsekMeterReadings.Models;
using Microsoft.EntityFrameworkCore;

namespace EnsekMeterReadings.Context
{
    public class ApplicationContext : DbContext
    {

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAccount>()
                .HasKey(i => i.AccountId);

            modelBuilder.Entity<MeterReading>().HasKey(i => i.RecordId);


        }

        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }
    }
}
