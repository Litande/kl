using System.Threading.Tasks;
using KL.Auth.Models.User;

namespace KL.Auth.Services
{
    public interface IAdminAuthenticationService
    {
        Task<string> Login(LoginInputModel model);
    }
}