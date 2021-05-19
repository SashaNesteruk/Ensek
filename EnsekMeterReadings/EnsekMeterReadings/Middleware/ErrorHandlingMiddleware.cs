using EnsekMeterReadings.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EnsekMeterReadings.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public ErrorHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ErrorHandlingMiddleware>();
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task WriteExceptionResponseAsync(HttpContext httpContext, string result, HttpStatusCode code)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)code;
            return httpContext.Response.WriteAsync(result);
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {

            string result;
            _logger.LogError(exception, exception.Message);

            switch (exception)
            {
                case FileReadException ex:
                    result = JsonConvert.SerializeObject(new { isSuccess = false, error = ex.Message });
                    return WriteExceptionResponseAsync(context, result, HttpStatusCode.BadRequest);
                default:
                    result = JsonConvert.SerializeObject(new { isSuccess = false, error = "Server error" });
                    return WriteExceptionResponseAsync(context, result, HttpStatusCode.InternalServerError);
            }
        }
    }

}
