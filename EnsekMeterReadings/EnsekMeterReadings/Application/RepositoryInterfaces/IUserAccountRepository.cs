using EnsekMeterReadings.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EnsekMeterReadings.Application.RepositoryInterfaces
{
    public interface IUserAccountRepository
    {
        Task<UserAccount> GetUserAccountById(int id, CancellationToken cancellationToken);
    }
}
