using System.Runtime.Serialization;

namespace Autopart.API.Infrastructure
{
    public class ApiError : ApiResponse
    {
        [DataMember]
        public override bool Success { get; set; } = false;

        [DataMember]
        public override int StatusCode { get; set; } = 400;
    }
}
