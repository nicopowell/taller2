using System.Security.Cryptography.X509Certificates;
using MVC.Models; 
public interface IUserRepository
{
    Usuario GetUser(string username,string password);
}
