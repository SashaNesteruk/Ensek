using EnsekMeterReadings.Application.Validators;
using EnsekMeterReadings.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnsekMeterReadings.Application.RepositoryInterfaces;

namespace EnsekMeterReadings.Application.Commands
{
    public class AddMeterReadingsCommandHandler : IRequestHandler<AddMeterReadingsCommand, List<MeterReading>>
    {
        private readonly IMeterReadingRepository _meterReadingRepository;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IMeterReadingValidator _meterReadingValidator;
        private readonly IFileReadRepository _fileReadRepository;
        private readonly ILogger<AddMeterReadingsCommandHandler> _logger;

        public AddMeterReadingsCommandHandler(IMeterReadingRepository meterReadingRepository, 
                                              IUserAccountRepository userAccountRepository, 
                                              IMeterReadingValidator meterReadingValidator,
                                              IFileReadRepository fileReadRepository,
                                              ILogger<AddMeterReadingsCommandHandler> logger)
        {
            _meterReadingRepository = meterReadingRepository;
            _userAccountRepository = userAccountRepository;
            _meterReadingValidator = meterReadingValidator;
            _fileReadRepository = fileReadRepository;
            _logger = logger;
        }

        public async Task<List<MeterReading>> Handle(AddMeterReadingsCommand request, CancellationToken cancellationToken)
        {
            var file = request.File;

            var meterReadings = _fileReadRepository.ReadMeterReadingsFile(file).ToList();
            var faultyReadings = new List<MeterReading>();

            foreach(var reading in meterReadings)
            {
                if (!_meterReadingValidator.IsValidMeterReading(reading))
                {
                    faultyReadings.Add(reading);
                    _logger.LogWarning("Found faulty meter reading! {0}, {1}, {2}", reading.AccountId, reading.MeterReadingDateTime, reading.MeterReadValue);
                    continue;
                }

                var userAccount = await _userAccountRepository.GetUserAccountById(reading.AccountId, cancellationToken);

                if (userAccount == null)
                {
                    faultyReadings.Add(reading);
                    _logger.LogWarning("Found meter reading with no corresponding account! {0}, {1}", reading.MeterReadingDateTime, reading.MeterReadValue);
                    continue;
                }

                var userReadings = await _meterReadingRepository.GetDuplicateMeterReadings(reading.AccountId, reading.MeterReadingDateTime, reading.MeterReadValue, cancellationToken);

                if (userReadings.Any())
                {
                    faultyReadings.Add(reading);
                    _logger.LogWarning("Found duplicate meter reading! {0}, {1}, {2}", reading.AccountId, reading.MeterReadingDateTime, reading.MeterReadValue);
                }
                else 
                {
                    await _meterReadingRepository.AddMeterReading(reading, cancellationToken);
                    _logger.LogWarning("Entered meter reading: {0}, {1}, {2}", reading.AccountId, reading.MeterReadingDateTime, reading.MeterReadValue);
                }
            }

            return faultyReadings;
        }
    }
}
