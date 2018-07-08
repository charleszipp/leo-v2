using System.Threading.Tasks;

namespace Leo.Core.Security
{
    public interface IUserManager
    {
        Task<string> CreateOrReplaceUserAsync(string authenticationType, string userId, string userName, string email);
    }
}