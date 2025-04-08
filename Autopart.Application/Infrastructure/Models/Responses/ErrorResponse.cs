namespace Autopart.Application.Infrastructure.Models.Responses
{
    public class ErrorResponse : BaseResponse
    {
        public int? Status { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public string TraceId { get; set; }

        public ErrorResponse() : base(false)
        {
        }

        public ErrorResponse(int? status, string title, string type, string traceId) : this()
        {
            Status = status;
            Message = title;
            Type = type;
            TraceId = traceId;
        }
    }
}
