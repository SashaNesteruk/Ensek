using EnsekMeterReadings.Application.RepositoryInterfaces;
using EnsekMeterReadings.Context;
using EnsekMeterReadings.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EnsekMeterReadings.Application.Repositories
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private ApplicationContext _context;

        public UserAccountRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Task<UserAccount> GetUserAccountById(int id, CancellationToken cancellationToken)
        {
            return _context.UserAccounts.FirstOrDefaultAsync(x => x.AccountId == id);
        }
    }
}
