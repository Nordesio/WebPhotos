using KPO_Cursovaya.Models;
namespace WebApp;
public interface IAuthenticationService
{
    Task Authenticate(User user);
}
