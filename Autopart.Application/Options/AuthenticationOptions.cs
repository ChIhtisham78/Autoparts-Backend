using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Options
{
    public class AuthenticationOptions
    {
        public const string SettingsSection = "AuthenticationSettings";

        public string Secret { get; set; }
        public string RefreshSecret { get; set; }
        public string ResetPasswordUrl { get; set; }
        public int Step { get; set; }
        public int Size { get; set; }
        public string HashMode { get; set; }
        public int AccessTokenExpirationInMinutes { get; set; }
        public int RefreshTokenExpirationInMinutes { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string ImageSLocalDicrectory { get; set; }
        public double ClockSkew { get; set; } = 10.0;
        public IList<string> ValidIssuers { get; set; } = new List<string>();
        public IList<string> ValidAudiences { get; set; } = new List<string>();
        public bool ValidateIssuer { get; set; }
        public bool ValidateAudience { get; set; }
        public bool ValidateLifetime { get; set; }
        public bool IncludeAccessTokenInResponse { get; set; }
        public ISet<string> IncludeAccessTokenInResponseUsers { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public string AccessTokenTemplateId { get; set; }

        public byte[] SecretKey => Encoding.ASCII.GetBytes(Secret);
        public byte[] RefreshSecretKey => Encoding.ASCII.GetBytes(RefreshSecret);
    }
}
