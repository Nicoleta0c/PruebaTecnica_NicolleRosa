using PruebaTecnica.Domain.Enums;

namespace PruebaTecnica.Domain.Entities
{
    public class Bet
    {
        public decimal Amount { get; set; }
        public BetType BetType { get; set; }
        public string? Color { get; set; }  
        public bool? IsEven { get; set; }  
        public int? Number { get; set; }   
    }
}
