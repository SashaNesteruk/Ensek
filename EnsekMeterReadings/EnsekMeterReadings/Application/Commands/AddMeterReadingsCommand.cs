using EnsekMeterReadings.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnsekMeterReadings.Application.Commands
{
    public class AddMeterReadingsCommand : IRequest<List<MeterReading>>
    {
        public AddMeterReadingsCommand(IFormFile file)
        {
            File = file;
        }

        public IFormFile File { get; }
    }
}
