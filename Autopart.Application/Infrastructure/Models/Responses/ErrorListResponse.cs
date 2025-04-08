namespace Autopart.Application.Infrastructure.Models.Responses
{
    public class ErrorListResponse : ErrorResponse
    {
        public IEnumerable<ValidationError> Errors { get; set; }

        public ErrorListResponse() : base()
        {
        }

        public ErrorListResponse(int? status, string title, string type, string traceId, IDictionary<string, string[]> errors) : base(status, title, type, traceId)
        {
            Errors = errors.SelectMany(error => error.Value.Select(v => new ValidationError(error.Key, v)));
        }

        public ErrorListResponse(ErrorResponse errorResponse, IDictionary<string, string[]> errors) : this(errorResponse?.Status, errorResponse?.Message, errorResponse?.Type, errorResponse?.TraceId, errors)
        {
        }
    }
}
