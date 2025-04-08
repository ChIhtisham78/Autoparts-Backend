namespace Autopart.Application.Infrastructure.Models.Responses
{
    public class ResultResponse : BaseResponse
    {
        public object Result { get; set; }

        public ResultResponse(object result) : base()
        {
            Result = result;
        }

        public ResultResponse(object result, bool success) : this(result)
        {
            Success = success;
        }
    }
}
