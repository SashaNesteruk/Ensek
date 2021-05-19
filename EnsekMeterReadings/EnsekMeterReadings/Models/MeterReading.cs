using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnsekMeterReadings.Models
{
    public class MeterReading
    {
        public Guid RecordId { get; set; }
        public int AccountId { get; set; }
        public DateTime MeterReadingDateTime { get; set; }

        public string MeterReadValue { get; set; }
    }
}
