using PruebaTecnica.Application.Interfaces;
using PruebaTecnica.Domain.Entities;
using System.Threading.Tasks;

namespace PruebaTecnica.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> LoadUserAsync(string name)
        {
            var user = await _userRepository.GetByNameAsync(name);
            if (user == null)
            {
                // Si no existe, crea uno nuevo con saldo 0
                user = new User { Name = name, Balance = 0 };
                await _userRepository.SaveAsync(user);
            }
            return user;
        }

        public async Task SaveUserAsync(User user)
        {
            await _userRepository.SaveAsync(user);
        }
    }
}
