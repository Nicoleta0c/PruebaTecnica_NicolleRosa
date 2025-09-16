using PruebaTecnica.Application.DTOs;
using PruebaTecnica.Domain.Entities;
using System.Threading.Tasks;

namespace PruebaTecnica.Application.Interfaces
{
   
    public interface IBetService
    {
        Task<BetResultDto> PlaceBetAsync(BetRequestDto request);
        RouletteResult GenerateSpin();
        Task AddBalanceAsync(string name, decimal amountToAdd);
        Task CommitBetAsync(string name, decimal newBalance);
    }
}
