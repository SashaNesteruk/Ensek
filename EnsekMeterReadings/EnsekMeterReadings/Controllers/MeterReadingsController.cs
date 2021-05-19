using EnsekMeterReadings.Application.Commands;
using EnsekMeterReadings.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EnsekMeterReadings.Controllers
{
    [ApiController]
    [Route("meter-reading-uploads")]
    public class MeterReadingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly ILogger<MeterReadingsController> _logger;

        public MeterReadingsController(ILogger<MeterReadingsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<int>> AddMeterReadings(CancellationToken cancellationToken)
        {
            var file = Request.Form.Files[0];
            var command = new AddMeterReadingsCommand(file);
            var result = await _mediator.Send(command, cancellationToken);

            return Created("/meter-reading-uploads", result.Count);
        }
    }
}
