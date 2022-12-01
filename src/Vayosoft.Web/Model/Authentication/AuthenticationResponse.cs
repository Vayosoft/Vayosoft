namespace Vayosoft.Web.Model.Authentication
{
    public record AuthenticationResponse(
        string Username,
        string Token,
        long TokenExpirationTime);
}
