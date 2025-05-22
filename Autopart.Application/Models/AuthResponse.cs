namespace Autopart.Application.Models
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public AuthenticatedUser User { get; set; }
        public static AuthResponse Empty => new() { AccessToken = string.Empty, RefreshToken = string.Empty };
        public bool IsEmpty() => string.IsNullOrWhiteSpace(AccessToken) && string.IsNullOrWhiteSpace(RefreshToken);
    }
    public class AuthenticatedUser
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
        public string? StripeVendorId { get; set; }
        public string? StripeDashboardAccess { get; set; }
        public string? StripeVendorStatus { get; set; }
        public string[]? Permissions { get; set; }
    }
}
