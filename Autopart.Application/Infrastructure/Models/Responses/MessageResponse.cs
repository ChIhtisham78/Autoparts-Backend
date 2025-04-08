namespace Autopart.Application.Infrastructure.Models.Responses
{
    public class MessageResponse : BaseResponse
    {
        public string Message { get; set; }

        public MessageResponse(string message) : base()
        {
            Message = message;
        }

        public MessageResponse(string message, bool success) : this(message)
        {
            Success = success;
        }
    }
}
