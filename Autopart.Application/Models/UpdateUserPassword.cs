namespace Autopart.Application.Models
{
    public class UpdateUserPassword
    {
        public int Id { get; set; }
        public string OldPassword { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
