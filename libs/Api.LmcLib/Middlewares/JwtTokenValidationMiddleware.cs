using Api.LmcLib.Interfaces;
using Api.LmcLib.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Api.LmcLib.Middlewares
{
    public class JwtTokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly IJwtToken _jwtToken;

        public JwtTokenValidationMiddleware(RequestDelegate next, IConfiguration configuration, IJwtToken jwtToken)
        {
            _next = next;
            _configuration = configuration;
            _jwtToken = jwtToken;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                try
                {
                    // You can now access the user's claims from the principal and use them in your application
                    context.User = _jwtToken.ValidateJwtToken(token, "secretKey");
                }
                catch (Exception)
                {
                    // Token validation failed. You can handle the error here if needed.
                    string resMessage = "Invalid token";
                    ErrorHandler response = new ErrorHandler{
                        Status=StatusCodes.Status401Unauthorized,
                        Message=resMessage
                    };
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    string strResponse = JsonConvert.SerializeObject(response);
                    await context.Response.WriteAsync(strResponse);
                    return;
                }
            }

            await _next(context);
        }
    }
}