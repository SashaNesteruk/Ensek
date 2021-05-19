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
    public class MeterReadingRepository : IMeterReadingRepository
    {
        private ApplicationContext _context;

        public MeterReadingRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MeterReading>> AddMeterReadings(IEnumerable<MeterReading> meterReadings, CancellationToken token)
        {
            await _context.MeterReadings.AddRangeAsync(meterReadings, token);
            await _context.SaveChangesAsync(token);

            return meterReadings;
        }

        public async Task<MeterReading> AddMeterReading(MeterReading meterReading, CancellationToken token)
        {
            await _context.MeterReadings.AddAsync(meterReading, token);
            await _context.SaveChangesAsync(token);

            return meterReading;
        }

        public Task<List<MeterReading>> GetDuplicateMeterReadings(int id, DateTime meterReadingDateTime, string meterReadValue, CancellationToken token)
        {
            return _context.MeterReadings.Where(x => x.AccountId == id
                                                        && x.MeterReadingDateTime == meterReadingDateTime 
                                                        && x.MeterReadValue == meterReadValue).ToListAsync(token);
        }
    }
}
