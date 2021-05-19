using EnsekMeterReadings.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EnsekMeterReadings.Application.Validators
{
    public class MeterReadingValidator : IMeterReadingValidator
    {
        public bool IsValidMeterReading(MeterReading meterReading)
        {
            return Regex.IsMatch(meterReading.MeterReadValue, "^[0-9]{5}$");
        }
    }
}
