using PruebaTecnica.Domain.Entities;
using System.Threading.Tasks;

namespace PruebaTecnica.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByNameAsync(string name); 
        Task SaveAsync(User user);              
    }
}
