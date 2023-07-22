using System.Security.Cryptography;
using System.Text;
using Api.CommonLib.Exceptions;
using Api.MessageLib.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Api.MessageLib.Services
{
    public class LineMessageValidation : ILineMessageValidation
    {
        private readonly ILogger<LineMessageValidation> _logger;
        private readonly IConfiguration _configuration;
        public LineMessageValidation(ILogger<LineMessageValidation> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public bool Validate(string signature, object body)
        {
            string lineSecret = _configuration["LineConfig:Channel:SecretId"];
            // create a new instance of HMACSHA256
            var key = Encoding.UTF8.GetBytes(lineSecret);
            var hmac = new HMACSHA256(key);

            // Compute the HMAC of the request body
            // the request body need to be string
            // var requestBody = JsonSerializer.Serialize(reqBody);
            // var requestBody = JsonConvert.SerializeObject(reqBody);
            var bodyBytes = Encoding.UTF8.GetBytes(body.ToString()!);
            var hmacBytes = hmac.ComputeHash(bodyBytes);

            // Compare the computed HMAC to the one sent in the request headers
            var receivedHmac = Convert.FromBase64String(signature);
            if (hmacBytes.SequenceEqual(receivedHmac))
            {
                // Request is valid
                // Console.WriteLine("Success!");
                _logger.LogInformation("Successfully validate the signature!");
                return true;
            }
            else
            {
                // Request is not valid
                // Console.WriteLine("Failed!");
                _logger.LogError("Failed validating the signature!");
                throw new ErrorResponseException(
                    StatusCodes.Status401Unauthorized,
                    "Failed validating the signature!",
                    new List<Error>()
                );
            }
        }
    }
}