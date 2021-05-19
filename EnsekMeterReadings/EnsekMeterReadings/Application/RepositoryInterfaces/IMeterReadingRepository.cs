using EnsekMeterReadings.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EnsekMeterReadings.Application.RepositoryInterfaces
{
    public interface IMeterReadingRepository
    {
        Task<IEnumerable<MeterReading>> AddMeterReadings(IEnumerable<MeterReading> meterReadings, CancellationToken token);
        Task<List<MeterReading>> GetDuplicateMeterReadings(int id, DateTime meterReadingDateTime, string meterReadValue, CancellationToken token);
        Task<MeterReading> AddMeterReading(MeterReading meterReading, CancellationToken token);
    }
}
