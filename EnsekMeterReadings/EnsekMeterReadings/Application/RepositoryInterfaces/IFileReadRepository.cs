using EnsekMeterReadings.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnsekMeterReadings.Application.RepositoryInterfaces
{
    public interface IFileReadRepository
    {
        IEnumerable<UserAccount> ReadAccountsFile(string path);

        IEnumerable<MeterReading> ReadMeterReadingsFile([FromForm] IFormFile file);
    }
}
