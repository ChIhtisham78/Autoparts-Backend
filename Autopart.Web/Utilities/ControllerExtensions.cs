using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace ZikApp.API.Utilities
{
    public static class ControllerExtensions
    {

        public static int GetCurrentUserId(this ControllerBase controller)
        {
            var identity = controller.HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(JwtRegisteredClaimNames.NameId)?.Value;
            if (userId is null)
            {
                return default;
            }
            return Convert.ToInt32(userId, CultureInfo.InvariantCulture.NumberFormat);
        }

        public static string GetSubDomain(this ControllerBase controller)
        {
            var subDomain = string.Empty;

            var host = controller.HttpContext.Request.Host.Host;

            if (!string.IsNullOrWhiteSpace(host))
            {
                subDomain = host.Split('.')[0];
            }

            return subDomain.Trim().ToLower();
        }

        public static string GetHost(this ControllerBase controller)
        {
            var hostDomain = string.Empty;

            var host = controller.HttpContext.Request.Host.Host;

            if (!string.IsNullOrWhiteSpace(host))
            {
                var parts = host.Split('.');
                if (parts.Length > 1)
                    hostDomain = parts[1];
                else
                    hostDomain = parts[0];
            }

            return hostDomain.Trim().ToLower();
        }

        public static string GetRemoteIPAddress(this ControllerBase controller)
        {

            var ip = "";
            if (!string.IsNullOrEmpty(controller.HttpContext.Request.Headers["CF-CONNECTING-IP"]))
                ip = controller.HttpContext.Request.Headers["CF-CONNECTING-IP"].ToString();

            var ipAddress = controller.HttpContext.GetServerVariable("HTTP_X_FORWARDED_FOR");

            if (!string.IsNullOrEmpty(ipAddress))
            {
                var addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                    ip = addresses.Last();
            }


            return ip.ToString();
        }



    }
}
