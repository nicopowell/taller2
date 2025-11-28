namespace MVC.Interfaces;
public interface IAuthenticationService
{
    bool Login( string username, string password);
    void Logout();
    bool IsAuthenticated();
    bool HasAccessLevel(string requiredAccessLevel);
}
