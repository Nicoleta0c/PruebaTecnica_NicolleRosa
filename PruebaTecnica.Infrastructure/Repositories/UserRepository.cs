using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Application.Interfaces;
using PruebaTecnica.Domain.Entities;
using PruebaTecnica.Infrastructure.Data;
using System.Threading.Tasks;

namespace PruebaTecnica.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // Obtiene un usuario por su nombre, ignorando mayúsculas y minúsculas
        public async Task<User?> GetByNameAsync(string name)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Name.ToLower() == name.ToLower());
        }

        // Guarda o actualiza un usuario en la base de datos
        public async Task SaveAsync(User user)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Name.ToLower() == user.Name.ToLower());

            if (existingUser != null)
            {
                existingUser.Balance = user.Balance;
                _context.Users.Update(existingUser);
            }
            else
            {
                await _context.Users.AddAsync(user);
            }

            await _context.SaveChangesAsync();
        }
    }
}
