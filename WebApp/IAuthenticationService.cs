using DbData.Models;
namespace WebApp;
public interface IAuthenticationService
{
    Task Authenticate(User user);
}
