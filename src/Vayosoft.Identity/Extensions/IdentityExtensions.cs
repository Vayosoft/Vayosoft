﻿using System.Security.Claims;
using System.Security.Principal;

namespace Vayosoft.Identity.Extensions
{
    public static class IdentityExtensions
    {
        public static long GetProviderId(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(UserClaimType.ProviderId);

            return claim == null ? 0 : long.Parse(claim.Value);
        }

        public static long GetUserId(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            return claim == null ? 0 : long.Parse(claim.Value);
        }

        public static string GetUserName(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(ClaimTypes.Name);

            return claim?.Value ?? string.Empty;
        }

        public static string GetUserEmail(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(ClaimTypes.Email);

            return claim?.Value ?? string.Empty;
        }

        public static UserType GetUserType(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(UserClaimType.UserType);

            if (claim == null)
                return UserType.Guest;

            Enum.TryParse<UserType>(claim.Value, out var userType);
            return userType;
        }
    }
}
