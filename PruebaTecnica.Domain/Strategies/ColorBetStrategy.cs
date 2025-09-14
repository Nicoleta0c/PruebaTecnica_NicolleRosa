using PruebaTecnica.Domain.Entities;
using PruebaTecnica.Domain.Interfaces;

namespace PruebaTecnica.Domain.Strategies
{
    public class ColorBetStrategy : IBetStrategy
    {
        // Gana la mitad de lo apostado si acierta el color
        public decimal CalculateWin(Bet bet, RouletteResult result)
        {
            if (!string.IsNullOrEmpty(bet.Color) && bet.Color == result.Color)
            {
                return bet.Amount * 0.5m;
            }
            return -bet.Amount;
        }
    }
}
