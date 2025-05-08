using Autopart.Application.Enum;
using Autopart.Application.Helpers;
using Autopart.Application.Models;
using Autopart.Application.Options;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Autopart.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthenticationOptions _authenticationOptions;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHelper _passwordHelper;
        private readonly SendGridSetting _sendGridSetting;
        private readonly IDistributedCache _cache;

        public AuthService(IOptions<AuthenticationOptions> authenticationOptions,
            IUserRepository userRepository,
            IPasswordHelper passwordHelper,
            IDistributedCache cache, SendGridSetting sendGridSetting)
        {
            _authenticationOptions = authenticationOptions.Value;
            _userRepository = userRepository;
            _passwordHelper = passwordHelper;
            _cache = cache;
            _sendGridSetting = sendGridSetting;
        }

        public async Task<bool> ChangePassword(ChangePasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                throw new ArgumentException("Email is required.");
            }

            if (request.Password != request.ConfirmPassword)
            {
                throw new ArgumentException("Passwords do not match.");
            }
            var user = await _userRepository.GetUserByEmail(request.Email);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }
            var passwordHash = _passwordHelper.CreatePasswordHash(user.SecurityStamp, request.Password);
            user.PasswordHash = passwordHash;
            _userRepository.UpdateUser(user);
            await _userRepository.UnitOfWork.SaveChangesAsync();

            return true;
        }


        public async Task<AuthResponse> Login(string email, string password, Guid sessionId)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return AuthResponse.Empty;
            }

            var isValidPassword = IsValidPassword(password, user);
            if (!isValidPassword || !user.IsActive)
            {
                return AuthResponse.Empty;
            }

            var accessToken = GenerateAccessToken(user, sessionId);
            var refreshToken = GenerateRefreshToken(user, out DateTime issuedAt, out DateTime expires);

            var authenticatedUser = new AuthenticatedUser()
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.AspNetUserRoles.FirstOrDefault()?.Role.Name,
                UserName = user.UserName,
                StripeVendorId = user.StripeVendorId,
                StripeDashboardAccess = user.StripeDashboardAccess,
                StripeVendorStatus = user.StripeVendorStatus,
                Permissions = GetPermissions(user)
            };

            return new AuthResponse()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                User = authenticatedUser
            };
        }
        public async Task<bool> ForgetPassword(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null || !user.IsActive)
            {
                return false;
            }

            var resetToken = Guid.NewGuid().ToString();
            var resetTokenKey = $"ResetTokens:{user.Email}";
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_authenticationOptions.AccessTokenExpirationInMinutes)
            };
            await _cache.SetStringAsync(resetTokenKey, resetToken, options);

            var resetLink = $"https://admin.easypartshub.com/createPassword?token={resetToken}&email={email}";
            var emailBody = $"Click <a href=\"{resetLink}\">here</a> to reset your password.";

            var emailResult = await _sendGridSetting.EmailAsync(
                subject: "Password Reset",
                email: email,
                userName: user.UserName,
                message1: string.Empty,
                body: emailBody,
                attachments: null
            );

            return emailResult;
        }



        public async Task<AuthResponse> AddUser(AddNewUserDto addNewUserDto, int permission)
        {
            try
            {
                // Create and save the new user
                var user = new AspNetUser
                {
                    UserName = addNewUserDto.UserName,
                    Email = addNewUserDto.Email,
                    PhoneNumber = addNewUserDto.PhoneNumber,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    NormalizedEmail = addNewUserDto.Email.ToUpper(),
                    SecurityStamp = _passwordHelper.CreateSalt(40),
                    NormalizedUserName = addNewUserDto.Email.ToUpper()
                };

                user.PasswordHash = _passwordHelper.CreatePasswordHash(user.SecurityStamp, addNewUserDto.Password);
                _userRepository.AddUser(user);
                await _userRepository.UnitOfWork.SaveChangesAsync();

                var roleInDb = await _userRepository.GetRoleById(permission);
                if (roleInDb == null)
                {
                    throw new Exception("Role not found");
                }

                var userRole = new AspNetUserRole
                {
                    RoleId = roleInDb.Id,
                    UserId = user.Id

                };
                user.AspNetUserRoles.Add(userRole);

                user.IsActive = roleInDb.Name == Roles.store_owner ? false : true;

                await _userRepository.UnitOfWork.SaveChangesAsync();

                // Generate tokens
                var sessionId = Guid.NewGuid();
                var accessToken = GenerateAccessToken(user, sessionId);
                var refreshToken = GenerateRefreshToken(user, out _, out _);

                // Prepare authenticated user
                var authenticatedUser = new AuthenticatedUser()
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Role = roleInDb.Name,
                    Permissions = GetPermissions(user)
                };

                // Return AuthResponse
                return new AuthResponse()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    User = authenticatedUser
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> Logout(string jwtToken)
        {
            try
            {
                string tokenKey = $"InvalidTokens:{jwtToken}";
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_authenticationOptions.AccessTokenExpirationInMinutes)
                };
                await _cache.SetStringAsync(tokenKey, jwtToken, options);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to invalidate JWT token.", ex);
            }
        }

        private static string[] GetPermissions(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }

            if (user.AspNetUserRoles == null || !user.AspNetUserRoles.Any())
            {
                throw new InvalidOperationException("User roles are not assigned or missing.");
            }

            var role = user.AspNetUserRoles.FirstOrDefault()?.Role?.Name;

            if (string.IsNullOrEmpty(role))
            {
                throw new InvalidOperationException("User role is missing or invalid.");
            }

            string[] perm;

            switch (role)
            {
                case Roles.Super_admin:
                    perm = new[] { "super_admin", "store_owner", "customer", "billing_manager", "stock_manager" };
                    break;
                case Roles.store_owner:
                    perm = new[] { "store_owner", "customer", "stock_manager" };
                    break;
                case Roles.Customer:
                    perm = new[] { "customer" };
                    break;
                case Roles.billing_manager:
                    perm = new[] { "billing_manager", "customer" };
                    break;
                case Roles.stock_manager:
                    perm = new[] { "stock_manager", "customer" };
                    break;
                default:
                    Console.WriteLine($"Warning: Role '{role}' is not explicitly recognized. Assigning default permissions.");
                    perm = new[] { "customer" };
                    break;
            }

            return perm;
        }



        private bool IsValidPassword(string password, AspNetUser user)
        {
            var passwordHash = _passwordHelper.CreatePasswordHash(user.SecurityStamp, password);
            return passwordHash == user.PasswordHash;
        }

        private string GenerateRefreshToken(AspNetUser user, out DateTime issuedAt, out DateTime expires)
        {
            issuedAt = DateTime.UtcNow;
            expires = issuedAt.AddMinutes(_authenticationOptions.RefreshTokenExpirationInMinutes);

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture))
                }),
                IssuedAt = issuedAt,
                Expires = expires,
                NotBefore = issuedAt,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_authenticationOptions.RefreshSecretKey), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _authenticationOptions.Issuer,
                Audience = _authenticationOptions.Audience
            };
            var refreshToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(refreshToken);
        }

        private string GenerateAccessToken(AspNetUser user, Guid uniqueId, string jti = null)
        {
            JwtSecurityTokenHandler tokenHandler = new();

            string tokenIdentifier = string.IsNullOrWhiteSpace(jti) ? Guid.NewGuid().ToString() : jti;

            IList<Claim> subjectClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, tokenIdentifier),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString(CultureInfo.InvariantCulture)),
                new Claim(CustomClaimTypes.Role, value: user.AspNetUserRoles.FirstOrDefault()?.Role.Name),
                new Claim(CustomClaimTypes.SessionId, uniqueId.ToString())
            };

            var userRole = user.AspNetUserRoles.FirstOrDefault()?.Role.Name;
            subjectClaims.Add(new Claim(CustomClaimTypes.Role, userRole));

            // Add "store_owner" role if the user is "super_admin"
            if (userRole == Roles.Super_admin)
            {
                subjectClaims.Add(new Claim(CustomClaimTypes.Role, Roles.store_owner));
            }
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                subjectClaims.Add(new Claim(ClaimTypes.Email, user.Email));
            }
            var date = DateTime.UtcNow;

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(subjectClaims),
                IssuedAt = date,
                Expires = date.AddMinutes(_authenticationOptions.AccessTokenExpirationInMinutes),
                NotBefore = date,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_authenticationOptions.SecretKey), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _authenticationOptions.Issuer,
                Audience = _authenticationOptions.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
