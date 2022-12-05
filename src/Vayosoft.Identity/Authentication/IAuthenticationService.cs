namespace Vayosoft.Identity.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> AuthenticateAsync(string username, string password, string ipAddress, CancellationToken cancellationToken = default);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string ipAddress, CancellationToken cancellationToken = default);
        Task RevokeTokenAsync(string token, string ipAddress, CancellationToken cancellationToken = default);
    }
}
