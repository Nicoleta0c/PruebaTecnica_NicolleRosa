using PruebaTecnica.Domain.Entities;
using System.Collections.Concurrent;

namespace PruebaTecnica.Application.Services
{
    public class InMemoryUserStore
    {
        private readonly ConcurrentDictionary<string, User> _inMemoryUsers =
            new ConcurrentDictionary<string, User>(StringComparer.OrdinalIgnoreCase);

        public User GetOrCreateUser(string name)
        {
            string normalizedName = name.ToLower();
            return _inMemoryUsers.GetOrAdd(normalizedName, new User { Name = name, Balance = 0 });
        }

        public void UpdateUser(User user)
        {
            string normalizedName = user.Name.ToLower();
            _inMemoryUsers[normalizedName] = user;
        }

        public void ClearMemory()
        {
            _inMemoryUsers.Clear();
        }
    }
}