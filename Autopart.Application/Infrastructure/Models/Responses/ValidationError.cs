namespace Autopart.Application.Infrastructure.Models.Responses
{
    public class ValidationError
    {
        public string Field { get; set; }
        public string Error { get; set; }

        public ValidationError()
        {
        }

        public ValidationError(string field, string error) : this()
        {
            Field = field;
            Error = error;
        }
    }
}
