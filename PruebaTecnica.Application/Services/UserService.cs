using PruebaTecnica.Application.Interfaces;
using PruebaTecnica.Domain.Entities;
using System.Threading.Tasks;

namespace PruebaTecnica.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly InMemoryUserStore _userStore;

        public UserService(IUserRepository userRepository, InMemoryUserStore userStore)
        {
            _userRepository = userRepository;
            _userStore = userStore;
        }

        //cargar usuario esde la BD si no está en memoria
        public async Task<User> LoadUserAsync(string name)
        {
            var user = _userStore.GetOrCreateUser(name);
            // Si el usuario no tiene saldo en memoria, intentar cargar desde la BD
            if (user.Balance == 0)
            {
                string normalizedName = name.ToLower();
                var existingUser = await _userRepository.GetByNameAsync(normalizedName);
                if (existingUser != null)
                {
                    user.Balance = existingUser.Balance;
                    _userStore.UpdateUser(user);
                }
            }
            return user;
        }

        //limpiar memoria

        public void ClearMemory()
        {
            _userStore.ClearMemory();
        }
        public void SaveUser(User user)
        {
            _userStore.UpdateUser(user);
        }

        public async Task AddBalanceAsync(string name, decimal amountToAdd)
        {
            if (amountToAdd < 0)
                throw new ArgumentException("Amount to add cannot be negative");

            var user = await LoadUserAsync(name);
            // Actualizar el saldo en memoria
            user.Balance += amountToAdd;
            SaveUser(user);
        }

        public async Task CommitUserToDbAsync(string name)
        {
            string normalizedName = name.ToLower();
            var user = await LoadUserAsync(name);
            if (user == null)
                throw new InvalidOperationException($"User {name} not found in memory");

            // Guardar en la BD
            await _userRepository.SaveAsync(user);
        }
    }
}