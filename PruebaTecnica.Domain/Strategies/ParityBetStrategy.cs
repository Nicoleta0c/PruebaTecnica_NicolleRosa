using PruebaTecnica.Domain.Entities;
using PruebaTecnica.Domain.Interfaces;

namespace PruebaTecnica.Domain.Strategies
{
    public class ParityBetStrategy : IBetStrategy
    {
        // Gana lo apostado si acierta el color y si es par o impar
        public decimal CalculateWin(Bet bet, RouletteResult result)
        {
            if (!string.IsNullOrEmpty(bet.Color) && bet.Color == result.Color &&
                bet.IsEven.HasValue && ((result.Number % 2 == 0) == bet.IsEven.Value))
            {
                return bet.Amount; 
            }
            return -bet.Amount;
        }
    }
}
