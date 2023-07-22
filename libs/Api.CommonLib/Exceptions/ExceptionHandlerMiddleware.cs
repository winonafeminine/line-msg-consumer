using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Api.CommonLib.Exceptions
{
    public class ExceptionHandlerMidleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMidleware> _logger;

        public ExceptionHandlerMidleware(RequestDelegate next, ILogger<ExceptionHandlerMidleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                HandleExceptionAsync(context, ex);
            }
        }

        private void HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.StatusCode = statusCode;

            ErrorHandler response = new ErrorHandler();

            if (ex is ErrorResponseException err)
            {
                context.Response.StatusCode = err.StatusCode;
                response.Status = err.StatusCode;
                response.Message = err.Description!;
                response.Errors = err.Errors.ToList();
            }
            else
            {
                response.Message = ex.Message;
            }
            string strRes = JsonConvert.SerializeObject(response);
            // _logger.LogError(strRes);
            context.Response.WriteAsync(strRes).GetAwaiter().GetResult(); ;
            // throw ex;
        }
    }

    public static class ExceptionHandlerMidlewareExtensions
    {
        public static IApplicationBuilder UseResponseExceptionHandler
        (this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMidleware>();
        }
    }
}