using Newtonsoft.Json;

namespace Api.LmcLib.Exceptions
{
    public class ErrorResponseException : Exception
    {
        public int StatusCode { get; set; }
        public string? Description { get; set; }
        public IList<Error> Errors = new List<Error>();
        public ErrorResponseException() : base() { }
        public ErrorResponseException(string message) : base(message) { }
        public ErrorResponseException(string message, Exception exp) : base(message, exp) { }
        public ErrorResponseException(int statusCode, string message, IList<Error> errors)
        {
            StatusCode = statusCode;
            Description = message;
            Errors = errors;
        }
    }

    public class Error
    {
        [JsonProperty("message", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? Message { get; set; }
        [JsonProperty("field", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string? Field { get; set; }
    }

    public class ErrorHandler
    {
        public ErrorHandler()
        {
            Errors = new List<Error>();
        }
        [JsonProperty("errors", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual List<Error> Errors { get; set; }

        [JsonProperty("message", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual string Message { get; set; } = "This information could not be found in the database.";
        
        [JsonProperty("status", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public virtual int Status { get; set; }
    }
}