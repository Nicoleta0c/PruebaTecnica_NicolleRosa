using PruebaTecnica.Domain.Entities;
using PruebaTecnica.Domain.Interfaces;

namespace PruebaTecnica.Domain.Strategies
{
    public class NumberBetStrategy : IBetStrategy
    {
        // Gana el triple de lo apostado si acierta el número y el color
        public decimal CalculateWin(Bet bet, RouletteResult result)
        {
            if (bet.Number.HasValue && !string.IsNullOrEmpty(bet.Color) &&
                bet.Number == result.Number && bet.Color == result.Color)
            {
                return bet.Amount * 3; 
            }
            return -bet.Amount; 
        }
    }
}
