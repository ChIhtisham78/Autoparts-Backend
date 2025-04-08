namespace Autopart.Application.Infrastructure.Models.Responses
{
    public class BaseResponse
    {
        public bool Success { get; set; }

        public BaseResponse()
        {
            Success = true;
        }

        public BaseResponse(bool success) : this()
        {
            Success = success;
        }
    }
}
