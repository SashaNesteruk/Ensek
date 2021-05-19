using EnsekMeterReadings.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnsekMeterReadings.Application.Validators
{
    public interface IMeterReadingValidator
    {
        bool IsValidMeterReading(MeterReading meterReading);
    }
}
