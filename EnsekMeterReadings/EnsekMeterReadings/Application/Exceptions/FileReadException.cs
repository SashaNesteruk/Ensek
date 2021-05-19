using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnsekMeterReadings.Application.Exceptions
{
    public class FileReadException : Exception
    {
        public FileReadException(string message) : base(message)
        {
        
        }
    }
}
