using System.Runtime.Serialization;

namespace Autopart.API.Infrastructure
{
    [DataContract]
    public class ApiResponse
    {
        [DataMember]
        public virtual bool Success { get; set; } = true;

        [DataMember]
        public virtual int StatusCode { get; set; } = 200;

        [DataMember]
        public string Message { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public object Result { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public IEnumerable<ValidationError> Errors { get; set; }
    }
}
