using Leo.Core.Id;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leo.Core.Security
{
    public class InMemoryUserManager : IUserManager
    {
        private readonly IdProvider ids;
        private readonly IDictionary<string, User> users = new Dictionary<string, User>();

        public InMemoryUserManager(IdProvider ids) => this.ids = ids;

        public Task<string> CreateOrReplaceUserAsync(string authenticationType, string userId, string userName, string email)
        {
            var id = ids.Create();
            users.Add(id, new User(authenticationType, userId, userName, email));
            return Task.FromResult(id);
        }

        private class User
        {
            public User(string authenticationType, string userId, string userName, string email)
            {
                AuthenticationType = authenticationType;
                UserId = userId;
                UserName = userName;
                Email = email;
            }

            public string AuthenticationType { get; }

            public string Email { get; }

            public string UserId { get; }

            public string UserName { get; }
        }
    }
}