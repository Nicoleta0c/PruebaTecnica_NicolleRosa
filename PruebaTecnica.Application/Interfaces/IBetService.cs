using PruebaTecnica.Application.DTOs;
using PruebaTecnica.Domain.Entities;
using System.Threading.Tasks;

namespace PruebaTecnica.Application.Interfaces
{
   
    public interface IBetService
    {
       
        Task<BetResultDto> PlaceBetAsync(BetRequestDto request);
        RouletteResult GenerateSpin();
    }
}
