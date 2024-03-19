using System.Security.Claims;
using Microsoft.Extensions.Options;
using Vayosoft.Identity.Persistence;
using Vayosoft.Identity.Security;
using Vayosoft.Identity.Tokens;

namespace Vayosoft.Identity.Authentication
{
    public class AuthenticationService(
        IPasswordHasher passwordHasher,
        ITokenService<ClaimsPrincipal> tokenService,
        IOptions<TokenSettings> appSettings,
        IUserRepository userRepository)
        : IAuthenticationService
    {
        private readonly TokenSettings _appSettings = appSettings.Value;

        public async Task<AuthenticationResult> AuthenticateAsync(string username, string password, string ipAddress, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.FindByEmailAsync(username, cancellationToken);
            if (user is null || !passwordHasher.VerifyHashedPassword(user.PasswordHash, password))
                throw new ApplicationException("Username or password is incorrect");

            return await AuthenticateAsync(user, ipAddress, cancellationToken);
        }

        public async Task<AuthenticationResult> AuthenticateAsync(UserEntity user, string ipAddress, CancellationToken cancellationToken = default)
        {
            // authentication successful so generate jwt and refresh tokens
            List<RoleDTO> roles = null;
            if (userRepository is IUserRoleStore roleStore)
            {
                roles = await roleStore.GetUserRolesAsync(user.Id, cancellationToken);
            }
            var jwtToken = tokenService.GenerateToken(user, roles ?? Enumerable.Empty<RoleDTO>());
            var refreshToken = tokenService.GenerateRefreshToken(ipAddress);
            user.RefreshTokens.Add(refreshToken);

            // remove old refresh tokens from user
            RemoveOldRefreshTokens(user);

            // save changes to db
            await userRepository.UpdateAsync(user, cancellationToken);

            return new AuthenticationResult(user, jwtToken, refreshToken.Token, refreshToken.Expires);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string ipAddress, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.FindByRefreshTokenAsync(token, cancellationToken) ?? 
                throw new ApplicationException("Invalid token");

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
            if (refreshToken.IsRevoked)
            {
                // revoke all descendant tokens in case this token has been compromised
                RevokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
                // save changes to db
                await userRepository.UpdateAsync(user, cancellationToken);
            }

            if (!refreshToken.IsActive)
                throw new ApplicationException("Invalid token");

            // replace old refresh token with a new one (rotate token)
            var newRefreshToken = RotateRefreshToken(refreshToken, ipAddress);
            user.RefreshTokens.Add(newRefreshToken);

            // remove old refresh tokens from user
            RemoveOldRefreshTokens(user);
            // save changes to db
            await userRepository.UpdateAsync(user, cancellationToken);

            // generate new jwt
            List<RoleDTO> roles = null;
            if (userRepository is IUserRoleStore roleStore)
            {
                roles = await roleStore.GetUserRolesAsync(user.Id, cancellationToken);
            }

            var jwtToken = tokenService.GenerateToken(user, roles ?? Enumerable.Empty<RoleDTO>());

            return new AuthenticationResult(user, jwtToken, newRefreshToken.Token, newRefreshToken.Expires);
        }

        public async Task RevokeTokenAsync(string token, string ipAddress, CancellationToken cancellationToken = default)
        {
            var user = await userRepository.FindByRefreshTokenAsync(token, cancellationToken) ??
                throw new ApplicationException("Invalid token");

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive)
                throw new ApplicationException("Invalid token");

            // revoke token and save
            RevokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
            // save changes to db
            await userRepository.UpdateAsync(user, cancellationToken);
        }

        private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = tokenService.GenerateRefreshToken(ipAddress);
            RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
            return newRefreshToken;
        }

        private void RemoveOldRefreshTokens(IUser user)
        {
            // remove old inactive refresh tokens from user based on TTL in app settings
            user.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private static void RevokeDescendantRefreshTokens(RefreshToken refreshToken, IUser user, string ipAddress, string reason)
        {
            // recursively traverse the refresh token chain and ensure all descendants are revoked
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                var childToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
                if (childToken is { IsActive: true })
                    RevokeRefreshToken(childToken, ipAddress, reason);
                else if (childToken is not null)
                    RevokeDescendantRefreshTokens(childToken, user, ipAddress, reason);
            }
        }

        private static void RevokeRefreshToken(RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;
        }
    }
}
