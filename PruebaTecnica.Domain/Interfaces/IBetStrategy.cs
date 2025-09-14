using PruebaTecnica.Domain.Entities;

namespace PruebaTecnica.Domain.Interfaces
{
    public interface IBetStrategy
    {
        decimal CalculateWin(Bet bet, RouletteResult result);
    }
}
